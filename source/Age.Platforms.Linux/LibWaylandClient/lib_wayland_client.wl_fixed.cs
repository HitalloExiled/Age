namespace Age.Platforms.Linux.LibWaylandClient;

internal static partial class lib_wayland_client
{
    public static double wl_fixed_to_double(wl_fixed_t f) => f / 256.0;
}
