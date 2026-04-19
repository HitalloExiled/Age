namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_data_device_listener
{
    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        wl_data_offer*  /* id */,
        void
    > data_offer;

    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        uint32_t        /* serial */,
        wl_surface*     /* surface */,
        wl_fixed_t      /* x */,
        wl_fixed_t      /* y */,
        wl_data_offer*  /* id */,
        void
    > enter;

    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        void
    > leave;

    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        int32_t         /* time */,
        wl_fixed_t      /* x */,
        wl_fixed_t      /* y */,
        void
    > motion;

    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        void
    > drop;

    public required delegate* unmanaged<
        void*           /* data */,
        wl_data_device* /* wl_data_device */,
        wl_data_offer*  /* id */,
        void
    > selection;
}
