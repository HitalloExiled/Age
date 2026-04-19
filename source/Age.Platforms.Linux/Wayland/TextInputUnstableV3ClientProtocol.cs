using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal unsafe static class TextInputUnstableV3ClientProtocol
{
    private const uint ZWP_TEXT_INPUT_MANAGER_V3_GET_TEXT_INPUT = 1;

    private static readonly wl_interface** text_input_unstable_v3_types;

    private readonly static wl_message* zwp_text_input_v3_requests;
    private readonly static wl_message* zwp_text_input_v3_events;
    private readonly static wl_message* zwp_text_input_manager_v3_requests;

    public readonly static wl_interface* zwp_text_input_v3_interface;
    public readonly static wl_interface* zwp_text_input_manager_v3_interface;

    static TextInputUnstableV3ClientProtocol()
    {
        const int TYPES_COUNT = 8;

        text_input_unstable_v3_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_text_input_v3_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),                Ustr(""),     text_input_unstable_v3_types + 0),
            new(Ustr("enable"),                 Ustr(""),     text_input_unstable_v3_types + 0),
            new(Ustr("disable"),                Ustr(""),     text_input_unstable_v3_types + 0),
            new(Ustr("set_surrounding_text"),   Ustr("sii"),  text_input_unstable_v3_types + 0),
            new(Ustr("set_text_change_cause"),  Ustr("u"),    text_input_unstable_v3_types + 0),
            new(Ustr("set_content_type"),       Ustr("uu"),   text_input_unstable_v3_types + 0),
            new(Ustr("set_cursor_rectangle"),   Ustr("iiii"), text_input_unstable_v3_types + 0),
            new(Ustr("commit"),                 Ustr(""),     text_input_unstable_v3_types + 0),
        ]);

        zwp_text_input_v3_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("enter"),                    Ustr("o"),    text_input_unstable_v3_types + 4),
            new(Ustr("leave"),                    Ustr("o"),    text_input_unstable_v3_types + 5),
            new(Ustr("preedit_string"),           Ustr("?sii"), text_input_unstable_v3_types + 0),
            new(Ustr("commit_string"),            Ustr("?s"),   text_input_unstable_v3_types + 0),
            new(Ustr("delete_surrounding_text"),  Ustr("uu"),   text_input_unstable_v3_types + 0),
            new(Ustr("done"),                     Ustr("u"),    text_input_unstable_v3_types + 0),
        ]);

        zwp_text_input_v3_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_text_input_v3"), 1,
                8, zwp_text_input_v3_requests,
                6, zwp_text_input_v3_events
            )
        );

        zwp_text_input_manager_v3_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),        Ustr(""),   text_input_unstable_v3_types + 0),
            new(Ustr("get_text_input"), Ustr("no"), text_input_unstable_v3_types + 6),
        ]);

        zwp_text_input_manager_v3_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_text_input_manager_v3"), 1,
                2, zwp_text_input_manager_v3_requests,
                0, null
            )
        );

        text_input_unstable_v3_types[4] = wl_surface_interface;
        text_input_unstable_v3_types[5] = wl_surface_interface;
        text_input_unstable_v3_types[6] = zwp_text_input_v3_interface;
        text_input_unstable_v3_types[7] = wl_seat_interface;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static zwp_text_input_v3* zwp_text_input_manager_v3_get_text_input(zwp_text_input_manager_v3* zwp_text_input_manager_v3, wl_seat* seat) =>
        (zwp_text_input_v3*)wl_proxy_marshal_flags(
            (wl_proxy*)zwp_text_input_manager_v3,
            ZWP_TEXT_INPUT_MANAGER_V3_GET_TEXT_INPUT,
            zwp_text_input_v3_interface,
            wl_proxy_get_version((wl_proxy*)zwp_text_input_manager_v3),
            0,
            null,
            seat
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int zwp_text_input_v3_add_listener(zwp_text_input_v3* zwp_text_input_v3, zwp_text_input_v3_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)zwp_text_input_v3, (void**)listener, data);
}
