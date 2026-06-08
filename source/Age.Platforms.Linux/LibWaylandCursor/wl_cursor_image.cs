namespace Age.Platforms.Linux.LibWaylandCursor;

internal struct wl_cursor_image
{
	public uint32_t width;
	public uint32_t height;
	public uint32_t hotspot_x;
	public uint32_t hotspot_y;
	public uint32_t delay;
}
