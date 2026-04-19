using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.TabletUnstableV2ClientProtocol;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct wp_cursor_shape_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class CursorShapeV1ClientProtocol
{
    private static readonly wl_interface** cursor_shape_v1_types;

    private readonly static wl_message* wp_cursor_shape_manager_v1_requests;
    private readonly static wl_message* wp_cursor_shape_device_v1_requests;

    public readonly static wl_interface* wp_cursor_shape_manager_v1_interface;
    public readonly static wl_interface* wp_cursor_shape_device_v1_interface;

    static CursorShapeV1ClientProtocol()
    {
        const int TYPES_COUNT = 6;

        cursor_shape_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_cursor_shape_manager_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),            Ustr(""),   cursor_shape_v1_types + 0),
            new(Ustr("get_pointer"),        Ustr("no"), cursor_shape_v1_types + 2),
            new(Ustr("get_tablet_tool_v2"), Ustr("no"), cursor_shape_v1_types + 4),
        ]);

        wp_cursor_shape_manager_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_cursor_shape_manager_v1"), 2,
                3, wp_cursor_shape_manager_v1_requests,
                0, null
            )
        );

        wp_cursor_shape_device_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),   Ustr(""),   cursor_shape_v1_types + 0),
            new(Ustr("set_shape"), Ustr("uu"), cursor_shape_v1_types + 0),
        ]);

        wp_cursor_shape_device_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_cursor_shape_device_v1"), 2,
                2, wp_cursor_shape_device_v1_requests,
                0, null
            )
        );

        cursor_shape_v1_types[2] = wp_cursor_shape_device_v1_interface;
        cursor_shape_v1_types[3] = wl_pointer_interface;
        cursor_shape_v1_types[4] = wp_cursor_shape_device_v1_interface;
        cursor_shape_v1_types[5] = zwp_tablet_tool_v2_interface;
    }
}
