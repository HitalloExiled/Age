using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;
using static Age.Platforms.Linux.Wayland.XdgShellClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct zxdg_decoration_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class XdgDecorationUnstableV1ClientProtocol
{
    private const uint ZXDG_DECORATION_MANAGER_V1_DESTROY = 0;

    private static readonly wl_interface** xdg_decoration_unstable_v1_types;

    private readonly static wl_message* zxdg_decoration_manager_v1_requests;
    private readonly static wl_message* zxdg_toplevel_decoration_v1_requests;
    private readonly static wl_message* zxdg_toplevel_decoration_v1_events;

    public readonly static wl_interface* zxdg_decoration_manager_v1_interface;
    public readonly static wl_interface* zxdg_toplevel_decoration_v1_interface;

    static XdgDecorationUnstableV1ClientProtocol()
    {
        const int TYPES_COUNT = 3;

        xdg_decoration_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zxdg_decoration_manager_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),                  Ustr(""),   xdg_decoration_unstable_v1_types + 0),
            new(Ustr("get_toplevel_decoration"),  Ustr("no"), xdg_decoration_unstable_v1_types + 1),
        ]);

        zxdg_decoration_manager_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zxdg_decoration_manager_v1"), 1,
                2, zxdg_decoration_manager_v1_requests,
                0, null
            )
        );

        zxdg_toplevel_decoration_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),    Ustr(""),  xdg_decoration_unstable_v1_types + 0),
            new(Ustr("set_mode"),   Ustr("u"), xdg_decoration_unstable_v1_types + 0),
            new(Ustr("unset_mode"), Ustr(""),  xdg_decoration_unstable_v1_types + 0),
        ]);

        zxdg_toplevel_decoration_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("configure"), Ustr("u"), xdg_decoration_unstable_v1_types + 0),
        ]);

        zxdg_toplevel_decoration_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zxdg_toplevel_decoration_v1"), 1,
                3, zxdg_toplevel_decoration_v1_requests,
                1, zxdg_toplevel_decoration_v1_events
            )
        );

        xdg_decoration_unstable_v1_types[1] = zxdg_toplevel_decoration_v1_interface;
        xdg_decoration_unstable_v1_types[2] = xdg_toplevel_interface;
    }

    public static void zxdg_decoration_manager_v1_destroy(zxdg_decoration_manager_v1* zxdg_decoration_manager_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)zxdg_decoration_manager_v1,
            ZXDG_DECORATION_MANAGER_V1_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)zxdg_decoration_manager_v1),
            WL_MARSHAL_FLAG_DESTROY
        );
}
