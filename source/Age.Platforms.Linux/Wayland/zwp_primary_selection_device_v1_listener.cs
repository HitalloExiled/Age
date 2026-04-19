namespace Age.Platforms.Linux.Wayland;

internal unsafe struct zwp_primary_selection_device_v1_listener
{
    public required delegate* unmanaged<
        void*                            /* data */,
        zwp_primary_selection_device_v1* /* zwp_primary_selection_device_v1 */,
        zwp_primary_selection_offer_v1*  /* offer */,
        void
    > data_offer;

    public required delegate* unmanaged<
        void*                            /* data */,
        zwp_primary_selection_device_v1* /* zwp_primary_selection_device_v1 */,
        zwp_primary_selection_offer_v1*  /* id */,
        void
    > selection;
}
