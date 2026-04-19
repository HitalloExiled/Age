using System.Runtime.InteropServices;
using System.Security;
using Age.Core;
using Age.Core.Extensions;

namespace ThirdParty.Wayland;

public delegate void XdgTopLevelConfiguredHandler(int width, int height, ReadOnlySpan<uint> states);
public delegate void XdgTopLevelCloseHandler();
public delegate void XdgTopLevelConfigureBoundsHandler(int width, int height);
public delegate void XdgTopLevelWMCapabilitiesHandler(ReadOnlySpan<uint> capabilities);

public unsafe class XdgTopLevel : Proxy<xdg_toplevel>
{
    private const uint XDG_TOPLEVEL_SET_TITLE  = 2;
    private const uint XDG_TOPLEVEL_SET_APP_ID = 3;

    private event XdgTopLevelCloseHandler?           closed;
    private event XdgTopLevelConfiguredHandler?      configured;
    private event XdgTopLevelConfigureBoundsHandler? configuredBounds;
    private event XdgTopLevelWMCapabilitiesHandler?  wMCapabilities;

    public event XdgTopLevelCloseHandler? Closed
    {
        add
        {
            closed += value;

            this.EnsureNativeListener();
        }
        remove
        {
            closed -= value;

            this.TryDetachNativeListener();
        }
    }

    public event XdgTopLevelConfiguredHandler? Configured
    {
        add
        {
            configured += value;

            this.EnsureNativeListener();
        }
        remove
        {
            configured -= value;

            this.TryDetachNativeListener();
        }
    }

    public event XdgTopLevelConfigureBoundsHandler? ConfiguredBounds
    {
        add
        {
            configuredBounds += value;

            this.EnsureNativeListener();
        }
        remove
        {
            configuredBounds -= value;

            this.TryDetachNativeListener();
        }
    }

    public event XdgTopLevelWMCapabilitiesHandler? WMCapabilities
    {
        add
        {
            wMCapabilities += value;

            this.EnsureNativeListener();
        }
        remove
        {
            wMCapabilities -= value;

            this.TryDetachNativeListener();
        }
    }

    private static readonly Dictionary<Handle<xdg_toplevel>, XdgTopLevel> instances = [];

    private xdg_toplevel_listener* nativeListener;

    internal XdgTopLevel(Handle<xdg_toplevel> handle) : base(handle) =>
        instances[handle] = this;

    [UnmanagedCallersOnly]
    private static void CloseCallback(void* data, xdg_toplevel* xdgToplevel)
    {
        var instance = instances[xdgToplevel];

        instance.closed?.Invoke();
    }

    [UnmanagedCallersOnly]
    private static void ConfigureBoundsCallback(void* data, xdg_toplevel* xdgToplevel, int32_t width, int32_t height)
    {
        var instance = instances[xdgToplevel];

        instance.configuredBounds?.Invoke(width, height);
    }

    [UnmanagedCallersOnly]
    private static void ConfigureCallback(void* data, xdg_toplevel* xdgToplevel, int32_t width, int32_t height, wl_array* states)
    {
        var instance = instances[xdgToplevel];

        var statesSpan = new ReadOnlySpan<uint>(states->data, (int)(states->size / sizeof(uint)));

        instance.configured?.Invoke(width, height, statesSpan);
    }

    [UnmanagedCallersOnly]
    private static void WMCapabilitiesCallback(void* data, xdg_toplevel* xdgToplevel, wl_array* capabilities)
    {
        var instance = instances[xdgToplevel];

        var capabilitiesSpan = new ReadOnlySpan<uint>(capabilities->data, (int)(capabilities->size / sizeof(uint)));

        instance.wMCapabilities?.Invoke(capabilitiesSpan);
    }

    private void EnsureNativeListener()
    {
        if (this.nativeListener == null)
        {
            this.nativeListener = NativeMemory.AllocSet<xdg_toplevel_listener>(
                new()
                {
                    configure        = &ConfigureCallback,
                    close            = &CloseCallback,
                    configure_bounds = &ConfigureBoundsCallback,
                    wm_capabilities  = &WMCapabilitiesCallback
                }
            );

            WaylandException.Check(PInvoke.xdg_toplevel_add_listener(this.Handle, this.nativeListener, null), "Failed to add listener");
        }
    }

    private void TryDetachNativeListener()
    {
        if (this.nativeListener != null && configured == null && closed == null)
        {
            WaylandException.Check(PInvoke.xdg_toplevel_add_listener(this.Handle, null, null), "Failed to remove listener");

            NativeMemory.Free(this.nativeListener);

            this.nativeListener = null;
        }
    }

    protected override void OnDisposed(bool disposing)
    {
        base.OnDisposed(disposing);

        instances.Remove(this.Handle);
    }

    public void SetTitle(string title)
    {
        var uTitle = new UnmanagedString(title);

        PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            XDG_TOPLEVEL_SET_TITLE,
            null,
            PInvoke.wl_proxy_get_version((wl_proxy*)this.Handle.Value),
            0,
            uTitle
        );
    }

    public void SetAppId(string appId)
    {
        var uAppId = new UnmanagedString(appId);

        PInvoke.wl_proxy_marshal_flags(
            this.Handle,
            XDG_TOPLEVEL_SET_APP_ID,
            null,
            PInvoke.wl_proxy_get_version((wl_proxy*)this.Handle.Value),
            0,
            uAppId
        );
    }
}
