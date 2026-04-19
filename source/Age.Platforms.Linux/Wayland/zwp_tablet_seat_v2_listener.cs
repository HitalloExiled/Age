namespace Age.Platforms.Linux.Wayland;

internal unsafe struct zwp_tablet_seat_v2_listener
{
    public required delegate* unmanaged<
        void*               /* data */,
        zwp_tablet_seat_v2* /* zwp_tablet_seat_v2 */,
        zwp_tablet_v2*      /* id */,
        void
    > tablet_added;

    public required delegate* unmanaged<
        void*               /* data */,
        zwp_tablet_seat_v2* /* zwp_tablet_seat_v2 */,
        zwp_tablet_tool_v2* /* id */,
        void
    > tool_added;

    public required delegate* unmanaged<
        void*               /* data */,
        zwp_tablet_seat_v2* /* zwp_tablet_seat_v2 */,
        zwp_tablet_pad_v2*  /* id */,
        void
    > pad_added;
}
