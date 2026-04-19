using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;

namespace Age.Platforms.Linux.Wayland;

internal struct wp_fractional_scale_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class FractionalScaleV1ClientProtocol
{
    private static readonly wl_interface** fractional_scale_v1_types;

    private readonly static wl_message* wp_fractional_scale_manager_v1_requests;
    private readonly static wl_message* wp_fractional_scale_v1_requests;
    private readonly static wl_message* wp_fractional_scale_v1_events;

    public readonly static wl_interface* wp_fractional_scale_manager_v1_interface;
    public readonly static wl_interface* wp_fractional_scale_v1_interface;

    static FractionalScaleV1ClientProtocol()
    {
        const int TYPES_COUNT = 3;

        fractional_scale_v1_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_fractional_scale_manager_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),              Ustr(""),   fractional_scale_v1_types + 0),
            new(Ustr("get_fractional_scale"), Ustr("no"), fractional_scale_v1_types + 1),
        ]);

        wp_fractional_scale_manager_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_fractional_scale_manager_v1"), 1,
                2, wp_fractional_scale_manager_v1_requests,
                0, null
            )
        );

        wp_fractional_scale_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"), Ustr(""), fractional_scale_v1_types + 0),
        ]);

        wp_fractional_scale_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("preferred_scale"), Ustr("u"), fractional_scale_v1_types + 0),
        ]);

        wp_fractional_scale_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_fractional_scale_v1"), 1,
                1, wp_fractional_scale_v1_requests,
                1, wp_fractional_scale_v1_events
            )
        );

        fractional_scale_v1_types[1] = wp_fractional_scale_v1_interface;
        fractional_scale_v1_types[2] = WaylandClientProtocol.wl_surface_interface;
    }
}
