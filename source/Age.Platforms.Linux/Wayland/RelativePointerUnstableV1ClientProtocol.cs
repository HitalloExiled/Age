using Age.Core.Extensions;
using System.Runtime.InteropServices;

using static Age.Platforms.Linux.Wayland.Helper;

namespace Age.Platforms.Linux.Wayland;

internal struct zwp_relative_pointer_manager_v1;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline")]
internal static unsafe class RelativePointerUnstableV1ClientProtocol
{
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

        zwp_relative_pointer_manager_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"),               Ustr(""),   relative_pointer_unstable_v1_types + 0),
            new(Ustr("get_relative_pointer"),  Ustr("no"), relative_pointer_unstable_v1_types + 6),
        ]);

        zwp_relative_pointer_manager_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_relative_pointer_manager_v1"), 1,
                2, zwp_relative_pointer_manager_v1_requests,
                0, null
            )
        );

        zwp_relative_pointer_v1_requests = NativeMemory.AllocSet<wl_message>([
            new(Ustr("destroy"), Ustr(""), relative_pointer_unstable_v1_types + 0),
        ]);

        zwp_relative_pointer_v1_events = NativeMemory.AllocSet<wl_message>([
            new(Ustr("relative_motion"), Ustr("uuffff"), relative_pointer_unstable_v1_types + 0),
        ]);

        zwp_relative_pointer_v1_interface = NativeMemory.AllocSet(
            new wl_interface(
                Ustr("zwp_relative_pointer_v1"), 1,
                1, zwp_relative_pointer_v1_requests,
                1, zwp_relative_pointer_v1_events
            )
        );

        relative_pointer_unstable_v1_types[6] = zwp_relative_pointer_v1_interface;
        relative_pointer_unstable_v1_types[7] = WaylandClientProtocol.wl_pointer_interface;
    }
}
