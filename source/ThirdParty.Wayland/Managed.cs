namespace ThirdParty.Wayland;

public abstract class Managed<T> where T : unmanaged
{
    internal Handle<T> Handle { get; }

    internal Managed(Handle<T> handle) => this.Handle = handle;
}
