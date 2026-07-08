namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_data_offer_listener
{
    public required delegate* unmanaged<
        void*          /* data */,
        wl_data_offer* /* wl_data_offer */,
        byte*          /* mime_type */,
        void
    > offer;

    public required delegate* unmanaged<
        void*          /* data */,
        wl_data_offer* /* wl_data_offer */,
        uint32_t       /* source_actions */,
        void
    > source_actions;

    public required delegate* unmanaged<
        void*          /* data */,
        wl_data_offer* /* wl_data_offer */,
        uint32_t       /* dnd_action */,
        void
    > action;
}
