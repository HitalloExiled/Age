namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_array
{
    public size_t size;
    public size_t alloc;
    public void*  data;
}
