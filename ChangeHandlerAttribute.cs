namespace UECS;

[AttributeUsage(AttributeTargets.Field)]
public class ChangeHandlerAttribute : Attribute
{
    public bool Inheritance { get; }

    public ChangeHandlerAttribute(bool inheritance = false)
    {
        Inheritance = inheritance;
    }
} 