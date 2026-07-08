namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_registry_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        wl_registry* /* wl_registry */,
        uint32_t     /* name */,
        byte*        /* interface */,
        uint32_t     /* version */,
        void
    > global;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_registry* /* wl_registry */,
        uint32_t     /* name */,
        void
    > global_remove;
}
