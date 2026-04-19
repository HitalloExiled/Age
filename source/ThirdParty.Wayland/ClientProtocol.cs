using System.Runtime.InteropServices;

namespace ThirdParty.Wayland;

public struct wl_compositor;
public struct wl_display;
public struct wl_proxy;
public struct wl_registry;
public struct wl_surface;
public struct xdg_wm_base;

public unsafe struct wl_message(byte* name, byte* signature, wl_interface** types)
{
	public byte*          name      = name;
	public byte*          signature = signature;
	public wl_interface** types     = types;
}

public unsafe struct wl_interface(byte* name, int version, int method_count, wl_message* methods, int event_count, wl_message* events)
{
	public byte*       name         = name;
	public int         version      = version;
	public int         method_count = method_count;
	public wl_message* methods      = methods;
	public int         event_count  = event_count;
	public wl_message* events       = events;
}

public unsafe struct wl_registry_listener
{
    public required delegate* unmanaged<void*, wl_registry*, uint, byte*, uint, void> global;
    public required delegate* unmanaged<void*, wl_registry*, uint, void>              global_remove;
}

internal unsafe static class ClientProtocol
{
    public static wl_interface* wl_compositor_interface = GetInterface<wl_interface>("wl_compositor_interface");
    public static wl_interface* wl_registry_interface   = GetInterface<wl_interface>("wl_registry_interface");
    public static wl_interface* wl_surface_interface    = GetInterface<wl_interface>("wl_surface_interface");
    public static wl_interface* wl_seat_interface       = GetInterface<wl_interface>("wl_seat_interface");
    public static wl_interface* wl_output_interface     = GetInterface<wl_interface>("wl_output_interface");

    private static T* GetInterface<T>(string name) where T : unmanaged
    {
        var handle = NativeLibrary.Load(PInvoke.LIBRARY);

        return (T*)NativeLibrary.GetExport(handle, name);
    }
}
