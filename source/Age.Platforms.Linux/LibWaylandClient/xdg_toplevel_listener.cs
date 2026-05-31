namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct xdg_toplevel_listener
{
    public required delegate* unmanaged<
        void*         /* data */,
        xdg_toplevel* /* xdg_toplevel */,
        int32_t       /* width */,
        int32_t       /* height */,
        wl_array*     /* states */,
        void
    > configure;

    public required delegate* unmanaged<
        void*         /* data */,
        xdg_toplevel* /* xdg_toplevel */,
        void
    > close;

    public required delegate* unmanaged<
        void*         /* data */,
        xdg_toplevel* /* xdg_toplevel */,
        int32_t       /* width */,
        int32_t       /* height */,
        void
    > configure_bounds;

    public required delegate* unmanaged<
        void*         /* data */,
        xdg_toplevel* /* xdg_toplevel */,
        wl_array*     /* capabilities */,
        void
    > wm_capabilities;
}
