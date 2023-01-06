using System.Collections;
using System.Reflection;
using UECS.Exceptions;
using Utils;
using SystemException = UECS.Exceptions.SystemException;

namespace UECS;

public sealed class SystemsController
{
    private static readonly Dictionary<SystemsController, List<ComponentBase>> AllControllers = new();
    private static readonly Type FilterType = typeof(IFilter);

    public static SystemsController MainController => AllControllers.Keys.FirstOrDefault();
    public static IEnumerable<SystemsController> Controllers => AllControllers.Keys;

    private readonly List<SystemAction> _syncSystems = new();
    private readonly List<SystemAction> _asyncSystems = new();
    private readonly Dictionary<Type, List<ComponentBase>> _components = new();
    private Dictionary<Type, List<ComponentBase>> _syncChangedComponents = new();
    private Dictionary<Type, List<ComponentBase>> _asyncChangedComponents = new();

    private Dictionary<Type, List<ComponentBase>> _updateSyncChangedComponents = new();
    private Dictionary<Type, List<ComponentBase>> _updateAsyncChangedComponents = new();
    
    private object _syncLock = new();
    private object _asyncLock = new();
    private object _allLock = new();
    private object _currLock = new();
    
    
    public SystemsController()
    {
        AllControllers.Add(this, new List<ComponentBase>());
    }
    public void Add(ComponentBase componentBase)
    {
        var type = componentBase.GetType();
        lock (_allLock)
        {
            AllControllers[this].Add(componentBase);
        }
        if (_components.ContainsKey(type))
        {
            var components = _components[type];
            lock (_currLock)
            {
                if (components.Contains(componentBase)) throw new SystemException("component already registered");
                components.Add(componentBase);
            }
        }
        else
            lock(_currLock)
                _components.Add(type, new List<ComponentBase>
                {
                    componentBase
                });
    }
    public void Remove(ComponentBase componentBase)
    {
        var type = componentBase.GetType();
        lock (_allLock)
        {
            AllControllers[this].Add(componentBase);
        }
        lock (_currLock)
        {
            if (!_components.ContainsKey(type))
                throw new SystemException("component is not registered in this controller");
            _components[type].Remove(componentBase);
        }
    }
    public void Register(IAsyncSystem system)
    {
        (system is ISyncSystem ? _syncSystems : _asyncSystems).Add(new SystemAction
        {
            System = system,
            Filters = GetSystemFilterFields(system),
            Handlers = GetSystemChangeHandlers(system)
        });
        system.Init();
    }

    public void UpdateSync()
    {
        lock (_syncLock)
        {
            _updateSyncChangedComponents = _syncChangedComponents;
            _syncChangedComponents = new Dictionary<Type, List<ComponentBase>>();
        }
        Update(_syncSystems, _updateSyncChangedComponents);
        _updateSyncChangedComponents.Clear();
    }

    public void UpdateAsync()
    {
        lock (_asyncLock)
        {
            _updateAsyncChangedComponents = _asyncChangedComponents;
            _asyncChangedComponents = new Dictionary<Type, List<ComponentBase>>();
        }
        Update(_asyncSystems, _updateAsyncChangedComponents);
        _updateAsyncChangedComponents.Clear();
    }

    private void Update(IReadOnlyList<SystemAction> list, Dictionary<Type, List<ComponentBase>> changedComponents)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var system = list[i];
            SetFilters(system);
            SetHandlers(system, changedComponents);
            system.System.Update();
        }
    } 
    private void SetFilters(SystemAction system)
    {
        foreach (var field in system.Filters)
        {
            var type = field.FieldType;
            var filterTypes = type.GetGenericArguments();
            var inheritance = ((AutoInjectAttribute)field.GetCustomAttributes().First(a => a is AutoInjectAttribute)).Inheritance;
            var result = filterTypes
                .Select(t =>
                {
                    if (!inheritance)
                        return _components.ContainsKey(t) ? _components[t] : Enumerable.Empty<ComponentBase>();
                    var hashSet = new HashSet<ComponentBase>();
                    lock (_currLock)
                    {
                        foreach (var key in _components.Keys.Where(key => key == t || key.IsSubclassOf(t)))
                            hashSet.UnionWith(_components[key]);
                    }
                    return hashSet.AsEnumerable();
                }).ToArray();
            field.SetValue(system.System,
                Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new object[] { result }, null));
        }
    }
    private void SetHandlers(SystemAction system, IReadOnlyDictionary<Type, List<ComponentBase>> changedComponents)
    {
        foreach (var field in system.Handlers)
        {
            var type = field.FieldType;
            var filterTypes = type.GetGenericArguments();
            var inheritance = ((ChangeHandlerAttribute)field.GetCustomAttributes().First(a => a is ChangeHandlerAttribute)).Inheritance;
            var result = filterTypes  
                .Select(t =>
                {
                    if (!inheritance)
                        return changedComponents.ContainsKey(t) ? changedComponents[t] : Enumerable.Empty<ComponentBase>();
                    var hashSet = new HashSet<ComponentBase>();
                    foreach (var key in changedComponents.Keys.Where(key => key == t || key.IsSubclassOf(t)))
                        hashSet.UnionWith(changedComponents[key]);
                    return hashSet.AsEnumerable();
                }).ToArray();
            field.SetValue(system.System,
                Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null, new object[] { result }, null));
        }
    }
    private static FieldInfo[] GetSystemFilterFields(IAsyncSystem system) =>
        system.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(f =>
            FilterType.IsAssignableFrom(f.FieldType) && f.GetCustomAttribute<AutoInjectAttribute>() is not null).ToArray();

    private static FieldInfo[] GetSystemChangeHandlers(IAsyncSystem system) =>
        system.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).Where(f =>
            FilterType.IsAssignableFrom(f.FieldType) && f.GetCustomAttribute<ChangeHandlerAttribute>() is not null).ToArray();

    internal static void UpdateComponent(ComponentBase component)
    {
        var controller = FindComponentController(component);
        
        ApplyUpdate(component, controller._asyncChangedComponents, controller._asyncLock);
        ApplyUpdate(component, controller._syncChangedComponents, controller._syncLock);
    }

    public static SystemsController FindComponentController(ComponentBase componentBase)
    {
        foreach (var (key, value) in AllControllers)
            if (value.Contains(componentBase))
                return key;
        return null;
    }

    private static void ApplyUpdate(ComponentBase component, IDictionary<Type, List<ComponentBase>> changedComponents,
        object l)
    {
        var type = component.GetType();
        lock (l)
        {
            if (changedComponents.ContainsKey(type))
            {
                var components = changedComponents[type];

                if (components.Contains(component)) return;
                components.Add(component);
            }
            else
                changedComponents.Add(type, new List<ComponentBase>
                {
                    component
                });
        }
    }

    private struct SystemAction
    {
        public IAsyncSystem System;
        public FieldInfo[] Filters;
        public FieldInfo[] Handlers;
    }
}