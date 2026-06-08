using System.Runtime.InteropServices;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Linux.LibWaylandCursor;

internal struct wl_cursor_theme;

internal unsafe static partial class lib_wayland_cursor
{
    private const string LIBRARY = "libwayland-cursor.so.0";

    [LibraryImport(LIBRARY)]
    public static partial wl_buffer* wl_cursor_image_get_buffer(wl_cursor_image* image);

    [LibraryImport(LIBRARY)]
    public static partial wl_cursor* wl_cursor_theme_get_cursor(wl_cursor_theme* theme, byte* name);

    [LibraryImport(LIBRARY)]
    public static partial int wl_cursor_frame(wl_cursor* cursor, uint32_t time);

    [LibraryImport(LIBRARY)]
    public static partial int wl_cursor_frame_and_duration(wl_cursor* cursor, uint32_t time, uint32_t* duration);

    [LibraryImport(LIBRARY)]
    public static partial void wl_cursor_theme_destroy(wl_cursor_theme* theme);

    [LibraryImport(LIBRARY)]
    public static partial wl_cursor_theme* wl_cursor_theme_load(byte* name, int size, wl_shm* shm);
}
