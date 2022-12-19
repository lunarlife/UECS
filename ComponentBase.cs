namespace UECS;

public abstract record ComponentBase
{
    public SystemsController TargetController { get; }
    public ComponentBase(SystemsController controller = null)
    {
        controller ??= SystemsController.MainController;
        (controller ?? throw new SystemException("Controller not found")).Add(this);
        TargetController = controller;
    }

    public void Update()
    {
        SystemsController.UpdateComponent(this);
    }
}