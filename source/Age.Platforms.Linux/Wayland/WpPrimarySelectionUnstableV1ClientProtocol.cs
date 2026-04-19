using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct zwp_primary_selection_device_manager_v1;
internal struct zwp_primary_selection_device_v1;
internal struct zwp_primary_selection_offer_v1;
internal struct zwp_text_input_manager_v3;
internal struct zwp_text_input_v3;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal unsafe static class WpPrimarySelectionUnstableV1ClientProtocol
{
    private const uint ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_GET_DEVICE = 1;

    private static readonly wl_interface** wp_primary_selection_unstable_v1_types;

    private static readonly wl_message* zwp_primary_selection_device_manager_v1_requests;
    private static readonly wl_message* zwp_primary_selection_device_v1_events;
    private static readonly wl_message* zwp_primary_selection_device_v1_requests;
    private static readonly wl_message* zwp_primary_selection_offer_v1_events;
    private static readonly wl_message* zwp_primary_selection_offer_v1_requests;
    private static readonly wl_message* zwp_primary_selection_source_v1_events;
    private static readonly wl_message* zwp_primary_selection_source_v1_requests;

    public static readonly wl_interface* zwp_primary_selection_device_manager_v1_interface;
    public static readonly wl_interface* zwp_primary_selection_device_v1_interface;
    public static readonly wl_interface* zwp_primary_selection_offer_v1_interface;
    public static readonly wl_interface* zwp_primary_selection_source_v1_interface;

    static WpPrimarySelectionUnstableV1ClientProtocol()
    {
        const uint TYPES_COUNT = 9;

        wp_primary_selection_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_primary_selection_device_manager_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("create_source"), Ustr("n"),  wp_primary_selection_unstable_v1_types + 2),
            new(Ustr("get_device"),    Ustr("no"), wp_primary_selection_unstable_v1_types + 3),
            new(Ustr("destroy"),       Ustr(""),   wp_primary_selection_unstable_v1_types + 0)
        ]);

        zwp_primary_selection_device_manager_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_primary_selection_device_manager_v1"), 1,
                3, zwp_primary_selection_device_manager_v1_requests,
                0, null
            )
        );

        zwp_primary_selection_device_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("set_selection"), Ustr("?ou"), wp_primary_selection_unstable_v1_types + 5),
	        new(Ustr("destroy"),       Ustr(""), wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_device_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("data_offer"), Ustr("n"),  wp_primary_selection_unstable_v1_types + 7),
	        new(Ustr("selection"),  Ustr("?o"), wp_primary_selection_unstable_v1_types + 8),
        ]);

        zwp_primary_selection_device_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_primary_selection_device_v1"), 1,
                2, zwp_primary_selection_device_v1_requests,
                2, zwp_primary_selection_device_v1_events
            )
        );

        zwp_primary_selection_offer_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("receive"), Ustr("sh"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("destroy"), Ustr(""),   wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_offer_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("offer"), Ustr("s"), wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_offer_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_primary_selection_offer_v1"), 1,
                2, zwp_primary_selection_offer_v1_requests,
                1, zwp_primary_selection_offer_v1_events
            )
        );

        zwp_primary_selection_source_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("offer"),   Ustr("s"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("destroy"), Ustr(""),  wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_source_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("send"),      Ustr("sh"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("cancelled"), Ustr(""),   wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_source_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_primary_selection_source_v1"), 1,
                2, zwp_primary_selection_source_v1_requests,
                2, zwp_primary_selection_source_v1_events
            )
        );

        wp_primary_selection_unstable_v1_types[2] = zwp_primary_selection_source_v1_interface;
        wp_primary_selection_unstable_v1_types[3] = zwp_primary_selection_device_v1_interface;
        wp_primary_selection_unstable_v1_types[4] = wl_seat_interface;
        wp_primary_selection_unstable_v1_types[5] = zwp_primary_selection_source_v1_interface;

        wp_primary_selection_unstable_v1_types[7] = zwp_primary_selection_offer_v1_interface;
        wp_primary_selection_unstable_v1_types[8] = zwp_primary_selection_offer_v1_interface;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int zwp_primary_selection_device_v1_add_listener(zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_device_v1_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)zwp_primary_selection_device_v1, (void**)listener, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static zwp_primary_selection_device_v1* zwp_primary_selection_device_manager_v1_get_device(zwp_primary_selection_device_manager_v1* zwp_primary_selection_device_manager_v1, wl_seat* seat) =>
        (zwp_primary_selection_device_v1*)wl_proxy_marshal_flags(
            (wl_proxy*)zwp_primary_selection_device_manager_v1,
            ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_GET_DEVICE,
            zwp_primary_selection_device_v1_interface,
            wl_proxy_get_version((wl_proxy*)zwp_primary_selection_device_manager_v1),
            0,
            null,
            seat
        );
}
