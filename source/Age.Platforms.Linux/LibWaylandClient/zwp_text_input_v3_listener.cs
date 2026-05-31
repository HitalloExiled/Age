namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct zwp_text_input_v3_listener
{
    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        wl_surface*        /* surface */,
        void
    > enter;

    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        wl_surface*        /* surface */,
        void
    > leave;

    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        byte*              /* text */,
        int32_t            /* cursor_begin */,
        int32_t            /* cursor_end */,
        void
    > preedit_string;

    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        byte*              /* text */,
        void
    > commit_string;

    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        uint32_t           /* before_length */,
        uint32_t           /* after_length */,
        void
    > delete_surrounding_text;

    public required delegate* unmanaged<
        void*              /* data */,
        zwp_text_input_v3* /* zwp_text_input_v3 */,
        uint32_t           /* serial */,
        void
    > done;
}
