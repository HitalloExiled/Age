namespace Age.Platforms.Linux.LibDecor;

internal unsafe struct libdecor_frame_interface
{
    public required delegate* unmanaged<
        libdecor_frame*         /* frame */,
        libdecor_configuration* /* configuration */,
        void*                   /* user_data */,
        void
    > configure;

    public required delegate* unmanaged<
        libdecor_frame* /* frame */,
        void*           /* user_data */,
        void
    > close;

    public required delegate* unmanaged<
        libdecor_frame* /* frame */,
        void*           /* user_data */,
        void
    > commit;

    public required delegate* unmanaged<
        libdecor_frame* /* frame */,
        byte*           /* seat_name */,
        void*           /* user_data */,
        void
    > dismiss_popup;

    public delegate* unmanaged<void> reserved0;
    public delegate* unmanaged<void> reserved1;
    public delegate* unmanaged<void> reserved2;
    public delegate* unmanaged<void> reserved3;
    public delegate* unmanaged<void> reserved4;
    public delegate* unmanaged<void> reserved5;
    public delegate* unmanaged<void> reserved6;
    public delegate* unmanaged<void> reserved7;
    public delegate* unmanaged<void> reserved8;
    public delegate* unmanaged<void> reserved9;
}
