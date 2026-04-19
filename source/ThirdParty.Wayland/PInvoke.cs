using System.Runtime.InteropServices;

namespace ThirdParty.Wayland;

internal unsafe static partial class PInvoke
{
    internal const string LIBRARY = "libwayland-client.so.0";

    [LibraryImport(LIBRARY)]
    public static partial wl_display* wl_display_connect(byte* name);

    // wl_display

    [LibraryImport(LIBRARY)]
    public static partial void wl_display_disconnect(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_dispatch_pending(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_flush(wl_display *display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_prepare_read(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_read_events(wl_display *display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_roundtrip(wl_display* display);

    // wl_proxy

    [LibraryImport(LIBRARY)]
    public static partial uint32_t wl_proxy_get_version(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_destroy(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags);

    // wl_proxy - xdg_wm_base

    [LibraryImport(LIBRARY)]
    public static partial xdg_surface* wl_proxy_marshal_flags(xdg_wm_base* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, byte* id, wl_surface* surface);

    // wl_proxy - xdg_surface

    [LibraryImport(LIBRARY)]
    public static partial void* wl_proxy_marshal_flags(xdg_surface* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, uint serial);

    [LibraryImport(LIBRARY)]
    public static partial xdg_toplevel* wl_proxy_marshal_flags(xdg_surface* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, void* id);

    [LibraryImport(LIBRARY, EntryPoint = nameof(wl_proxy_add_listener))]
    public static partial int xdg_surface_add_listener(xdg_surface* proxy, xdg_surface_listener* implementation, void* data);

    // wl_proxy - xdg_toplevel

    [LibraryImport(LIBRARY)]
    public static partial xdg_toplevel* wl_proxy_marshal_flags(xdg_toplevel* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, byte* value);

    [LibraryImport(LIBRARY, EntryPoint = nameof(wl_proxy_add_listener))]
    public static partial int xdg_toplevel_add_listener(xdg_toplevel* proxy, xdg_toplevel_listener* implementation, void* data);

    // wl_proxy - wl_registry

    [LibraryImport(LIBRARY)]
    public static partial int wl_proxy_add_listener(wl_registry* proxy, wl_registry_listener* implementation, void* data);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_registry* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, uint name, byte* @interfaceName, uint bindVersion, void* nullTerminator);
}
