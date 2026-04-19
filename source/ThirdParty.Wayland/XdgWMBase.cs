namespace ThirdParty.Wayland;

public class XdgWMBase : Proxy<xdg_wm_base>
{
    private const uint XDG_WM_BASE_GET_XDG_SURFACE = 2;

    internal XdgWMBase(Handle<xdg_wm_base> handle) : base(handle)
    { }

    public unsafe XdgSurface CreateSurface(Surface surface)
    {
        var xdgSurface = PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            XDG_WM_BASE_GET_XDG_SURFACE,
            XdgShellProtocol.xdg_surface_interface,
            PInvoke.wl_proxy_get_version((wl_proxy*)this.Handle.Value),
            0,
            null,
            (wl_surface*)surface.Handle
        );

        return new(xdgSurface);
    }
}
