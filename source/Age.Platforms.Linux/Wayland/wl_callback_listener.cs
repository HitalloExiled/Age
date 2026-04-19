namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_callback_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        wl_callback* /* wl_callback */,
        uint32_t     /* callback_data */,
        void
    > done;
}
