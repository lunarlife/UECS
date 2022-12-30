namespace UECS;

[AttributeUsage(AttributeTargets.Field)]
public sealed class AutoInjectAttribute : Attribute
{
    public bool Inheritance { get; }

    public AutoInjectAttribute(bool inheritance = false)
    {
        Inheritance = inheritance;
    }
} 