namespace ThirdParty.Wayland;

public class Compositor : Proxy<wl_compositor>
{
    private const uint WL_COMPOSITOR_CREATE_SURFACE = 0;

    internal Compositor(Handle<wl_compositor> handle) : base(handle)
    { }

    public unsafe Surface CreateSurface()
    {
        var proxy = (wl_proxy*)this.Handle.Value;

	    var surface = (wl_surface*)PInvoke. wl_proxy_marshal_flags(
            proxy,
            WL_COMPOSITOR_CREATE_SURFACE,
            ClientProtocol.wl_surface_interface,
            PInvoke.wl_proxy_get_version(proxy),
            0
        );

	    return new(surface);
    }
}
