namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct zwp_primary_selection_source_v1_listener
{
    public required delegate* unmanaged<
        void*                            /* data */,
        zwp_primary_selection_source_v1* /* zwp_primary_selection_source_v1 */,
        byte*                            /* mime_type */,
        int                              /* fd */,
        void
    > send;

    public required delegate* unmanaged<
        void*                            /* data */,
        zwp_primary_selection_source_v1* /* zwp_primary_selection_source_v1 */,
        void
    > cancelled;
}
