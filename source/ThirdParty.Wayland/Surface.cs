namespace ThirdParty.Wayland;

public unsafe class Surface : Proxy<wl_surface>
{
    private const uint WL_SURFACE_COMMIT = 6;

    internal Surface(Handle<wl_surface> handle) : base(handle)
    { }

    public void Commit()
    {
        var proxy = (wl_proxy*)this.Handle.Value;

        PInvoke.wl_proxy_marshal_flags(
            proxy,
            WL_SURFACE_COMMIT,
            null,
            PInvoke.wl_proxy_get_version(proxy),
            0
        );
    }
}
