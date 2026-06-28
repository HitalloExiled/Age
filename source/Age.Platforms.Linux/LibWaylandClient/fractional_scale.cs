using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.LibWaylandClient.Helper;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct wp_fractional_scale_manager_v1;
internal struct wp_fractional_scale_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class fractional_scale
{
    private const uint WP_FRACTIONAL_SCALE_MANAGER_V1_DESTROY              = 0;
    private const uint WP_FRACTIONAL_SCALE_MANAGER_V1_GET_FRACTIONAL_SCALE = 1;

    private const uint WP_FRACTIONAL_SCALE_V1_DESTROY = 0;

    private static readonly wl_interface** fractional_scale_v1_types;

    private readonly static wl_message* wp_fractional_scale_manager_v1_requests;
    private readonly static wl_message* wp_fractional_scale_v1_requests;
    private readonly static wl_message* wp_fractional_scale_v1_events;

    public readonly static wl_interface* wp_fractional_scale_manager_v1_interface;
    public readonly static wl_interface* wp_fractional_scale_v1_interface;

    static fractional_scale()
    {
        const int TYPES_COUNT = 3;

        fractional_scale_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_fractional_scale_manager_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),              Ustr(""),   fractional_scale_v1_types + 0),
            new(Ustr("get_fractional_scale"), Ustr("no"), fractional_scale_v1_types + 1),
        ]);

        wp_fractional_scale_manager_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_fractional_scale_manager_v1"), 1,
                2, wp_fractional_scale_manager_v1_requests,
                0, null
            )
        );

        wp_fractional_scale_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), fractional_scale_v1_types + 0),
        ]);

        wp_fractional_scale_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("preferred_scale"), Ustr("u"), fractional_scale_v1_types + 0),
        ]);

        wp_fractional_scale_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_fractional_scale_v1"), 1,
                1, wp_fractional_scale_v1_requests,
                1, wp_fractional_scale_v1_events
            )
        );

        fractional_scale_v1_types[1] = wp_fractional_scale_v1_interface;
        fractional_scale_v1_types[2] = lib_wayland_client.wl_surface_interface;
    }

    public static int wp_fractional_scale_v1_add_listener(wp_fractional_scale_v1* wp_fractional_scale_v1, wp_fractional_scale_v1_listener* listener, void* data) =>
        lib_wayland_client.wl_proxy_add_listener((wl_proxy*)wp_fractional_scale_v1, (void**)listener, data);

    public static void wp_fractional_scale_v1_destroy(wp_fractional_scale_v1* wp_fractional_scale_v1) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_fractional_scale_v1,
            WP_FRACTIONAL_SCALE_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_fractional_scale_v1),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    public static void wp_fractional_scale_manager_v1_destroy(wp_fractional_scale_manager_v1* wp_fractional_scale_manager_v1) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_fractional_scale_manager_v1,
            WP_FRACTIONAL_SCALE_MANAGER_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_fractional_scale_manager_v1),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    public static wp_fractional_scale_v1* wp_fractional_scale_manager_v1_get_fractional_scale(wp_fractional_scale_manager_v1* wp_fractional_scale_manager_v1, wl_surface* surface) =>
        (wp_fractional_scale_v1*)lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_fractional_scale_manager_v1,
            WP_FRACTIONAL_SCALE_MANAGER_V1_GET_FRACTIONAL_SCALE,
            wp_fractional_scale_v1_interface,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_fractional_scale_manager_v1),
            0,
            [default, surface]
        );
}
