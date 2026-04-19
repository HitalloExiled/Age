using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct xdg_activation_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class XdgActivationV1ClientProtocol
{
    private const uint XDG_ACTIVATION_V1_DESTROY = 0;

    private static readonly wl_interface** xdg_activation_v1_types;

    private readonly static wl_message* xdg_activation_v1_requests;
    private readonly static wl_message* xdg_activation_token_v1_requests;
    private readonly static wl_message* xdg_activation_token_v1_events;

    public readonly static wl_interface* xdg_activation_v1_interface;
    public readonly static wl_interface* xdg_activation_token_v1_interface;

    static XdgActivationV1ClientProtocol()
    {
        const int TYPES_COUNT = 7;

        xdg_activation_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        xdg_activation_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),              Ustr(""),   xdg_activation_v1_types + 0),
            new(Ustr("get_activation_token"), Ustr("n"),  xdg_activation_v1_types + 1),
            new(Ustr("activate"),             Ustr("so"), xdg_activation_v1_types + 2),
        ]);

        xdg_activation_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("xdg_activation_v1"), 1,
                3, xdg_activation_v1_requests,
                0, null
            )
        );

        xdg_activation_token_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("set_serial"),  Ustr("uo"), xdg_activation_v1_types + 4),
            new(Ustr("set_app_id"),  Ustr("s"),  xdg_activation_v1_types + 0),
            new(Ustr("set_surface"), Ustr("o"),  xdg_activation_v1_types + 6),
            new(Ustr("commit"),      Ustr(""),   xdg_activation_v1_types + 0),
            new(Ustr("destroy"),     Ustr(""),   xdg_activation_v1_types + 0),
        ]);

        xdg_activation_token_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("done"), Ustr("s"), xdg_activation_v1_types + 0),
        ]);

        xdg_activation_token_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("xdg_activation_token_v1"), 1,
                5, xdg_activation_token_v1_requests,
                1, xdg_activation_token_v1_events
            )
        );

        xdg_activation_v1_types[1] = xdg_activation_token_v1_interface;

        xdg_activation_v1_types[3] = wl_surface_interface;

        xdg_activation_v1_types[5] = wl_seat_interface;
        xdg_activation_v1_types[6] = wl_surface_interface;
    }

    public static void xdg_activation_v1_destroy(xdg_activation_v1* xdg_activation_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)xdg_activation_v1,
            XDG_ACTIVATION_V1_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)xdg_activation_v1),
            WL_MARSHAL_FLAG_DESTROY
        );
}
