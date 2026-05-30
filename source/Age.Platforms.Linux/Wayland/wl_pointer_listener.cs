namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_pointer_listener
{
    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* serial */,
        wl_surface* /* surface */,
        wl_fixed_t  /* surface_x */,
        wl_fixed_t  /* surface_y */,
        void
    > enter;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* serial */,
        wl_surface* /* surface */,
        void
    > leave;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* time */,
        wl_fixed_t  /* surface_x */,
        wl_fixed_t  /* surface_y */,
        void
    > motion;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* serial */,
        uint32_t    /* time */,
        uint32_t    /* button */,
        uint32_t    /* state */,
        void
    > button;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* time */,
        uint32_t    /* axis */,
        wl_fixed_t  /* value */,
        void
    > axis;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        void
    > frame;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* axis_source */,
        void
    > axis_source;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* time */,
        uint32_t    /* axis */,
        void
    > axis_stop;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* axis */,
        int32_t     /* discrete */,
        void
    > axis_discrete;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* axis */,
        int32_t     /* value120 */,
        void
    > axis_value120;

    public required delegate* unmanaged<
        void*       /* data */,
        wl_pointer* /* wl_pointer */,
        uint32_t    /* axis */,
        uint32_t    /* direction */,
        void
    > axis_relative_direction;
}
