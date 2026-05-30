using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct xdg_surface;
internal struct xdg_toplevel;
internal struct xdg_wm_base;

internal unsafe struct wl_array
{
    public size_t size;
    public size_t alloc;
    public void*  data;
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class XdgShellClientProtocol
{
    private const uint XDG_SURFACE_SET_WINDOW_GEOMETRY = 3;
    private const uint XDG_WM_BASE_DESTROY             = 0;
    private const uint XDG_WM_BASE_GET_XDG_SURFACE     = 2;

    private static readonly wl_interface** xdg_shell_types;

    private readonly static wl_message* xdg_popup_events;
    private readonly static wl_message* xdg_popup_requests;
    private readonly static wl_message* xdg_positioner_requests;
    private readonly static wl_message* xdg_surface_events;
    private readonly static wl_message* xdg_surface_requests;
    private readonly static wl_message* xdg_toplevel_events;
    private readonly static wl_message* xdg_toplevel_requests;
    private readonly static wl_message* xdg_wm_base_events;
    private readonly static wl_message* xdg_wm_base_requests;

    public readonly static wl_interface* xdg_popup_interface;
    public readonly static wl_interface* xdg_positioner_interface;
    public readonly static wl_interface* xdg_surface_interface;
    public readonly static wl_interface* xdg_toplevel_interface;
    public readonly static wl_interface* xdg_wm_base_interface;

    static XdgShellClientProtocol()
    {
        const int TYPES_COUNT = 25;

        xdg_shell_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        xdg_wm_base_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),           Ustr(""),   xdg_shell_types + 0),
            new(Ustr("create_positioner"), Ustr("n"),  xdg_shell_types + 4),
            new(Ustr("get_xdg_surface"),   Ustr("no"), xdg_shell_types + 5),
            new(Ustr("pong"),              Ustr("u"),  xdg_shell_types + 5),
        ]);

        xdg_wm_base_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("ping"), Ustr("u"), xdg_shell_types + 0),
        ]);

        xdg_wm_base_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_wm_base"), 7,
                4, xdg_wm_base_requests,
                1, xdg_wm_base_events
            )
        );

        xdg_positioner_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),                   Ustr(""),     xdg_shell_types + 0),
            new(Ustr("set_size"),                  Ustr("ii"),   xdg_shell_types + 0),
            new(Ustr("set_anchor_rect"),           Ustr("iiii"), xdg_shell_types + 0),
            new(Ustr("set_anchor"),                Ustr("u"),    xdg_shell_types + 0),
            new(Ustr("set_gravity"),               Ustr("u"),    xdg_shell_types + 0),
            new(Ustr("set_constraint_adjustment"), Ustr("u"),    xdg_shell_types + 0),
            new(Ustr("set_offset"),                Ustr("ii"),   xdg_shell_types + 0),
            new(Ustr("set_reactive"),              Ustr("3"),    xdg_shell_types + 0),
            new(Ustr("set_parent_size"),           Ustr("3ii"),  xdg_shell_types + 0),
            new(Ustr("set_parent_configure"),      Ustr("3u"),   xdg_shell_types + 0),
        ]);

        xdg_positioner_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_positioner"), 1,
                10, xdg_positioner_requests,
                0, null
            )
        );

        xdg_surface_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),             Ustr(""),     xdg_shell_types + 0),
            new(Ustr("get_toplevel"),        Ustr("n"),    xdg_shell_types + 7),
            new(Ustr("get_popup"),           Ustr("n?oo"), xdg_shell_types + 8),
            new(Ustr("set_window_geometry"), Ustr("iiii"), xdg_shell_types + 0),
            new(Ustr("ack_configure"),       Ustr("u"),    xdg_shell_types + 0),
        ]);

        xdg_surface_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("configure"), Ustr("u"), xdg_shell_types + 0)
        ]);

        xdg_surface_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_surface"), 7,
                5, xdg_surface_requests,
                1, xdg_surface_events
            )
        );

        xdg_toplevel_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),          Ustr(""),      xdg_shell_types + 0),
            new(Ustr("set_parent"),       Ustr("?)o"),   xdg_shell_types + 11),
            new(Ustr("set_title"),        Ustr("s)"),    xdg_shell_types + 0),
            new(Ustr("set_app_id"),       Ustr("s)"),    xdg_shell_types + 0),
            new(Ustr("show_window_menu"), Ustr("o)uii"), xdg_shell_types + 12),
            new(Ustr("move"),             Ustr("o)u"),   xdg_shell_types + 16),
            new(Ustr("resize"),           Ustr("o)uu"),  xdg_shell_types + 18),
            new(Ustr("set_max_size"),     Ustr("i)i"),   xdg_shell_types + 0),
            new(Ustr("set_min_size"),     Ustr("i)i"),   xdg_shell_types + 0),
            new(Ustr("set_maximized"),    Ustr(""),      xdg_shell_types + 0),
            new(Ustr("unset_maximized"),  Ustr(""),      xdg_shell_types + 0),
            new(Ustr("set_fullscreen"),   Ustr("?)o"),   xdg_shell_types + 21),
            new(Ustr("unset_fullscreen"), Ustr(""),      xdg_shell_types + 0),
            new(Ustr("set_minimized"),    Ustr(""),      xdg_shell_types + 0),
        ]);

        xdg_toplevel_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("configure"),        Ustr("iia"), xdg_shell_types + 0),
            new(Ustr("close"),            Ustr(""),    xdg_shell_types + 0),
            new(Ustr("configure_bounds"), Ustr("4ii"), xdg_shell_types + 0),
            new(Ustr("wm_capabilities"),  Ustr("5a"),  xdg_shell_types + 0),
        ]);

        xdg_toplevel_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_toplevel"), 7,
                14, xdg_toplevel_requests,
                4, xdg_toplevel_events
            )
        );

        xdg_popup_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), xdg_shell_types + 0)
        ]);

        xdg_popup_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("popup_done"), Ustr(""), xdg_shell_types + 0),
        ]);

        xdg_popup_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_popup"), 1,
                3, xdg_popup_requests,
                3, xdg_popup_events
            )
        );

        xdg_shell_types[4]  = xdg_positioner_interface;
        xdg_shell_types[5]  = xdg_surface_interface;
        xdg_shell_types[6]  = wl_surface_interface;
        xdg_shell_types[7]  = xdg_toplevel_interface;
        xdg_shell_types[8]  = xdg_popup_interface;
        xdg_shell_types[9]  = xdg_surface_interface;
        xdg_shell_types[10] = xdg_positioner_interface;
        xdg_shell_types[11] = xdg_toplevel_interface;
        xdg_shell_types[12] = wl_seat_interface;

        xdg_shell_types[16] = wl_seat_interface;

        xdg_shell_types[18] = wl_seat_interface;

        xdg_shell_types[21] = wl_output_interface;
        xdg_shell_types[22] = wl_seat_interface;

        xdg_shell_types[24] = xdg_positioner_interface;
    }

    #region wl_proxy - xdg_surface
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int xdg_surface_add_listener(xdg_surface* proxy, xdg_surface_listener* implementation, void* data) =>
        wl_proxy_add_listener((wl_proxy*)proxy, (void**)implementation, data);

    public static xdg_surface* xdg_wm_base_get_xdg_surface(xdg_wm_base* xdg_wm_base, wl_surface* surface) =>
        (xdg_surface*)wl_proxy_marshal_flags(
            (wl_proxy*)xdg_wm_base,
            XDG_WM_BASE_GET_XDG_SURFACE,
            xdg_surface_interface,
            wl_proxy_get_version((wl_proxy*)xdg_wm_base),
            0,
            [default, surface]
        );

    public static void xdg_surface_set_window_geometry(xdg_surface* xdg_surface, int32_t x, int32_t y, int32_t width, int32_t height) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)xdg_surface,
            XDG_SURFACE_SET_WINDOW_GEOMETRY,
            null,
            wl_proxy_get_version((wl_proxy*)xdg_surface),
            0,
            [x, y, width, height]
        );

    #endregion

    #region wl_proxy - xdg_toplevel
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int xdg_toplevel_add_listener(xdg_toplevel* xdg_toplevel, xdg_toplevel_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)xdg_toplevel, (void**)listener, data);

    #endregion

    #region wl_proxy - xdg_wm_base
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int xdg_wm_base_add_listener(xdg_wm_base* xdg_wm_base, xdg_wm_base_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)xdg_wm_base, (void**)listener, data);

    public static void xdg_wm_base_destroy(xdg_wm_base* xdg_wm_base) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)xdg_wm_base,
            XDG_WM_BASE_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)xdg_wm_base),
            WL_MARSHAL_FLAG_DESTROY
        );
    #endregion
}
