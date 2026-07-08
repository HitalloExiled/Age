namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct xdg_surface_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        xdg_surface* /* xdg_surface */,
        uint32_t     /* serial */,
        void
    > configure;
}
