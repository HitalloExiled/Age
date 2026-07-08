namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_keyboard_listener
{
    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        uint32_t     /* format */,
        int32_t      /* fd */,
        uint32_t     /* size */,
        void
    > keymap;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        uint32_t     /* serial */,
        wl_surface*  /* surface */,
        wl_array*    /* keys */,
        void
    > enter;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        uint32_t     /* serial */,
        wl_surface*  /* surface */,
        void
    > leave;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        uint32_t     /* serial */,
        uint32_t     /* time */,
        uint32_t     /* key */,
        uint32_t     /* state */,
        void
    > key;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        uint32_t     /* serial */,
        uint32_t     /* mods_depressed */,
        uint32_t     /* mods_latched */,
        uint32_t     /* mods_locked */,
        uint32_t     /* group */,
        void
    > modifiers;

    public required delegate* unmanaged<
        void*        /* data */,
        wl_keyboard* /* wl_keyboard */,
        int32_t      /* rate */,
        int32_t      /* delay */,
        void
    > repeat_info;
}
