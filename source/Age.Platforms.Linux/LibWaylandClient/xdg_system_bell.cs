using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.LibWaylandClient.Helper;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct xdg_system_bell_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class xdg_system_bell
{
    private const uint XDG_SYSTEM_BELL_V1_DESTROY = 0;

    private static readonly wl_interface** xdg_system_bell_v1_types;

    private readonly static wl_message* xdg_system_bell_v1_requests;

    public readonly static wl_interface* xdg_system_bell_v1_interface;

    static xdg_system_bell()
    {
        const int TYPES_COUNT = 1;

        xdg_system_bell_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        xdg_system_bell_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""),   xdg_system_bell_v1_types + 0),
            new(Ustr("ring"),    Ustr("?o"), xdg_system_bell_v1_types + 0),
        ]);

        xdg_system_bell_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("xdg_system_bell_v1"), 1,
                2, xdg_system_bell_v1_requests,
                0, null
            )
        );

        xdg_system_bell_v1_types[0] = lib_wayland_client.wl_surface_interface;
    }

    public static void xdg_system_bell_v1_destroy(xdg_system_bell_v1* xdg_system_bell_v1) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)xdg_system_bell_v1,
            XDG_SYSTEM_BELL_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)xdg_system_bell_v1),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );
}
