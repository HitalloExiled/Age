namespace Age.Platforms.Linux.Wayland;

internal unsafe struct zwp_pointer_gesture_pinch_v1_listener
{
    public required delegate* unmanaged<
        void*                         /* data */,
        zwp_pointer_gesture_pinch_v1* /* zwp_pointer_gesture_pinch_v1 */,
        uint32_t                      /* serial */,
        uint32_t                      /* time */,
        wl_surface*                   /* surface */,
        uint32_t                      /* fingers */,
        void
    > begin;

    public required delegate* unmanaged<
        void*                         /* data */,
        zwp_pointer_gesture_pinch_v1* /* zwp_pointer_gesture_pinch_v1 */,
        uint32_t                      /* time */,
        wl_fixed_t                    /* dx */,
        wl_fixed_t                    /* dy */,
        wl_fixed_t                    /* scale */,
        wl_fixed_t                    /* rotation */,
        void
    > update;

    public required delegate* unmanaged<
        void*                         /* data */,
        zwp_pointer_gesture_pinch_v1* /* zwp_pointer_gesture_pinch_v1 */,
        uint32_t                      /* serial */,
        uint32_t                      /* time */,
        int32_t                       /* cancelled */,
        void
    > end;
}
