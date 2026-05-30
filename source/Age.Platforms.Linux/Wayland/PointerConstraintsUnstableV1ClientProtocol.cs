using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct zwp_confined_pointer_v1;
internal struct zwp_locked_pointer_v1;
internal struct zwp_pointer_constraints_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class PointerConstraintsUnstableV1ClientProtocol
{
    private const uint ZWP_CONFINED_POINTER_V1_DESTROY    = 0;
    private const uint ZWP_CONFINED_POINTER_V1_SET_REGION = 1;

    private const uint ZWP_POINTER_CONSTRAINTS_V1_DESTROY = 0;

    private const uint  ZWP_LOCKED_POINTER_V1_DESTROY                  = 0;
    private const uint  ZWP_LOCKED_POINTER_V1_SET_CURSOR_POSITION_HINT = 1;
    private const uint  ZWP_LOCKED_POINTER_V1_SET_REGION               = 2;

    private static readonly wl_interface** pointer_constraints_unstable_v1_types;

    private readonly static wl_message* zwp_pointer_constraints_v1_requests;
    private readonly static wl_message* zwp_locked_pointer_v1_requests;
    private readonly static wl_message* zwp_locked_pointer_v1_events;
    private readonly static wl_message* zwp_confined_pointer_v1_requests;
    private readonly static wl_message* zwp_confined_pointer_v1_events;

    public readonly static wl_interface* zwp_pointer_constraints_v1_interface;
    public readonly static wl_interface* zwp_locked_pointer_v1_interface;
    public readonly static wl_interface* zwp_confined_pointer_v1_interface;

    static PointerConstraintsUnstableV1ClientProtocol()
    {
        const int TYPES_COUNT = 14;

        pointer_constraints_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_pointer_constraints_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),         Ustr(""),       pointer_constraints_unstable_v1_types + 0),
            new(Ustr("lock_pointer"),    Ustr("noo?ou"), pointer_constraints_unstable_v1_types + 2),
            new(Ustr("confine_pointer"), Ustr("noo?ou"), pointer_constraints_unstable_v1_types + 7),
        ]);

        zwp_pointer_constraints_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_pointer_constraints_v1"), 1,
                3, zwp_pointer_constraints_v1_requests,
                0, null
            )
        );

        zwp_locked_pointer_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),                  Ustr(""),   pointer_constraints_unstable_v1_types + 0),
            new(Ustr("set_cursor_position_hint"), Ustr("ff"), pointer_constraints_unstable_v1_types + 0),
            new(Ustr("set_region"),               Ustr("?o"), pointer_constraints_unstable_v1_types + 12),
        ]);

        zwp_locked_pointer_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("locked"),   Ustr(""), pointer_constraints_unstable_v1_types + 0),
            new(Ustr("unlocked"), Ustr(""), pointer_constraints_unstable_v1_types + 0),
        ]);

        zwp_locked_pointer_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_locked_pointer_v1"), 1,
                3, zwp_locked_pointer_v1_requests,
                2, zwp_locked_pointer_v1_events
            )
        );

        zwp_confined_pointer_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),    Ustr(""),   pointer_constraints_unstable_v1_types + 0),
            new(Ustr("set_region"), Ustr("?o"), pointer_constraints_unstable_v1_types + 13),
        ]);

        zwp_confined_pointer_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("confined"),   Ustr(""), pointer_constraints_unstable_v1_types + 0),
            new(Ustr("unconfined"), Ustr(""), pointer_constraints_unstable_v1_types + 0),
        ]);

        zwp_confined_pointer_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_confined_pointer_v1"), 1,
                2, zwp_confined_pointer_v1_requests,
                2, zwp_confined_pointer_v1_events
            )
        );

        pointer_constraints_unstable_v1_types[2] = zwp_locked_pointer_v1_interface;
        pointer_constraints_unstable_v1_types[3] = wl_surface_interface;
        pointer_constraints_unstable_v1_types[4] = wl_pointer_interface;
        pointer_constraints_unstable_v1_types[5] = wl_region_interface;

        pointer_constraints_unstable_v1_types[7] = zwp_confined_pointer_v1_interface;
        pointer_constraints_unstable_v1_types[8] = wl_surface_interface;
        pointer_constraints_unstable_v1_types[9] = wl_pointer_interface;
        pointer_constraints_unstable_v1_types[10] = wl_region_interface;

        pointer_constraints_unstable_v1_types[12] = wl_region_interface;
        pointer_constraints_unstable_v1_types[13] = wl_region_interface;
    }

    public static void zwp_pointer_constraints_v1_destroy(zwp_pointer_constraints_v1* zwp_pointer_constraints_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)zwp_pointer_constraints_v1,
            ZWP_POINTER_CONSTRAINTS_V1_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)zwp_pointer_constraints_v1),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static void zwp_confined_pointer_v1_destroy(zwp_confined_pointer_v1* zwp_confined_pointer_v1) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)zwp_confined_pointer_v1,
            ZWP_CONFINED_POINTER_V1_DESTROY,
            default,
            wl_proxy_get_version((wl_proxy*)zwp_confined_pointer_v1),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static void zwp_locked_pointer_v1_destroy(zwp_locked_pointer_v1* zwp_locked_pointer_v1) =>
    wl_proxy_marshal_flags(
        (wl_proxy*)zwp_locked_pointer_v1,
        ZWP_LOCKED_POINTER_V1_DESTROY,
        default,
        wl_proxy_get_version((wl_proxy*)zwp_locked_pointer_v1),
        WL_MARSHAL_FLAG_DESTROY
    );
}
