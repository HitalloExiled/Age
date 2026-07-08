using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.LibWaylandClient.Helper;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct wp_cursor_shape_device_v1;
internal struct wp_cursor_shape_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class cursor_shape
{
    private const uint WP_CURSOR_SHAPE_MANAGER_V1_DESTROY            =  0;
    private const uint WP_CURSOR_SHAPE_MANAGER_V1_GET_POINTER        =  1;
    private const uint WP_CURSOR_SHAPE_MANAGER_V1_GET_TABLET_TOOL_V2 =  2;

    private const uint WP_CURSOR_SHAPE_DEVICE_V1_DESTROY   = 0;
    private const uint WP_CURSOR_SHAPE_DEVICE_V1_SET_SHAPE = 1;

    private static readonly wl_interface** cursor_shape_v1_types;

    private readonly static wl_message* wp_cursor_shape_manager_v1_requests;
    private readonly static wl_message* wp_cursor_shape_device_v1_requests;

    public readonly static wl_interface* wp_cursor_shape_manager_v1_interface;
    public readonly static wl_interface* wp_cursor_shape_device_v1_interface;

    static cursor_shape()
    {
        const int TYPES_COUNT = 6;

        cursor_shape_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_cursor_shape_manager_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),            Ustr(""),   cursor_shape_v1_types + 0),
            new(Ustr("get_pointer"),        Ustr("no"), cursor_shape_v1_types + 2),
            new(Ustr("get_tablet_tool_v2"), Ustr("no"), cursor_shape_v1_types + 4),
        ]);

        wp_cursor_shape_manager_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_cursor_shape_manager_v1"), 2,
                3, wp_cursor_shape_manager_v1_requests,
                0, null
            )
        );

        wp_cursor_shape_device_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),   Ustr(""),   cursor_shape_v1_types + 0),
            new(Ustr("set_shape"), Ustr("uu"), cursor_shape_v1_types + 0),
        ]);

        wp_cursor_shape_device_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_cursor_shape_device_v1"), 2,
                2, wp_cursor_shape_device_v1_requests,
                0, null
            )
        );

        cursor_shape_v1_types[2] = wp_cursor_shape_device_v1_interface;
        cursor_shape_v1_types[3] = lib_wayland_client.wl_pointer_interface;
        cursor_shape_v1_types[4] = wp_cursor_shape_device_v1_interface;
        cursor_shape_v1_types[5] = tablet.zwp_tablet_tool_v2_interface;
    }

    public static void wp_cursor_shape_device_v1_destroy(wp_cursor_shape_device_v1* wp_cursor_shape_device_v1) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_cursor_shape_device_v1,
            WP_CURSOR_SHAPE_DEVICE_V1_DESTROY,
            default,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_cursor_shape_device_v1),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    public static void wp_cursor_shape_manager_v1_destroy(wp_cursor_shape_manager_v1* wp_cursor_shape_manager_v1) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_cursor_shape_manager_v1,
            WP_CURSOR_SHAPE_MANAGER_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_cursor_shape_manager_v1),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    public static wp_cursor_shape_device_v1* wp_cursor_shape_manager_v1_get_pointer(wp_cursor_shape_manager_v1* wp_cursor_shape_manager_v1, wl_pointer* pointer) =>
        (wp_cursor_shape_device_v1*)lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_cursor_shape_manager_v1,
            WP_CURSOR_SHAPE_MANAGER_V1_GET_POINTER,
            wp_cursor_shape_device_v1_interface,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_cursor_shape_manager_v1),
            0,
            [default, pointer]
        );

    public static void wp_cursor_shape_device_v1_set_shape(wp_cursor_shape_device_v1* wp_cursor_shape_device_v1, uint32_t serial, uint32_t shape) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)wp_cursor_shape_device_v1,
            WP_CURSOR_SHAPE_DEVICE_V1_SET_SHAPE,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)wp_cursor_shape_device_v1),
            0,
            [serial, shape]
        );
}
