using System.Runtime.InteropServices;

namespace Age.Platforms.Linux.Wayland;

internal struct wl_compositor;
internal struct wl_data_device;
internal struct wl_data_device_manager;
internal struct wl_data_offer;
internal struct wl_display;
internal struct wl_output;
internal struct wl_proxy;
internal struct wl_registry;
internal struct wl_seat;
internal struct wl_shm;
internal struct wl_surface;

internal unsafe static partial class WaylandClientProtocol
{
    public const string LIBRARY = "libwayland-client.so.0";

    public const int WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE = 1;
    public const int WL_DISPLAY_GET_REGISTRY                = 1;
    public const int WL_REGISTRY_BIND                       = 0;

    public static wl_interface* wl_compositor_interface          = GetInterface<wl_interface>(nameof(wl_compositor_interface));
    public static wl_interface* wl_data_device_interface         = GetInterface<wl_interface>(nameof(wl_data_device_interface));
    public static wl_interface* wl_data_device_manager_interface = GetInterface<wl_interface>(nameof(wl_data_device_manager_interface));
    public static wl_interface* wl_output_interface              = GetInterface<wl_interface>(nameof(wl_output_interface));
    public static wl_interface* wl_pointer_interface             = GetInterface<wl_interface>(nameof(wl_pointer_interface));
    public static wl_interface* wl_region_interface              = GetInterface<wl_interface>(nameof(wl_region_interface));
    public static wl_interface* wl_registry_interface            = GetInterface<wl_interface>(nameof(wl_registry_interface));
    public static wl_interface* wl_seat_interface                = GetInterface<wl_interface>(nameof(wl_seat_interface));
    public static wl_interface* wl_shm_interface                 = GetInterface<wl_interface>(nameof(wl_shm_interface));
    public static wl_interface* wl_surface_interface             = GetInterface<wl_interface>(nameof(wl_surface_interface));

    private static T* GetInterface<T>(string name) where T : unmanaged
    {
        var handle = NativeLibrary.Load(LIBRARY);

        return (T*)NativeLibrary.GetExport(handle, name);
    }

    [LibraryImport(LIBRARY)]
    public static partial wl_display* wl_display_connect(byte* name);

    #region wl_display
    [LibraryImport(LIBRARY)]
    public static partial void wl_display_disconnect(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_dispatch_pending(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_flush(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_prepare_read(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_read_events(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_roundtrip(wl_display* display);

    public static wl_registry* wl_display_get_registry(wl_display* wl_display)
    {
        var proxy = (wl_proxy*)wl_display;

        return (wl_registry*)wl_proxy_marshal_flags(
            proxy,
            WL_DISPLAY_GET_REGISTRY,
            wl_registry_interface,
            wl_proxy_get_version(proxy),
            0
        );
    }
    #endregion

    #region wl_proxy
    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_destroy(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial void* wl_proxy_get_user_data(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial byte** wl_proxy_get_tag(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial uint32_t wl_proxy_get_version(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial int wl_proxy_add_listener(wl_proxy* proxy, void** implementation, void* data);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, void* arg1);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, void* arg1, void* arg2);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, void* arg1, void* arg2, void* arg3);

    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_set_tag(wl_proxy* proxy, byte** tag);
    #endregion

    #region wl_proxy - wl_data_device
    public static int wl_data_device_add_listener(wl_data_device* wl_data_device, wl_data_device_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_data_device, (void**)listener, data);
    #endregion

    #region wl_proxy - wl_data_device_manager
    public static wl_data_device* wl_data_device_manager_get_data_device(wl_data_device_manager* wl_data_device_manager, wl_seat* seat) =>
        (wl_data_device*)wl_proxy_marshal_flags(
            wl_data_device_manager,
            WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE,
            wl_data_device_interface,
            wl_proxy_get_version((wl_proxy*)wl_data_device_manager),
            0,
            null,
            seat
        );

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_data_device_manager* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, void* arg1, wl_seat* arg2);
    #endregion

    #region wl_proxy - wl_output
    public static int wl_output_add_listener(wl_output* wl_output, wl_output_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_output, (void**)listener, data);
    #endregion

    #region wl_proxy - wl_registry
    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_registry* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, uint arg1, byte* @arg2, uint arg3, void* arg4);

    public static int wl_registry_add_listener(wl_registry* wl_registry, wl_registry_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_registry, (void**)listener, data);

    public static void* wl_registry_bind(wl_registry* wl_registry, uint32_t name, wl_interface* @interface, uint32_t version) =>
        wl_proxy_marshal_flags(
            wl_registry,
            WL_REGISTRY_BIND,
            @interface,
            version,
            0,
            name,
            @interface->name,
            version,
            null
        );
    #endregion

    #region wl_proxy - wl_seat
    public static void* wl_seat_get_user_data(wl_seat* wl_seat) =>
        wl_proxy_get_user_data((wl_proxy*)wl_seat);

    public static int wl_seat_add_listener(wl_seat* wl_seat, wl_seat_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_seat, (void**)listener, data);
    #endregion
}
