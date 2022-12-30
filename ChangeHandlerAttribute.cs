namespace UECS;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ChangeHandlerAttribute : Attribute
{
    public bool Inheritance { get; }

    public ChangeHandlerAttribute(bool inheritance = false)
    {
        Inheritance = inheritance;
    }
} 