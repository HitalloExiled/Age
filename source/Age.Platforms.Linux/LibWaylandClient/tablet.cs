using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.LibWaylandClient.Helper;
using static Age.Platforms.Linux.LibWaylandClient.lib_wayland_client;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct zwp_tablet_manager_v2;
internal struct zwp_tablet_pad_v2;
internal struct zwp_tablet_seat_v2;
internal struct zwp_tablet_tool_v2;
internal struct zwp_tablet_v2;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class tablet
{
    private const uint ZWP_TABLET_MANAGER_V2_GET_TABLET_SEAT = 0;

    private static readonly wl_interface** tablet_unstable_v2_types;

    private readonly static wl_message* zwp_tablet_manager_v2_requests;
    private readonly static wl_message* zwp_tablet_seat_v2_requests;
    private readonly static wl_message* zwp_tablet_seat_v2_events;
    private readonly static wl_message* zwp_tablet_tool_v2_requests;
    private readonly static wl_message* zwp_tablet_tool_v2_events;
    private readonly static wl_message* zwp_tablet_v2_requests;
    private readonly static wl_message* zwp_tablet_v2_events;
    private readonly static wl_message* zwp_tablet_pad_ring_v2_requests;
    private readonly static wl_message* zwp_tablet_pad_ring_v2_events;
    private readonly static wl_message* zwp_tablet_pad_strip_v2_requests;
    private readonly static wl_message* zwp_tablet_pad_strip_v2_events;
    private readonly static wl_message* zwp_tablet_pad_group_v2_requests;
    private readonly static wl_message* zwp_tablet_pad_group_v2_events;
    private readonly static wl_message* zwp_tablet_pad_v2_requests;
    private readonly static wl_message* zwp_tablet_pad_v2_events;

    public readonly static wl_interface* zwp_tablet_manager_v2_interface;
    public readonly static wl_interface* zwp_tablet_seat_v2_interface;
    public readonly static wl_interface* zwp_tablet_tool_v2_interface;
    public readonly static wl_interface* zwp_tablet_v2_interface;
    public readonly static wl_interface* zwp_tablet_pad_ring_v2_interface;
    public readonly static wl_interface* zwp_tablet_pad_strip_v2_interface;
    public readonly static wl_interface* zwp_tablet_pad_group_v2_interface;
    public readonly static wl_interface* zwp_tablet_pad_v2_interface;

