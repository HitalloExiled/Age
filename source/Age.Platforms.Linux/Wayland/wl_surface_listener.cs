namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_surface_listener
{
    public required delegate* unmanaged<
        void*       /* data */,
        wl_surface* /* wl_surface */,
        wl_output*  /* output */,
        void
    > enter;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_surface* /* wl_surface */,
        wl_output*  /* output */,
        void
    > leave;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_surface* /* wl_surface */,
        int32_t     /* factor */,
        void
    > preferred_buffer_scale;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_surface* /* wl_surface */,
        int32_t     /* transform */,
        void
    > preferred_buffer_transform;
}
