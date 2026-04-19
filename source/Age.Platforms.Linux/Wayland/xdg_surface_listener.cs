namespace Age.Platforms.Linux.Wayland;

internal unsafe struct xdg_surface_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        xdg_surface* /* xdg_surface */,
        uint32_t     /* serial */,
        void
    > configure;
}
