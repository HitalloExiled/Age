using System.Runtime.InteropServices;
using Age.Core.Extensions;

using static Age.Platforms.Linux.Wayland.Helper;

namespace Age.Platforms.Linux.Wayland;

internal struct wp_viewport;
internal struct wp_viewporter;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class ViewporterProtocol
{
    private static readonly wl_interface** viewporter_types;

    private readonly static wl_message* wp_viewporter_requests;
    private readonly static wl_message* wp_viewport_requests;

    public readonly static wl_interface* wp_viewporter_interface;
    public readonly static wl_interface* wp_viewport_interface;

    static ViewporterProtocol()
    {
        const int TYPES_COUNT = 6;

        viewporter_types = (wl_interface**)NativeMemory.AllocZeroed<nint>(TYPES_COUNT);

        wp_viewporter_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),      Ustr(""),   viewporter_types + 0),
            new(Ustr("get_viewport"), Ustr("no"), viewporter_types + 4),
        ]);

        wp_viewporter_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_viewporter"), 1,
                2, wp_viewporter_requests,
                0, null
            )
        );

        wp_viewport_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),         Ustr(""),     viewporter_types + 0),
            new(Ustr("set_source"),      Ustr("ffff"), viewporter_types + 0),
            new(Ustr("set_destination"), Ustr("ii"),   viewporter_types + 0),
        ]);

        wp_viewport_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("wp_viewport"), 1,
                3, wp_viewport_requests,
                0, null
            )
        );

        viewporter_types[4] = wp_viewport_interface;
        viewporter_types[5] = WaylandClientProtocol.wl_surface_interface;
    }
}
