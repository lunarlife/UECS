using System.Collections;
using System.ComponentModel;

namespace UECS;


public interface IFilter
{
    protected IEnumerable<ComponentBase>[] Components { get; }
}


public readonly struct Filter<T> : IFilter, IEnumerable<FilterResult<T>>
    where T : ComponentBase
{
    private readonly IEnumerable<ComponentBase>[] _components;

    IEnumerable<ComponentBase>[] IFilter.Components => _components;

    private Filter(IEnumerable<ComponentBase>[] components)
    {
        _components = components;
    }
    public IEnumerator<FilterResult<T>> GetEnumerator() => new FilterEnumerator1(_components);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class FilterEnumerator1 : FilterEnumerator, IEnumerator<FilterResult<T>>
    {
        public FilterEnumerator1(IEnumerable<IEnumerable<ComponentBase>> components) : base(components)
        {
        }
        
        public new FilterResult<T> Current
        {
            get
            {
                var c = base.Current;
                return new FilterResult<T>(c[0] as T);
            }
        }
    }

}
public readonly struct Filter<T, T1> : IFilter, IEnumerable<FilterResult<T, T1>>
    where T : ComponentBase
    where T1 : ComponentBase
{
    private readonly IEnumerable<ComponentBase>[] _components;

    IEnumerable<ComponentBase>[] IFilter.Components => _components;

    internal Filter(IEnumerable<ComponentBase>[] components) => _components = components;
    public IEnumerator<FilterResult<T, T1>> GetEnumerator() => new FilterEnumerator1(_components);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class FilterEnumerator1 : FilterEnumerator, IEnumerator<FilterResult<T, T1>>
    {
        public FilterEnumerator1(IEnumerable<IEnumerable<ComponentBase>> components) : base(components)
        {
        }

        public new FilterResult<T, T1> Current
        {
            get
            {
                var c = base.Current;
                return new FilterResult<T, T1>(c[0] as T, c[1] as T1);
            }
        }
    }
}
public readonly struct Filter<T, T1, T2> : IFilter, IEnumerable<FilterResult<T, T1, T2>>
    where T : ComponentBase
    where T1 : ComponentBase
    where T2 : ComponentBase
{
    private readonly IEnumerable<ComponentBase>[] _components;

    IEnumerable<ComponentBase>[] IFilter.Components => _components;

    internal Filter(IEnumerable<ComponentBase>[] components)
    {
        _components = components;
    }
    public IEnumerator<FilterResult<T, T1, T2>> GetEnumerator() => new FilterEnumerator1(_components);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    private class FilterEnumerator1 : FilterEnumerator, IEnumerator<FilterResult<T, T1, T2>>
    {
        public FilterEnumerator1(IEnumerable<IEnumerable<ComponentBase>> components) : base(components)
        {
        }
        public new FilterResult<T, T1, T2> Current
        {
            get
            {
                var c = base.Current;
                return new FilterResult<T, T1, T2>(c[0] as T, c[1] as T1, c[2] as T2);
            }
        }
    }
}
public readonly struct Filter<T, T1, T2, T3> : IFilter, IEnumerable<FilterResult<T, T1, T2, T3>>
    where T : ComponentBase
    where T1 : ComponentBase
    where T2 : ComponentBase
    where T3 : ComponentBase
{
    private readonly IEnumerable<ComponentBase>[] _components;

    IEnumerable<ComponentBase>[] IFilter.Components => _components;

    internal Filter(IEnumerable<ComponentBase>[] components) => _components = components;

    public IEnumerator<FilterResult<T, T1, T2, T3>> GetEnumerator() => new FilterEnumerator1(_components);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    private class FilterEnumerator1 : FilterEnumerator, IEnumerator<FilterResult<T, T1, T2, T3>>
    {
        public FilterEnumerator1(IEnumerable<IEnumerable<ComponentBase>> components) : base(components)
        {
        }
        public new FilterResult<T, T1, T2, T3> Current
        {
            get
            {
                var c = base.Current;
                return new FilterResult<T, T1, T2, T3>(c[0] as T, c[1] as T1, c[2] as T2, c[3] as T3);
            }
        }
    }
}
public readonly struct Filter<T, T1, T2, T3, T4> : IFilter, IEnumerable<FilterResult<T, T1, T2, T3, T4>>
    where T : ComponentBase
    where T1 : ComponentBase
    where T2 : ComponentBase
    where T3 : ComponentBase
    where T4 : ComponentBase
{
    private readonly IEnumerable<ComponentBase>[] _components;

    IEnumerable<ComponentBase>[] IFilter.Components => _components;

    internal Filter(IEnumerable<ComponentBase>[] components)
    {
        _components = components;
    }

    public IEnumerator<FilterResult<T, T1, T2, T3, T4>> GetEnumerator() => new FilterEnumerator1(_components);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    private class FilterEnumerator1 : FilterEnumerator, IEnumerator<FilterResult<T, T1, T2, T3, T4>>
    {
        public FilterEnumerator1(IEnumerable<IEnumerable<ComponentBase>> components) : base(components)
        {
        }
        public new FilterResult<T, T1, T2, T3, T4> Current
        {
            get
            {
                var c = base.Current;
                return new FilterResult<T, T1, T2, T3, T4>(c[0] as T, c[1] as T1, c[2] as T2, c[3] as T3, c[3] as T4);
            }
        }
    }
}

