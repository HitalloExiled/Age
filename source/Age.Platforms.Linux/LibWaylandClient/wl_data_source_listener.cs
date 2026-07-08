namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_data_source_listener
{
    public required delegate* unmanaged<
        void*            /* data */,
        wl_data_source*  /* wl_data_source */,
        byte*            /* mime_type */,
        int32_t          /* fd */,
        void
    > send;

    public required delegate* unmanaged<
        void*            /* data */,
        wl_data_source*  /* wl_data_source */,
        void
    > cancelled;
}
