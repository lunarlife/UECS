namespace UECS;

[AttributeUsage(AttributeTargets.Field)]
public class AutoInjectAttribute : Attribute
{
    public bool Inheritance { get; }

    public AutoInjectAttribute(bool inheritance = false)
    {
        Inheritance = inheritance;
    }
} 