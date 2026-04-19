using Age.Core;

namespace ThirdParty.Wayland;

public unsafe class Display(string? name = null) : DisposableManaged<wl_display>(Create(name))
{
    private const int WL_DISPLAY_GET_REGISTRY = 1;

    public Registry Registry
    {
        get
        {
            if (field == null)
            {
                var proxy = (wl_proxy*)this.Handle.Value;

                var registry = (wl_registry*)PInvoke.wl_proxy_marshal_flags(proxy, WL_DISPLAY_GET_REGISTRY, ClientProtocol.wl_registry_interface, PInvoke.wl_proxy_get_version(proxy), 0);

                field = new(registry);
            }

            return field;
        }
    }

    private static Handle<wl_display> Create(string? name)
    {
        using var uName = new UnmanagedString(name);

        return Handle<wl_display>.EnsureNotNull(PInvoke.wl_display_connect(uName), "Can't connect to a Wayland display.");
    }

    protected override void OnDisposed(bool disposing) =>
        PInvoke.wl_display_disconnect(this.Handle);

    public void DispatchPending() =>
        WaylandException.Check(PInvoke.wl_display_dispatch_pending(this.Handle));

    public void Flush() =>
        WaylandException.Check(PInvoke.wl_display_flush(this.Handle));

    public bool PrepareRead() =>
        PInvoke.wl_display_prepare_read(this.Handle) != 0;

    public void ReadEvents() =>
        WaylandException.Check(PInvoke.wl_display_read_events(this.Handle));

    public void RoundTrip() =>
        WaylandException.Check(PInvoke.wl_display_roundtrip(this.Handle));
}
