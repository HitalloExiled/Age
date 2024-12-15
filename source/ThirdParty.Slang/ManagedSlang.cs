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
}
