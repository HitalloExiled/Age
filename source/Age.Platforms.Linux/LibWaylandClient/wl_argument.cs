using System.Runtime.InteropServices;

namespace Age.Platforms.Linux.LibWaylandClient;

[StructLayout(LayoutKind.Explicit)]
internal unsafe struct wl_argument
{
    [FieldOffset(0)]
	public int32_t i;

    [FieldOffset(0)]
	public uint32_t u;

    [FieldOffset(0)]
	public wl_fixed_t f;

    [FieldOffset(0)]
	public byte* s;

    [FieldOffset(0)]
	public wl_object *o;

    [FieldOffset(0)]
	public uint32_t n;

    [FieldOffset(0)]
	public wl_array *a;

    [FieldOffset(0)]
	public int32_t h;

    public static implicit operator wl_argument(int32_t value)   => new() { i = value };
    public static implicit operator wl_argument(uint32_t value)  => new() { u = value };
    public static implicit operator wl_argument(byte* value)     => new() { s = value };
    public static implicit operator wl_argument(void* value)     => new() { o = (wl_object*)value };
    public static implicit operator wl_argument(wl_array* value) => new() { a = value };
};
