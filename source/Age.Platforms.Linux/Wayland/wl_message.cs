namespace Age.Platforms.Linux.Wayland;

internal unsafe struct wl_message(byte* name, byte* signature, wl_interface** types)
{
    public byte*          name      = name;
    public byte*          signature = signature;
    public wl_interface** types     = types;
}
