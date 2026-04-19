namespace ThirdParty.Wayland;

public abstract class Proxy<T> : DisposableManaged<T> where T : unmanaged
{
    internal Proxy(Handle<T> handle) : base(handle)
    { }

    protected unsafe override void OnDisposed(bool disposing) =>
        PInvoke.wl_proxy_destroy((wl_proxy*)this.Handle.Value);
}
