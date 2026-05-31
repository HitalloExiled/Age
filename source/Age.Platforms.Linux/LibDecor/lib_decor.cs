using System.Runtime.InteropServices;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Linux.LibDecor;

internal struct libdecor;
internal struct libdecor_configuration;
internal struct libdecor_frame;
internal struct libdecor_state;

internal unsafe static partial class lib_decor
{
    private const string LIBRARY = "libdecor-0.so.0.200.5";

    [LibraryImport(LIBRARY)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool libdecor_configuration_get_content_size(libdecor_configuration* configuration, libdecor_frame* frame, int* width, int* height);

    [LibraryImport(LIBRARY)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool libdecor_configuration_get_window_state(libdecor_configuration* configuration, libdecor_window_state* window_state);

    [LibraryImport(LIBRARY)]
    public static partial libdecor_frame* libdecor_decorate(libdecor* context, wl_surface* surface, libdecor_frame_interface* iface, void* user_data);

    [LibraryImport(LIBRARY)]
    public static partial int libdecor_dispatch(libdecor* context, int timeout);

    [LibraryImport(LIBRARY)]
    public static partial void libdecor_frame_commit(libdecor_frame* frame, libdecor_state* state, libdecor_configuration* configuration);

    [LibraryImport(LIBRARY)]
    public static partial void libdecor_frame_close(libdecor_frame* frame);

    [LibraryImport(LIBRARY)]
    public static partial void libdecor_frame_set_app_id(libdecor_frame* frame, byte* app_id);

    [LibraryImport(LIBRARY)]
    public static partial void libdecor_frame_map(libdecor_frame* frame);

    [LibraryImport(LIBRARY)]
    public static partial libdecor* libdecor_new(wl_display* display, libdecor_interface* iface);

    [LibraryImport(LIBRARY)]
    public static partial void libdecor_state_free(libdecor_state* state);

    [LibraryImport(LIBRARY)]
    public static partial libdecor_state* libdecor_state_new(int width, int height);
}
