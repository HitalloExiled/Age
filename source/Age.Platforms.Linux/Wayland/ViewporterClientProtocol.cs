using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;

namespace Age.Platforms.Linux.Wayland;

internal struct wp_viewport;
internal struct wp_viewporter;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class ViewporterProtocol
{
    private const uint WL_MARSHAL_FLAG_DESTROY     = 1 << 0;
    private const uint WP_VIEWPORT_DESTROY         = 0;
    private const uint WP_VIEWPORT_SET_DESTINATION = 2;
    private const uint WP_VIEWPORTER_DESTROY       = 0;
    private const uint WP_VIEWPORTER_GET_VIEWPORT  = 1;

    private static readonly wl_interface** viewporter_types;

    private readonly static wl_message* wp_viewporter_requests;
    private readonly static wl_message* wp_viewport_requests;

    public readonly static wl_interface* wp_viewporter_interface;
    public readonly static wl_interface* wp_viewport_interface;

    static ViewporterProtocol()
    {
        const int TYPES_COUNT = 6;

        viewporter_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_viewporter_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),      Ustr(""),   viewporter_types + 0),
            new(Ustr("get_viewport"), Ustr("no"), viewporter_types + 4),
        ]);

        wp_viewporter_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_viewporter"), 1,
                2, wp_viewporter_requests,
                0, null
            )
        );

        wp_viewport_requests = NativeMemory.Alloc<wl_message>([
            new(Ustr("destroy"),         Ustr(""),     viewporter_types + 0),
            new(Ustr("set_source"),      Ustr("ffff"), viewporter_types + 0),
            new(Ustr("set_destination"), Ustr("ii"),   viewporter_types + 0),
        ]);

        wp_viewport_interface = NativeMemory.Alloc(
            new wl_interface(
                Ustr("wp_viewport"), 1,
                3, wp_viewport_requests,
                0, null
            )
        );

        viewporter_types[4] = wp_viewport_interface;
        viewporter_types[5] = wl_surface_interface;
    }

    public static void wp_viewport_destroy(wp_viewport* wp_viewport) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wp_viewport,
            WP_VIEWPORT_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)wp_viewport),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static void wp_viewport_set_destination(wp_viewport* wp_viewport, int32_t width, int32_t height) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wp_viewport,
            WP_VIEWPORT_SET_DESTINATION,
            null,
            wl_proxy_get_version((wl_proxy*)wp_viewport),
            0,
            width,
            height
        );

    public static wp_viewport* wp_viewporter_get_viewport(wp_viewporter* wp_viewporter, wl_surface* surface) =>
        (wp_viewport*)wl_proxy_marshal_flags(
            (wl_proxy*)wp_viewporter,
            WP_VIEWPORTER_GET_VIEWPORT,
            wp_viewport_interface,
            wl_proxy_get_version((wl_proxy*)wp_viewporter),
            0,
            default,
            surface
        );

    public static void wp_viewporter_destroy(wp_viewporter* wp_viewporter) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wp_viewporter,
            WP_VIEWPORTER_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)wp_viewporter),
            WL_MARSHAL_FLAG_DESTROY
        );
}
