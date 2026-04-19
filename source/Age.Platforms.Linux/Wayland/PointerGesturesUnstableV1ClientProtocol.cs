using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct zwp_pointer_gestures_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class PointerGesturesUnstableV1ClientProtocol
{
    private static readonly wl_interface** pointer_gestures_unstable_v1_types;

    private readonly static wl_message* zwp_pointer_gestures_v1_requests;
    private readonly static wl_message* zwp_pointer_gesture_swipe_v1_requests;
    private readonly static wl_message* zwp_pointer_gesture_swipe_v1_events;
    private readonly static wl_message* zwp_pointer_gesture_pinch_v1_requests;
    private readonly static wl_message* zwp_pointer_gesture_pinch_v1_events;
    private readonly static wl_message* zwp_pointer_gesture_hold_v1_requests;
    private readonly static wl_message* zwp_pointer_gesture_hold_v1_events;

    public readonly static wl_interface* zwp_pointer_gestures_v1_interface;
    public readonly static wl_interface* zwp_pointer_gesture_swipe_v1_interface;
    public readonly static wl_interface* zwp_pointer_gesture_pinch_v1_interface;
    public readonly static wl_interface* zwp_pointer_gesture_hold_v1_interface;

    static PointerGesturesUnstableV1ClientProtocol()
    {
        const int TYPES_COUNT = 23;

        pointer_gestures_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_pointer_gestures_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("get_swipe_gesture"), Ustr("no"),  pointer_gestures_unstable_v1_types + 5),
            new(Ustr("get_pinch_gesture"), Ustr("no"),  pointer_gestures_unstable_v1_types + 7),
            new(Ustr("release"),           Ustr("2"),   pointer_gestures_unstable_v1_types + 0),
            new(Ustr("get_hold_gesture"),  Ustr("3no"), pointer_gestures_unstable_v1_types + 9),
        ]);

        zwp_pointer_gestures_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_pointer_gestures_v1"), 3,
                4, zwp_pointer_gestures_v1_requests,
                0, null
            )
        );

        zwp_pointer_gesture_swipe_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"), Ustr(""), pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_swipe_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("begin"),  Ustr("uuou"), pointer_gestures_unstable_v1_types + 11),
            new(Ustr("update"), Ustr("uff"),  pointer_gestures_unstable_v1_types + 0),
            new(Ustr("end"),    Ustr("uui"),  pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_swipe_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_pointer_gesture_swipe_v1"), 2,
                1, zwp_pointer_gesture_swipe_v1_requests,
                3, zwp_pointer_gesture_swipe_v1_events
            )
        );

        zwp_pointer_gesture_pinch_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"), Ustr(""), pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_pinch_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("begin"),  Ustr("uuou"),  pointer_gestures_unstable_v1_types + 15),
            new(Ustr("update"), Ustr("uffff"), pointer_gestures_unstable_v1_types + 0),
            new(Ustr("end"),    Ustr("uui"),   pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_pinch_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_pointer_gesture_pinch_v1"), 2,
                1, zwp_pointer_gesture_pinch_v1_requests,
                3, zwp_pointer_gesture_pinch_v1_events
            )
        );

        zwp_pointer_gesture_hold_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"), Ustr("3"), pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_hold_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("begin"), Ustr("3uuou"), pointer_gestures_unstable_v1_types + 19),
            new(Ustr("end"),   Ustr("3uui"),  pointer_gestures_unstable_v1_types + 0),
        ]);

        zwp_pointer_gesture_hold_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_pointer_gesture_hold_v1"), 3,
                1, zwp_pointer_gesture_hold_v1_requests,
                2, zwp_pointer_gesture_hold_v1_events
            )
        );

        pointer_gestures_unstable_v1_types[5]  = zwp_pointer_gesture_swipe_v1_interface;
        pointer_gestures_unstable_v1_types[6]  = wl_pointer_interface;
        pointer_gestures_unstable_v1_types[7]  = zwp_pointer_gesture_pinch_v1_interface;
        pointer_gestures_unstable_v1_types[8]  = wl_pointer_interface;
        pointer_gestures_unstable_v1_types[9]  = zwp_pointer_gesture_hold_v1_interface;
        pointer_gestures_unstable_v1_types[10] = wl_pointer_interface;

        pointer_gestures_unstable_v1_types[13] = wl_surface_interface;

        pointer_gestures_unstable_v1_types[17] = wl_surface_interface;

        pointer_gestures_unstable_v1_types[21] = wl_surface_interface;
    }

    public static void zwp_pointer_gestures_v1_destroy(zwp_pointer_gestures_v1* zwp_pointer_gestures_v1) =>
        wl_proxy_destroy((wl_proxy*)zwp_pointer_gestures_v1);
}
