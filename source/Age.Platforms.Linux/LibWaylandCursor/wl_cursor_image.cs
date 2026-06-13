namespace Age.Platforms.Linux.LibWaylandCursor;

internal struct wl_cursor_image
{
	public uint32_t width;
	public uint32_t height;
	public uint32_t hotspotX;
	public uint32_t hotspotY;
	public uint32_t delay;
}
