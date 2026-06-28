using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.LibWaylandClient.Helper;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct zwp_primary_selection_device_manager_v1;
internal struct zwp_primary_selection_device_v1;
internal struct zwp_primary_selection_offer_v1;
internal struct zwp_primary_selection_source_v1;
internal struct zwp_text_input_manager_v3;
internal struct zwp_text_input_v3;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal unsafe static class primary_selection
{
    private const uint ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_CREATE_SOURCE = 0;
    private const uint ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_DESTROY = 2;
    private const uint ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_GET_DEVICE = 1;

    private const uint ZWP_PRIMARY_SELECTION_DEVICE_V1_DESTROY = 1;
    private const uint ZWP_PRIMARY_SELECTION_DEVICE_V1_SET_SELECTION = 0;
    private const uint ZWP_PRIMARY_SELECTION_OFFER_V1_DESTROY = 1;
    private const uint ZWP_PRIMARY_SELECTION_OFFER_V1_RECEIVE = 0;
    private const uint ZWP_PRIMARY_SELECTION_SOURCE_V1_DESTROY = 1;
    private const uint ZWP_PRIMARY_SELECTION_SOURCE_V1_OFFER = 0;

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

    static primary_selection()
    {
        const uint TYPES_COUNT = 9;

        wp_primary_selection_unstable_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        zwp_primary_selection_device_manager_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("create_source"), Ustr("n"),  wp_primary_selection_unstable_v1_types + 2),
            new(Ustr("get_device"),    Ustr("no"), wp_primary_selection_unstable_v1_types + 3),
            new(Ustr("destroy"),       Ustr(""),   wp_primary_selection_unstable_v1_types + 0)
        ]);

        zwp_primary_selection_device_manager_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_primary_selection_device_manager_v1"), 1,
                3, zwp_primary_selection_device_manager_v1_requests,
                0, null
            )
        );

        zwp_primary_selection_device_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("set_selection"), Ustr("?ou"), wp_primary_selection_unstable_v1_types + 5),
	        new(Ustr("destroy"),       Ustr(""), wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_device_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("data_offer"), Ustr("n"),  wp_primary_selection_unstable_v1_types + 7),
	        new(Ustr("selection"),  Ustr("?o"), wp_primary_selection_unstable_v1_types + 8),
        ]);

        zwp_primary_selection_device_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_primary_selection_device_v1"), 1,
                2, zwp_primary_selection_device_v1_requests,
                2, zwp_primary_selection_device_v1_events
            )
        );

        zwp_primary_selection_offer_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("receive"), Ustr("sh"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("destroy"), Ustr(""),   wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_offer_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("offer"), Ustr("s"), wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_offer_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_primary_selection_offer_v1"), 1,
                2, zwp_primary_selection_offer_v1_requests,
                1, zwp_primary_selection_offer_v1_events
            )
        );

        zwp_primary_selection_source_v1_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("offer"),   Ustr("s"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("destroy"), Ustr(""),  wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_source_v1_events = NativeMemory.Alloc<wl_message>([
            new(Ustr("send"),      Ustr("sh"), wp_primary_selection_unstable_v1_types + 0),
	        new(Ustr("cancelled"), Ustr(""),   wp_primary_selection_unstable_v1_types + 0),
        ]);

        zwp_primary_selection_source_v1_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("zwp_primary_selection_source_v1"), 1,
                2, zwp_primary_selection_source_v1_requests,
                2, zwp_primary_selection_source_v1_events
            )
        );

        wp_primary_selection_unstable_v1_types[2] = zwp_primary_selection_source_v1_interface;
        wp_primary_selection_unstable_v1_types[3] = zwp_primary_selection_device_v1_interface;
        wp_primary_selection_unstable_v1_types[4] = lib_wayland_client.wl_seat_interface;
        wp_primary_selection_unstable_v1_types[5] = zwp_primary_selection_source_v1_interface;

        wp_primary_selection_unstable_v1_types[7] = zwp_primary_selection_offer_v1_interface;
        wp_primary_selection_unstable_v1_types[8] = zwp_primary_selection_offer_v1_interface;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int zwp_primary_selection_device_v1_add_listener(zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_device_v1_listener* listener, void* data) =>
        lib_wayland_client.wl_proxy_add_listener((wl_proxy*)zwp_primary_selection_device_v1, (void**)listener, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static zwp_primary_selection_device_v1* zwp_primary_selection_device_manager_v1_get_device(zwp_primary_selection_device_manager_v1* zwp_primary_selection_device_manager_v1, wl_seat* seat) =>
        (zwp_primary_selection_device_v1*)lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)zwp_primary_selection_device_manager_v1,
            ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_GET_DEVICE,
            zwp_primary_selection_device_v1_interface,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)zwp_primary_selection_device_manager_v1),
            0,
            [default, seat]
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static zwp_primary_selection_source_v1* zwp_primary_selection_device_manager_v1_create_source(zwp_primary_selection_device_manager_v1* manager) =>
        (zwp_primary_selection_source_v1*)lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)manager,
            ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_CREATE_SOURCE,
            zwp_primary_selection_source_v1_interface,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)manager),
            0
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_device_manager_v1_destroy(zwp_primary_selection_device_manager_v1* manager) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)manager,
            ZWP_PRIMARY_SELECTION_DEVICE_MANAGER_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)manager),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_device_v1_set_selection(zwp_primary_selection_device_v1* device, zwp_primary_selection_source_v1* source, uint32_t serial) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)device,
            ZWP_PRIMARY_SELECTION_DEVICE_V1_SET_SELECTION,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)device),
            0,
            [source, serial]
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_device_v1_destroy(zwp_primary_selection_device_v1* device) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)device,
            ZWP_PRIMARY_SELECTION_DEVICE_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)device),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int zwp_primary_selection_offer_v1_add_listener(zwp_primary_selection_offer_v1* offer, zwp_primary_selection_offer_v1_listener* listener, void* data) =>
        lib_wayland_client.wl_proxy_add_listener((wl_proxy*)offer, (void**)listener, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_offer_v1_receive(zwp_primary_selection_offer_v1* offer, byte* mimeType, int fd) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)offer,
            ZWP_PRIMARY_SELECTION_OFFER_V1_RECEIVE,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)offer),
            0,
            [mimeType, fd]
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* zwp_primary_selection_offer_v1_get_user_data(zwp_primary_selection_offer_v1* zwp_primary_selection_offer_v1) =>
        lib_wayland_client.wl_proxy_get_user_data((wl_proxy*)zwp_primary_selection_offer_v1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_offer_v1_destroy(zwp_primary_selection_offer_v1* offer) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)offer,
            ZWP_PRIMARY_SELECTION_OFFER_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)offer),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int zwp_primary_selection_source_v1_add_listener(zwp_primary_selection_source_v1* source, zwp_primary_selection_source_v1_listener* listener, void* data) =>
        lib_wayland_client.wl_proxy_add_listener((wl_proxy*)source, (void**)listener, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_source_v1_offer(zwp_primary_selection_source_v1* source, byte* mimeType) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)source,
            ZWP_PRIMARY_SELECTION_SOURCE_V1_OFFER,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)source),
            0,
            [mimeType]
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void zwp_primary_selection_source_v1_destroy(zwp_primary_selection_source_v1* source) =>
        lib_wayland_client.wl_proxy_marshal_flags(
            (wl_proxy*)source,
            ZWP_PRIMARY_SELECTION_SOURCE_V1_DESTROY,
            null,
            lib_wayland_client.wl_proxy_get_version((wl_proxy*)source),
            lib_wayland_client.WL_MARSHAL_FLAG_DESTROY
        );
}
