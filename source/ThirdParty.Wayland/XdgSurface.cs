using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace ThirdParty.Wayland;

public delegate void XdgSurfaceConfiguredHandler(uint serial);

public unsafe class XdgSurface : Proxy<xdg_surface>
{
    private const uint XDG_SURFACE_ACK_CONFIGURE = 4;
    private const uint XDG_SURFACE_GET_TOPLEVEL  = 1;

    private event XdgSurfaceConfiguredHandler? configured;

    public event XdgSurfaceConfiguredHandler? Configured
    {
        add
        {
            configured += value;

            if (this.nativeListener == null)
            {
                this.nativeListener = NativeMemory.AllocSet<xdg_surface_listener>(
                    new()
                    {
                        configure = &ConfigureCallback,
                    }
                );

                WaylandException.Check(PInvoke.xdg_surface_add_listener(this.Handle, this.nativeListener, null), "Failed to add listener");
            }
        }
        remove
        {
            configured -= value;

            if (configured == null)
            {
                WaylandException.Check(PInvoke.xdg_surface_add_listener(this.Handle, null, null), "Failed to remove listener");

                NativeMemory.Free(this.nativeListener);

                this.nativeListener = null;
            }
        }
    }

    private static readonly Dictionary<Handle<xdg_surface>, XdgSurface> instances = [];

    private xdg_surface_listener* nativeListener;

    internal XdgSurface(Handle<xdg_surface> handle) : base(handle) =>
        instances[handle] = this;

    [UnmanagedCallersOnly]
    private static void ConfigureCallback(void* data, xdg_surface* xdgSurface, uint serial)
    {
        var instance = instances[xdgSurface];

        instance.configured?.Invoke(serial);
    }

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);

        instances.Remove(this.Handle);
    }

    public void AckConfigure(uint serial) =>
        _ = PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            XDG_SURFACE_ACK_CONFIGURE,
            null,
            PInvoke.wl_proxy_get_version((wl_proxy*)this.Handle.Value),
            0,
            serial
        );

    public XdgTopLevel GetTopLevel()
    {
        var topLevel = PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            XDG_SURFACE_GET_TOPLEVEL,
            XdgShellProtocol.xdg_toplevel_interface,
            PInvoke.wl_proxy_get_version((wl_proxy*)this.Handle.Value),
            0,
            null
        );

        return new(topLevel);
    }
}