internal class FilterEnumerator : IEnumerator<ComponentBase[]>
{
    private readonly IEnumerator<ComponentBase>[] _components;

    public FilterEnumerator(IEnumerable<IEnumerable<ComponentBase>> components) => _components = components.Select(c => c.GetEnumerator()).ToArray();

    public bool MoveNext() => _components.Any(c => c.MoveNext());

    public void Reset()
    {
        foreach (var enumerator in _components) enumerator.Reset();
    }

    public ComponentBase[] Current => _components.Select(c => c.Current).ToArray();

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        foreach (var enumerator in _components) enumerator.Dispose();
    }
}

public readonly struct FilterResult<T>
{
    private readonly T _result;

    public FilterResult(T result)
    {
        _result = result;
    }
    public T Get1() => _result;
}
public readonly struct FilterResult<T, T1>
{
    private readonly T _result1;
    private readonly T1 _result2;

    public FilterResult(T result1, T1 result2)
    {
        _result1 = result1;
        _result2 = result2;
    }
    public T Get1() => _result1;
    public T1 Get2() => _result2;
}
public readonly struct FilterResult<T, T1, T2>
{
    private readonly T _result1;
    private readonly T1 _result2;
    private readonly T2 _result3;

    public FilterResult(T result1, T1 result2, T2 result3)
    {
        _result1 = result1;
        _result2 = result2;
        _result3 = result3;
    }
    public T Get1() => _result1;
    public T1 Get2() => _result2;
    public T2 Get3() => _result3;
}
public readonly struct FilterResult<T, T1, T2, T3>
{
    private readonly T _result1;
    private readonly T1 _result2;
    private readonly T2 _result3;
    private readonly T3 _result4;

    public FilterResult(T result1, T1 result2, T2 result3, T3 result4)
    {
        _result1 = result1;
        _result2 = result2;
        _result3 = result3;
        _result4 = result4;
    }
    public T Get1() => _result1;
    public T1 Get2() => _result2;
    public T2 Get3() => _result3;
    public T3 Get4() => _result4;
}

public readonly struct FilterResult<T, T1, T2, T3, T4>
{
    private readonly T _result1;
    private readonly T1 _result2;
    private readonly T2 _result3;
    private readonly T3 _result4;
    private readonly T4 _result5;

    public FilterResult(T result1, T1 result2, T2 result3, T3 result4, T4 result5)
    {
        _result1 = result1;
        _result2 = result2;
        _result3 = result3;
        _result4 = result4;
        _result5 = result5;
    }
    public T Get1() => _result1;
    public T1 Get2() => _result2;
    public T2 Get3() => _result3;
    public T3 Get4() => _result4;
    public T4 Get5() => _result5;
}


