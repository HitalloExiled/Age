namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_seat_listener
{
    public required delegate* unmanaged<
        void*    /* data */,
        wl_seat* /* wl_seat */,
        uint32_t /* capabilities */,
        void
    > capabilities;

    public required delegate* unmanaged<
        void*    /* data */,
        wl_seat* /* wl_seat */,
        byte*    /* name */,
        void
    > name;
}
