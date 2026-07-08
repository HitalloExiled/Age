namespace Age.Platforms.Linux.LibWaylandCursor;

internal unsafe struct wl_cursor
{
	public uint              image_count;
	public wl_cursor_image** images;
	public byte*             name;
};
