namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_buffer_listener
{
    public required delegate* unmanaged<
        void*      /* data */,
        wl_buffer* /* wl_buffer */,
        void
    > release;
}
