namespace Age.Platforms.Linux.Wayland;

internal unsafe struct xdg_wm_base_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        xdg_wm_base* /* xdg_wm_base */,
        uint32_t     /* serial */,
        void
    > ping;
}
