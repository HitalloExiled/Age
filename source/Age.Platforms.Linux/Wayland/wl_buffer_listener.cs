namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_buffer_listener
{
    public required delegate* unmanaged<
        void*      /* data */,
        wl_buffer* /* wl_buffer */,
        void
    > release;
}
