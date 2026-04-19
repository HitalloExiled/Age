namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_output_listener
{
    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        int32_t    /* x */,
        int32_t    /* y */,
        int32_t    /* physical_width */,
        int32_t    /* physical_height */,
        int32_t    /* subpixel */,
        byte*      /* make */,
        byte*      /* model */,
        int32_t    /* transform */,
        void
    > geometry;

    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        uint32_t   /* flags */,
        int32_t    /* width */,
        int32_t    /* height */,
        int32_t    /* refresh */,
        void
    > mode;

    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        void
    > done;

    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        int32_t    /* factor */,
        void
    > scale;

    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        byte*      /* name */,
        void
    > name;

    public required delegate* unmanaged<
        void*      /* data */,
        wl_output* /* wl_output */,
        byte*      /* description */,
        void
    > description;
}
