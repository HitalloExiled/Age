using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;

namespace ThirdParty.Wayland;

public struct xdg_surface;
public struct xdg_toplevel;

public unsafe struct wl_array
{
	public size_t size;
	public size_t alloc;
	public void*  data;
}

public unsafe struct xdg_surface_listener
{
    public required delegate* unmanaged<void*, xdg_surface*, uint32_t, void> configure;
}

public unsafe struct xdg_toplevel_listener
{
    public required delegate* unmanaged<void*, xdg_toplevel*, int32_t, int32_t, wl_array*, void> configure;
    public required delegate* unmanaged<void*, xdg_toplevel*, void>                              close;
    public required delegate* unmanaged<void*, xdg_toplevel*, int32_t, int32_t, void>            configure_bounds;
    public required delegate* unmanaged<void*, xdg_toplevel*, wl_array*, void>                   wm_capabilities;

}


[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class XdgShellProtocol
{
    private static readonly wl_interface** xdg_shell_types;

    public static wl_message*   xdg_wm_base_requests;
    public static wl_message*   xdg_wm_base_events;
    public static wl_interface* xdg_wm_base_interface;
    public static wl_message*   xdg_positioner_requests;
    public static wl_interface* xdg_positioner_interface;
    public static wl_message*   xdg_surface_requests;
    public static wl_message*   xdg_surface_events;
    public static wl_interface* xdg_surface_interface;
    public static wl_message*   xdg_toplevel_requests;
    public static wl_message*   xdg_toplevel_events;
    public static wl_interface* xdg_toplevel_interface;
    public static wl_message*   xdg_popup_requests;
    public static wl_message*   xdg_popup_events;
    public static wl_interface* xdg_popup_interface;

    static XdgShellProtocol()
    {
        const int TYPES_COUNT = 25;

        xdg_shell_types = (wl_interface**)NativeMemory.AllocZeroed((nuint)(sizeof(wl_interface**) * TYPES_COUNT));

        xdg_wm_base_requests = new NativeRefArray<wl_message>([
            new(new UnmanagedString("destroy"),           new UnmanagedString(""),   xdg_shell_types + 0),
            new(new UnmanagedString("create_positioner"), new UnmanagedString("n"),  xdg_shell_types + 4),
            new(new UnmanagedString("get_xdg_surface"),   new UnmanagedString("no"), xdg_shell_types + 5),
            new(new UnmanagedString("pong"),              new UnmanagedString("u"),  xdg_shell_types + 5),
        ]);

        xdg_wm_base_events = new NativeRefArray<wl_message>([
            new(new UnmanagedString("ping"), new UnmanagedString("u"), xdg_shell_types + 0),
        ]);

        xdg_wm_base_interface = NativeMemory.AllocSet<wl_interface>(
            new(
                new UnmanagedString("xdg_wm_base"), 7,
                4, xdg_wm_base_requests,
                1, xdg_wm_base_events
            )
        );

        xdg_positioner_requests = new NativeRefArray<wl_message>([
            new(new UnmanagedString("destroy"),                   new UnmanagedString(""),     xdg_shell_types + 0),
            new(new UnmanagedString("set_size"),                  new UnmanagedString("ii"),   xdg_shell_types + 0),
            new(new UnmanagedString("set_anchor_rect"),           new UnmanagedString("iiii"), xdg_shell_types + 0),
            new(new UnmanagedString("set_anchor"),                new UnmanagedString("u"),    xdg_shell_types + 0),
            new(new UnmanagedString("set_gravity"),               new UnmanagedString("u"),    xdg_shell_types + 0),
            new(new UnmanagedString("set_constraint_adjustment"), new UnmanagedString("u"),    xdg_shell_types + 0),
            new(new UnmanagedString("set_offset"),                new UnmanagedString("ii"),   xdg_shell_types + 0),
            new(new UnmanagedString("set_reactive"),              new UnmanagedString("3"),    xdg_shell_types + 0),
            new(new UnmanagedString("set_parent_size"),           new UnmanagedString("3ii"),  xdg_shell_types + 0),
            new(new UnmanagedString("set_parent_configure"),      new UnmanagedString("3u"),   xdg_shell_types + 0),
        ]);

        xdg_positioner_interface = NativeMemory.AllocSet<wl_interface>(
            new(
                new UnmanagedString("xdg_positioner"), 1,
                10, xdg_positioner_requests,
	            0, null
            )
        );

        xdg_surface_requests = new NativeRefArray<wl_message>([
            new(new UnmanagedString("destroy"),             new UnmanagedString(""),     xdg_shell_types + 0),
            new(new UnmanagedString("get_toplevel"),        new UnmanagedString("n"),    xdg_shell_types + 7),
            new(new UnmanagedString("get_popup"),           new UnmanagedString("n?oo"), xdg_shell_types + 8),
            new(new UnmanagedString("set_window_geometry"), new UnmanagedString("iiii"), xdg_shell_types + 0),
            new(new UnmanagedString("ack_configure"),       new UnmanagedString("u"),    xdg_shell_types + 0),
        ]);

        xdg_surface_events = new NativeRefArray<wl_message>([
            new(new UnmanagedString("configure"), new UnmanagedString("u"), xdg_shell_types + 0)
        ]);

        xdg_surface_interface = NativeMemory.AllocSet<wl_interface>(
            new(
                new UnmanagedString("xdg_surface"), 7,
                5, xdg_surface_requests,
	            1, xdg_surface_events
            )
        );

        xdg_toplevel_requests = new NativeRefArray<wl_message>([
            new(new UnmanagedString("destroy"),          new UnmanagedString(""),      xdg_shell_types + 0),
            new(new UnmanagedString("set_parent"),       new UnmanagedString("?)o"),   xdg_shell_types + 11),
            new(new UnmanagedString("set_title"),        new UnmanagedString("s)"),    xdg_shell_types + 0),
            new(new UnmanagedString("set_app_id"),       new UnmanagedString("s)"),    xdg_shell_types + 0),
            new(new UnmanagedString("show_window_menu"), new UnmanagedString("o)uii"), xdg_shell_types + 12),
            new(new UnmanagedString("move"),             new UnmanagedString("o)u"),   xdg_shell_types + 16),
            new(new UnmanagedString("resize"),           new UnmanagedString("o)uu"),  xdg_shell_types + 18),
            new(new UnmanagedString("set_max_size"),     new UnmanagedString("i)i"),   xdg_shell_types + 0),
            new(new UnmanagedString("set_min_size"),     new UnmanagedString("i)i"),   xdg_shell_types + 0),
            new(new UnmanagedString("set_maximized"),    new UnmanagedString(""),      xdg_shell_types + 0),
            new(new UnmanagedString("unset_maximized"),  new UnmanagedString(""),      xdg_shell_types + 0),
            new(new UnmanagedString("set_fullscreen"),   new UnmanagedString("?)o"),   xdg_shell_types + 21),
            new(new UnmanagedString("unset_fullscreen"), new UnmanagedString(""),      xdg_shell_types + 0),
            new(new UnmanagedString("set_minimized"),    new UnmanagedString(""),      xdg_shell_types + 0),
        ]);

        xdg_toplevel_events = new NativeRefArray<wl_message>([
            new(new UnmanagedString("configure"),        new UnmanagedString("iia"), xdg_shell_types + 0),
            new(new UnmanagedString("close"),            new UnmanagedString(""),    xdg_shell_types + 0),
            new(new UnmanagedString("configure_bounds"), new UnmanagedString("4ii"), xdg_shell_types + 0),
            new(new UnmanagedString("wm_capabilities"),  new UnmanagedString("5a"),  xdg_shell_types + 0),
        ]);

        xdg_toplevel_interface = NativeMemory.AllocSet<wl_interface>(
            new(
                new UnmanagedString("xdg_toplevel"), 7,
                14, xdg_toplevel_requests,
	            4, xdg_toplevel_events
            )
        );

        xdg_popup_requests = new NativeRefArray<wl_message>([
            new(new UnmanagedString("destroy"), new UnmanagedString(""), xdg_shell_types + 0)
        ]);

        xdg_popup_events = new NativeRefArray<wl_message>([
            new(new UnmanagedString("popup_done"), new UnmanagedString(""), xdg_shell_types + 0),
        ]);

        xdg_popup_interface = NativeMemory.AllocSet<wl_interface>(
            new(
                new UnmanagedString("xdg_popup"), 1,
                3, xdg_popup_requests,
	            3, xdg_popup_events
            )
        );

        xdg_shell_types[4]  = xdg_positioner_interface;
        xdg_shell_types[5]  = xdg_surface_interface;
        xdg_shell_types[6]  = ClientProtocol.wl_surface_interface;
        xdg_shell_types[7]  = xdg_toplevel_interface;
        xdg_shell_types[8]  = xdg_popup_interface;
        xdg_shell_types[9]  = xdg_surface_interface;
        xdg_shell_types[10] = xdg_positioner_interface;
        xdg_shell_types[11] = xdg_toplevel_interface;
        xdg_shell_types[12] = ClientProtocol.wl_seat_interface;

        xdg_shell_types[16] = ClientProtocol.wl_seat_interface;

        xdg_shell_types[18] = ClientProtocol.wl_seat_interface;

        xdg_shell_types[21] = ClientProtocol.wl_output_interface;
        xdg_shell_types[22] = ClientProtocol.wl_seat_interface;

        xdg_shell_types[24] = xdg_positioner_interface;
    }
}
