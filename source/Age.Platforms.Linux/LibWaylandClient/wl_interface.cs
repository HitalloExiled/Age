namespace Age.Platforms.Linux.LibWaylandClient;

internal unsafe struct wl_interface(byte* name, int version, int method_count, wl_message* methods, int event_count, wl_message* events)
{
    public byte*       name         = name;
    public int         version      = version;
    public int         method_count = method_count;
    public wl_message* methods      = methods;
    public int         event_count  = event_count;
    public wl_message* events       = events;
}
