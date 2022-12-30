namespace UECS;

public abstract record ComponentBase
{
    public ComponentBase()
    {
        
    }

    public void Update()
    {
        SystemsController.UpdateComponent(this);
    }
}