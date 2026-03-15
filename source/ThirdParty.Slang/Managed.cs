namespace ThirdParty.Slang;

public abstract class Managed<T>
{
    internal Handle<T> Handle { get; }

    public Session Session { get; }

    internal Managed(Session session, Handle<T> handle)
    {
        if (handle.Value == default)
        {
            throw new InvalidOperationException();
        }

        this.Session = session;
        this.Handle  = handle;
    }

    public static bool operator == (Managed<T> left, Managed<T> right) => left.Handle == right.Handle;
    public static bool operator != (Managed<T> left, Managed<T> right) => left.Handle != right.Handle;

    public override bool Equals(object? obj) =>
        obj is Managed<T> managed && managed == this;

    public override int GetHashCode() =>
        this.Handle.GetHashCode();
}
