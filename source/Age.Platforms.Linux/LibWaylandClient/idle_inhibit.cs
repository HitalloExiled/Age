using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.LibWaylandClient.Helper;
using static Age.Platforms.Linux.LibWaylandClient.lib_wayland_client;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct zwp_idle_inhibit_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class idle_inhibit
{
    private const uint ZWP_IDLE_INHIBIT_MANAGER_V1_DESTROY = 0;

    private static readonly wl_interface** idle_inhibit_unstable_v1_types;

    private readonly static wl_message* zwp_idle_inhibit_manager_v1_requests;
    private readonly static wl_message* zwp_idle_inhibitor_v1_requests;

    public readonly static wl_interface* zwp_idle_inhibit_manager_v1_interface;
    public readonly static wl_interface* zwp_idle_inhibitor_v1_interface;

    static idle_inhibit()
    {
        const int TYPES_COUNT = 2;

        idle_inhibit_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_idle_inhibit_manager_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),          Ustr(""),   idle_inhibit_unstable_v1_types + 0),
            new(Ustr("create_inhibitor"), Ustr("no"), idle_inhibit_unstable_v1_types + 0),
        ]);

        zwp_idle_inhibit_manager_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_idle_inhibit_manager_v1"), 1,
                2, zwp_idle_inhibit_manager_v1_requests,
                0, null
            )
        );

        zwp_idle_inhibitor_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), idle_inhibit_unstable_v1_types + 0),
        ]);

        zwp_idle_inhibitor_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_idle_inhibitor_v1"), 1,
                1, zwp_idle_inhibitor_v1_requests,
                0, null
            )
        );

        idle_inhibit_unstable_v1_types[0] = zwp_idle_inhibitor_v1_interface;
        idle_inhibit_unstable_v1_types[1] = wl_surface_interface;
    }

    public static void zwp_idle_inhibit_manager_v1_destroy(zwp_idle_inhibit_manager_v1* zwp_idle_inhibit_manager_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)zwp_idle_inhibit_manager_v1,
            ZWP_IDLE_INHIBIT_MANAGER_V1_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)zwp_idle_inhibit_manager_v1),
            WL_MARSHAL_FLAG_DESTROY
        );
}
