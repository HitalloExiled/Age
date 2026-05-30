using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct zwp_relative_pointer_manager_v1;
internal struct zwp_relative_pointer_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class RelativePointerUnstableV1ClientProtocol
{
    private const uint ZWP_RELATIVE_POINTER_MANAGER_V1_DESTROY              = 0;
    private const uint ZWP_RELATIVE_POINTER_MANAGER_V1_GET_RELATIVE_POINTER = 1;

    private const uint ZWP_RELATIVE_POINTER_V1_DESTROY = 0;

    private static readonly wl_interface** relative_pointer_unstable_v1_types;

    private readonly static wl_message* zwp_relative_pointer_manager_v1_requests;
    private readonly static wl_message* zwp_relative_pointer_v1_requests;
    private readonly static wl_message* zwp_relative_pointer_v1_events;

    public readonly static wl_interface* zwp_relative_pointer_manager_v1_interface;
    public readonly static wl_interface* zwp_relative_pointer_v1_interface;

    static RelativePointerUnstableV1ClientProtocol()
    {
        const int TYPES_COUNT = 8;

        relative_pointer_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_relative_pointer_manager_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),               Ustr(""),   relative_pointer_unstable_v1_types + 0),
            new(Ustr("get_relative_pointer"),  Ustr("no"), relative_pointer_unstable_v1_types + 6),
        ]);

        zwp_relative_pointer_manager_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_relative_pointer_manager_v1"), 1,
                2, zwp_relative_pointer_manager_v1_requests,
                0, null
            )
        );

        zwp_relative_pointer_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), relative_pointer_unstable_v1_types + 0),
        ]);

        zwp_relative_pointer_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("relative_motion"), Ustr("uuffff"), relative_pointer_unstable_v1_types + 0),
        ]);

        zwp_relative_pointer_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_relative_pointer_v1"), 1,
                1, zwp_relative_pointer_v1_requests,
                1, zwp_relative_pointer_v1_events
            )
        );

        relative_pointer_unstable_v1_types[6] = zwp_relative_pointer_v1_interface;
        relative_pointer_unstable_v1_types[7] = WaylandClientProtocol.wl_pointer_interface;
    }

    public static void zwp_relative_pointer_manager_v1_destroy(zwp_relative_pointer_manager_v1* zwp_relative_pointer_manager_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)zwp_relative_pointer_manager_v1,
            ZWP_RELATIVE_POINTER_MANAGER_V1_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)zwp_relative_pointer_manager_v1),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static zwp_relative_pointer_v1* zwp_relative_pointer_manager_v1_get_relative_pointer(zwp_relative_pointer_manager_v1* zwp_relative_pointer_manager_v1, wl_pointer* pointer) =>
        (zwp_relative_pointer_v1*)wl_proxy_marshal_flags(
            (wl_proxy*)zwp_relative_pointer_manager_v1,
            ZWP_RELATIVE_POINTER_MANAGER_V1_GET_RELATIVE_POINTER,
            zwp_relative_pointer_v1_interface,
            wl_proxy_get_version((wl_proxy*)zwp_relative_pointer_manager_v1),
            0,
            [default, pointer]
        );

    public static void zwp_relative_pointer_v1_destroy(zwp_relative_pointer_v1* zwp_relative_pointer_v1) =>
    wl_proxy_marshal_flags(
        (wl_proxy*)zwp_relative_pointer_v1,
        ZWP_RELATIVE_POINTER_V1_DESTROY,
        default,
        wl_proxy_get_version((wl_proxy*)zwp_relative_pointer_v1),
        WL_MARSHAL_FLAG_DESTROY
    );

    public static int zwp_relative_pointer_v1_add_listener(zwp_relative_pointer_v1* zwp_relative_pointer_v1, zwp_relative_pointer_v1_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)zwp_relative_pointer_v1, (void**)listener, data);
}
