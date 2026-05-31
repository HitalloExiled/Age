namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct zwp_relative_pointer_v1_listener
{
    public required delegate* unmanaged<
        void*                    /* data */,
        zwp_relative_pointer_v1* /* zwp_relative_pointer_v1 */,
        uint32_t                 /* utime_hi */,
        uint32_t                 /* utime_lo */,
        wl_fixed_t               /* dx */,
        wl_fixed_t               /* dy */,
        wl_fixed_t               /* dx_unaccel */,
        wl_fixed_t               /* dy_unaccel */,
        void
    > relative_motion;
}
