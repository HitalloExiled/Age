namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wp_fractional_scale_v1_listener
{
    public required delegate* unmanaged<
        void*                   /* data */,
        wp_fractional_scale_v1* /* wp_fractional_scale_v1 */,
        uint32_t                /* scale */,
        void
    > preferred_scale;
}
