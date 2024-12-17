namespace ThirdParty.Slang;

public abstract class ManagedSlang
{
    internal nint Handle { get; }

    internal ManagedSlang(nint handle)
    {
        if (handle == default)
        {
            throw new InvalidOperationException();
        }

        this.Handle = handle;
    }

    public static bool operator == (ManagedSlang left, ManagedSlang right) => left.Handle == right.Handle;
    public static bool operator != (ManagedSlang left, ManagedSlang right) => left.Handle != right.Handle;

    public override bool Equals(object? obj) =>
        obj is ManagedSlang managed && managed.Handle == this.Handle;

    public override int GetHashCode() =>
        this.Handle.GetHashCode();
}