    static tablet()
    {
        const int TYPES_COUNT = 23;

        tablet_unstable_v2_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_tablet_manager_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("get_tablet_seat"), Ustr("no"), tablet_unstable_v2_types + 3),
            new(Ustr("destroy"),         Ustr(""),   tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_manager_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_manager_v2"), 1,
                2, zwp_tablet_manager_v2_requests,
                0, null
            )
        );

        zwp_tablet_seat_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_seat_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("tablet_added"), Ustr("n"), tablet_unstable_v2_types + 5),
            new(Ustr("tool_added"),   Ustr("n"), tablet_unstable_v2_types + 6),
            new(Ustr("pad_added"),    Ustr("n"), tablet_unstable_v2_types + 7),
        ]);

        zwp_tablet_seat_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_seat_v2"), 1,
                1, zwp_tablet_seat_v2_requests,
                3, zwp_tablet_seat_v2_events
            )
        );

        zwp_tablet_tool_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("set_cursor"), Ustr("u?oii"), tablet_unstable_v2_types + 8),
            new(Ustr("destroy"),    Ustr(""),      tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_tool_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("type"),              Ustr("u"),    tablet_unstable_v2_types + 0),
            new(Ustr("hardware_serial"),   Ustr("uu"),   tablet_unstable_v2_types + 0),
            new(Ustr("hardware_id_wacom"), Ustr("uu"),   tablet_unstable_v2_types + 0),
            new(Ustr("capability"),        Ustr("u"),    tablet_unstable_v2_types + 0),
            new(Ustr("done"),              Ustr(""),     tablet_unstable_v2_types + 0),
            new(Ustr("removed"),           Ustr(""),     tablet_unstable_v2_types + 0),
            new(Ustr("proximity_in"),      Ustr("uoo"),  tablet_unstable_v2_types + 12),
            new(Ustr("proximity_out"),     Ustr(""),     tablet_unstable_v2_types + 0),
            new(Ustr("down"),              Ustr("u"),    tablet_unstable_v2_types + 0),
            new(Ustr("up"),                Ustr(""),     tablet_unstable_v2_types + 0),
            new(Ustr("motion"),            Ustr("ff"),   tablet_unstable_v2_types + 0),
            new(Ustr("pressure"),          Ustr("u"),    tablet_unstable_v2_types + 0),
            new(Ustr("distance"),          Ustr("u"),    tablet_unstable_v2_types + 0),
            new(Ustr("tilt"),              Ustr("ff"),   tablet_unstable_v2_types + 0),
            new(Ustr("rotation"),          Ustr("f"),    tablet_unstable_v2_types + 0),
            new(Ustr("slider"),            Ustr("i"),    tablet_unstable_v2_types + 0),
            new(Ustr("wheel"),             Ustr("fi"),   tablet_unstable_v2_types + 0),
            new(Ustr("button"),            Ustr("uuu"),  tablet_unstable_v2_types + 0),
            new(Ustr("frame"),             Ustr("u"),    tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_tool_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_tool_v2"), 1,
                2, zwp_tablet_tool_v2_requests,
                19, zwp_tablet_tool_v2_events
            )
        );

        zwp_tablet_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("name"),    Ustr("s"),  tablet_unstable_v2_types + 0),
            new(Ustr("id"),      Ustr("uu"), tablet_unstable_v2_types + 0),
            new(Ustr("path"),    Ustr("s"),  tablet_unstable_v2_types + 0),
            new(Ustr("done"),    Ustr(""),   tablet_unstable_v2_types + 0),
            new(Ustr("removed"), Ustr(""),   tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_v2"), 1,
                1, zwp_tablet_v2_requests,
                5, zwp_tablet_v2_events
            )
        );

        zwp_tablet_pad_ring_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("set_feedback"), Ustr("su"), tablet_unstable_v2_types + 0),
            new(Ustr("destroy"),      Ustr(""),   tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_ring_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("source"), Ustr("u"), tablet_unstable_v2_types + 0),
            new(Ustr("angle"),  Ustr("f"), tablet_unstable_v2_types + 0),
            new(Ustr("stop"),   Ustr(""),  tablet_unstable_v2_types + 0),
            new(Ustr("frame"),  Ustr("u"), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_ring_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_pad_ring_v2"), 1,
                2, zwp_tablet_pad_ring_v2_requests,
                4, zwp_tablet_pad_ring_v2_events
            )
        );

        zwp_tablet_pad_strip_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("set_feedback"), Ustr("su"), tablet_unstable_v2_types + 0),
            new(Ustr("destroy"),      Ustr(""),   tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_strip_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("source"),   Ustr("u"), tablet_unstable_v2_types + 0),
            new(Ustr("position"), Ustr("u"), tablet_unstable_v2_types + 0),
            new(Ustr("stop"),     Ustr(""),  tablet_unstable_v2_types + 0),
            new(Ustr("frame"),    Ustr("u"), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_strip_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_pad_strip_v2"), 1,
                2, zwp_tablet_pad_strip_v2_requests,
                4, zwp_tablet_pad_strip_v2_events
            )
        );

        zwp_tablet_pad_group_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"), Ustr(""), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_group_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("buttons"),     Ustr("a"),   tablet_unstable_v2_types + 0),
            new(Ustr("ring"),        Ustr("n"),   tablet_unstable_v2_types + 15),
            new(Ustr("strip"),       Ustr("n"),   tablet_unstable_v2_types + 16),
            new(Ustr("modes"),       Ustr("u"),   tablet_unstable_v2_types + 0),
            new(Ustr("done"),        Ustr(""),    tablet_unstable_v2_types + 0),
            new(Ustr("mode_switch"), Ustr("uuu"), tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_group_v2_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_tablet_pad_group_v2"), 1,
                1, zwp_tablet_pad_group_v2_requests,
                6, zwp_tablet_pad_group_v2_events
            )
        );

        zwp_tablet_pad_v2_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("set_feedback"), Ustr("usu"), tablet_unstable_v2_types + 0),
            new(Ustr("destroy"),      Ustr(""),    tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_v2_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("group"),   Ustr("n"),   tablet_unstable_v2_types + 17),
            new(Ustr("path"),    Ustr("s"),   tablet_unstable_v2_types + 0),
            new(Ustr("buttons"), Ustr("u"),   tablet_unstable_v2_types + 0),
            new(Ustr("done"),    Ustr(""),    tablet_unstable_v2_types + 0),
            new(Ustr("button"),  Ustr("uuu"), tablet_unstable_v2_types + 0),
            new(Ustr("enter"),   Ustr("uoo"), tablet_unstable_v2_types + 18),
            new(Ustr("leave"),   Ustr("uo"),  tablet_unstable_v2_types + 21),
            new(Ustr("removed"), Ustr(""),    tablet_unstable_v2_types + 0),
        ]);

        zwp_tablet_pad_v2_interface = NativeMemory.Alloc(
            new wl_interface(Ustr("zwp_tablet_pad_v2"), 1,
                2, zwp_tablet_pad_v2_requests,
                8, zwp_tablet_pad_v2_events
            )
        );

        tablet_unstable_v2_types[3] = zwp_tablet_seat_v2_interface;
        tablet_unstable_v2_types[4] = wl_seat_interface;
        tablet_unstable_v2_types[5] = zwp_tablet_v2_interface;
        tablet_unstable_v2_types[6] = zwp_tablet_tool_v2_interface;
        tablet_unstable_v2_types[7] = zwp_tablet_pad_v2_interface;

        tablet_unstable_v2_types[9] = wl_surface_interface;

        tablet_unstable_v2_types[13] = zwp_tablet_v2_interface;
        tablet_unstable_v2_types[14] = wl_surface_interface;
        tablet_unstable_v2_types[15] = zwp_tablet_pad_ring_v2_interface;
        tablet_unstable_v2_types[16] = zwp_tablet_pad_strip_v2_interface;
        tablet_unstable_v2_types[17] = zwp_tablet_pad_group_v2_interface;

        tablet_unstable_v2_types[19] = zwp_tablet_v2_interface;
        tablet_unstable_v2_types[20] = wl_surface_interface;

        tablet_unstable_v2_types[22] = wl_surface_interface;
    }

    public static zwp_tablet_seat_v2* zwp_tablet_manager_v2_get_tablet_seat(zwp_tablet_manager_v2* zwp_tablet_manager_v2, wl_seat* seat) =>
        (zwp_tablet_seat_v2*)wl_proxy_marshal_flags(
            (wl_proxy*)zwp_tablet_manager_v2,
            ZWP_TABLET_MANAGER_V2_GET_TABLET_SEAT,
            zwp_tablet_seat_v2_interface,
            wl_proxy_get_version((wl_proxy*)
            zwp_tablet_manager_v2),
            0,
            [default, seat]
        );

    public static int zwp_tablet_seat_v2_add_listener(zwp_tablet_seat_v2* zwp_tablet_seat_v2, zwp_tablet_seat_v2_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)zwp_tablet_seat_v2, (void**)listener, data);
}
