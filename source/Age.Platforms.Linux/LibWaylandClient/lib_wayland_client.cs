using System.Runtime.InteropServices;

namespace Age.Platforms.Linux.LibWaylandClient;

internal struct wl_buffer;
internal struct wl_callback;
internal struct wl_compositor;
internal struct wl_data_device;
internal struct wl_data_device_manager;
internal struct wl_data_offer;
internal struct wl_display;
internal struct wl_keyboard;
internal struct wl_object;
internal struct wl_output;
internal struct wl_pointer;
internal struct wl_proxy;
internal struct wl_registry;
internal struct wl_seat;
internal struct wl_shm;
internal struct wl_shm_pool;
internal struct wl_surface;

internal unsafe static partial class lib_wayland_client
{
    private const string LIBRARY = "libwayland-client.so.0";

    private const int WL_BUFFER_DESTROY                      = 0;
    private const int WL_COMPOSITOR_CREATE_SURFACE           = 0;
    private const int WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE = 1;
    private const int WL_DISPLAY_GET_REGISTRY                = 1;

    private const int WL_POINTER_SET_CURSOR = 0;
    private const int WL_POINTER_RELEASE    = 1;

    private const int WL_REGISTRY_BIND   = 0;
    private const int WL_SHM_CREATE_POOL = 0;

    private const int WL_SEAT_GET_POINTER  = 0;
    private const int WL_SEAT_GET_KEYBOARD = 1;
    private const int WL_SEAT_GET_TOUCH    = 2;
    private const int WL_SEAT_RELEASE      = 3;

    private const uint WL_SURFACE_DESTROY              = 0;
    private const uint WL_SURFACE_ATTACH               = 1;
    private const uint WL_SURFACE_DAMAGE               = 2;
    private const uint WL_SURFACE_FRAME                = 3;
    private const uint WL_SURFACE_SET_OPAQUE_REGION    = 4;
    private const uint WL_SURFACE_SET_INPUT_REGION     = 5;
    private const uint WL_SURFACE_COMMIT               = 6;
    private const uint WL_SURFACE_SET_BUFFER_TRANSFORM = 7;
    private const uint WL_SURFACE_SET_BUFFER_SCALE     = 8;
    private const uint WL_SURFACE_DAMAGE_BUFFER        = 9;
    private const uint WL_SURFACE_OFFSET               = 10;

    private const uint WL_SHM_POOL_CREATE_BUFFER = 0;
    private const uint WL_SHM_POOL_DESTROY       = 1;
    private const uint WL_SHM_POOL_RESIZE        = 2;

    public const int WL_MARSHAL_FLAG_DESTROY = 1 << 0;

    public const uint WL_POINTER_ENTER_SINCE_VERSION                   = 1;
    public const uint WL_POINTER_LEAVE_SINCE_VERSION                   = 1;
    public const uint WL_POINTER_MOTION_SINCE_VERSION                  = 1;
    public const uint WL_POINTER_BUTTON_SINCE_VERSION                  = 1;
    public const uint WL_POINTER_AXIS_SINCE_VERSION                    = 1;
    public const uint WL_POINTER_FRAME_SINCE_VERSION                   = 5;
    public const uint WL_POINTER_AXIS_SOURCE_SINCE_VERSION             = 5;
    public const uint WL_POINTER_AXIS_STOP_SINCE_VERSION               = 5;
    public const uint WL_POINTER_AXIS_DISCRETE_SINCE_VERSION           = 5;
    public const uint WL_POINTER_AXIS_VALUE120_SINCE_VERSION           = 8;
    public const uint WL_POINTER_AXIS_RELATIVE_DIRECTION_SINCE_VERSION = 9;
    public const uint WL_POINTER_SET_CURSOR_SINCE_VERSION              = 1;
    public const uint WL_POINTER_RELEASE_SINCE_VERSION                 = 3;

    private static readonly nint handle = NativeLibrary.Load(LIBRARY);

    public static wl_interface* wl_buffer_interface              = GetInterface<wl_interface>(nameof(wl_buffer_interface));
    public static wl_interface* wl_callback_interface            = GetInterface<wl_interface>(nameof(wl_callback_interface));
    public static wl_interface* wl_compositor_interface          = GetInterface<wl_interface>(nameof(wl_compositor_interface));
    public static wl_interface* wl_data_device_interface         = GetInterface<wl_interface>(nameof(wl_data_device_interface));
    public static wl_interface* wl_data_device_manager_interface = GetInterface<wl_interface>(nameof(wl_data_device_manager_interface));
    public static wl_interface* wl_keyboard_interface            = GetInterface<wl_interface>(nameof(wl_keyboard_interface));
    public static wl_interface* wl_output_interface              = GetInterface<wl_interface>(nameof(wl_output_interface));
    public static wl_interface* wl_pointer_interface             = GetInterface<wl_interface>(nameof(wl_pointer_interface));
    public static wl_interface* wl_region_interface              = GetInterface<wl_interface>(nameof(wl_region_interface));
    public static wl_interface* wl_registry_interface            = GetInterface<wl_interface>(nameof(wl_registry_interface));
    public static wl_interface* wl_seat_interface                = GetInterface<wl_interface>(nameof(wl_seat_interface));
    public static wl_interface* wl_shm_interface                 = GetInterface<wl_interface>(nameof(wl_shm_interface));
    public static wl_interface* wl_shm_pool_interface            = GetInterface<wl_interface>(nameof(wl_shm_pool_interface));
    public static wl_interface* wl_surface_interface             = GetInterface<wl_interface>(nameof(wl_surface_interface));

    private static T* GetInterface<T>(string name) where T : unmanaged =>
        (T*)NativeLibrary.GetExport(handle, name);

    [LibraryImport(LIBRARY)]
    public static partial void wl_display_cancel_read(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial wl_display* wl_display_connect(byte* name);

    #region wl_callback
    public static void wl_callback_destroy(wl_callback* wl_callback) =>
        wl_proxy_destroy((wl_proxy*)wl_callback);

    public static void wl_callback_set_user_data(wl_callback* wl_callback, void* user_data) =>
	    wl_proxy_set_user_data((wl_proxy*) wl_callback, user_data);
    #endregion

    #region wl_display
    [LibraryImport(LIBRARY)]
    public static partial void wl_display_disconnect(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_dispatch_pending(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_flush(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_get_error(wl_display* display);

    [LibraryImport(LIBRARY)]
    public static partial uint32_t wl_display_get_protocol_error(wl_display*display, wl_interface** @interface, uint32_t *id);

    [LibraryImport(LIBRARY)]
    public static partial int wl_display_get_fd(wl_display* display);

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
    public static partial int wl_proxy_add_listener(wl_proxy* proxy, void** implementation, void* data);

    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_destroy(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial byte** wl_proxy_get_tag(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial void* wl_proxy_get_user_data(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial uint32_t wl_proxy_get_version(wl_proxy* proxy);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_array_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, wl_argument* args);

    [LibraryImport(LIBRARY)]
    public static partial wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags);

    public static wl_proxy* wl_proxy_marshal_flags(wl_proxy* proxy, uint32_t opcode, wl_interface* @interface, uint32_t version, uint32_t flags, ReadOnlySpan<wl_argument> args)
    {
        fixed (wl_argument* pArgs = args)
        {
            return wl_proxy_marshal_array_flags(proxy, opcode, @interface, version, flags, pArgs);
        }
    }

    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_set_tag(wl_proxy* proxy, byte** tag);

    [LibraryImport(LIBRARY)]
    public static partial void wl_proxy_set_user_data(wl_proxy *proxy, void *user_data);
    #endregion

    #region wl_proxy - wl_buffer
    public static void wl_buffer_destroy(wl_buffer* wl_buffer) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_buffer,
            WL_BUFFER_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)wl_buffer),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static int wl_buffer_add_listener(wl_buffer* wl_buffer, wl_buffer_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_buffer, (void**)listener, data);
    #endregion

    #region wl_proxy - wl_callback
    public static int wl_callback_add_listener(wl_callback* wl_callback, wl_callback_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_callback, (void**)listener, data);
    #endregion

    #region wl_proxy - wl_compositor
    public static wl_surface* wl_compositor_create_surface(wl_compositor* wl_compositor) =>
        (wl_surface*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_compositor,
            WL_COMPOSITOR_CREATE_SURFACE,
            wl_surface_interface,
            wl_proxy_get_version((wl_proxy*)wl_compositor),
            0,
            [default]
        );

    public static void wl_compositor_destroy(wl_compositor* wl_compositor) =>
        wl_proxy_destroy((wl_proxy*)wl_compositor);
    #endregion

    #region wl_proxy - wl_data_device
    public static int wl_data_device_add_listener(wl_data_device* wl_data_device, wl_data_device_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_data_device, (void**)listener, data);

    public static void wl_data_device_destroy(wl_data_device* wl_data_device) =>
        wl_proxy_destroy((wl_proxy*)wl_data_device);
    #endregion

    #region wl_proxy - wl_data_device_manager
    public static wl_data_device* wl_data_device_manager_get_data_device(wl_data_device_manager* wl_data_device_manager, wl_seat* seat) =>
        (wl_data_device*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_data_device_manager,
            WL_DATA_DEVICE_MANAGER_GET_DATA_DEVICE,
            wl_data_device_interface,
            wl_proxy_get_version((wl_proxy*)wl_data_device_manager),
            0,
            [default, seat]
        );
    #endregion

    #region wl_proxy - wl_keyboard
    public static int wl_keyboard_add_listener(wl_keyboard* wl_keyboard, wl_keyboard_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_keyboard, (void**)listener, data);

    public static void wl_keyboard_destroy(wl_keyboard* wl_keyboard) =>
        wl_proxy_destroy((wl_proxy*)wl_keyboard);
    #endregion

    #region wl_proxy - wl_output
    public static int wl_output_add_listener(wl_output* wl_output, wl_output_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_output, (void**)listener, data);

    public static void wl_output_destroy(wl_output* wl_output) =>
        wl_proxy_destroy((wl_proxy*)wl_output);
    #endregion

    #region wl_proxy - wl_pointer
    public static int wl_pointer_add_listener(wl_pointer* wl_pointer, wl_pointer_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_pointer, (void**)listener, data);

    public static void wl_pointer_destroy(wl_pointer* wl_pointer) =>
        wl_proxy_destroy((wl_proxy*)wl_pointer);

    public static uint wl_pointer_get_version(wl_pointer* wl_pointer) =>
        wl_proxy_get_version((wl_proxy*)wl_pointer);

    public static void wl_pointer_set_cursor(wl_pointer* wl_pointer, uint32_t serial, wl_surface* surface, int32_t hotspot_x, int32_t hotspot_y) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_pointer,
            WL_POINTER_SET_CURSOR,
            null,
            wl_proxy_get_version((wl_proxy*)wl_pointer),
            0,
            [serial, surface, hotspot_x, hotspot_y]
        );
    #endregion

    #region wl_proxy - wl_registry
    public static int wl_registry_add_listener(wl_registry* wl_registry, wl_registry_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_registry, (void**)listener, data);

    public static void* wl_registry_bind(wl_registry* wl_registry, uint32_t name, wl_interface* @interface, uint32_t version) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_registry,
            WL_REGISTRY_BIND,
            @interface,
            version,
            0,
            [name, @interface->name, version, default]
        );

    public static void wl_registry_destroy(wl_registry* wl_registry) =>
        wl_proxy_destroy((wl_proxy*)wl_registry);
    #endregion

    #region wl_proxy - wl_shm
    public static wl_shm_pool* wl_shm_create_pool(wl_shm* wl_shm, int32_t fd, int32_t size) =>
        (wl_shm_pool*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_shm,
            WL_SHM_CREATE_POOL,
            wl_shm_pool_interface,
            wl_proxy_get_version((wl_proxy*)wl_shm),
            0,
            [default, fd, size]
        );

    public static void wl_shm_destroy(wl_shm* wl_shm) =>
        wl_proxy_destroy((wl_proxy*)wl_shm);

    public static void wl_shm_pool_destroy(wl_shm_pool* wl_shm_pool) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_shm_pool,
            WL_SHM_POOL_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)wl_shm_pool),
            WL_MARSHAL_FLAG_DESTROY
        );
    #endregion

    public static wl_buffer* wl_shm_pool_create_buffer(wl_shm_pool* wl_shm_pool, int32_t offset, int32_t width, int32_t height, int32_t stride, uint32_t format) =>
        (wl_buffer*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_shm_pool,
            WL_SHM_POOL_CREATE_BUFFER,
            wl_buffer_interface,
            wl_proxy_get_version((wl_proxy*)wl_shm_pool),
            0,
            [default, offset, width, height, stride, format]
        );

    #region wl_proxy - wl_surface
    public static int wl_surface_add_listener(wl_surface* wl_surface, wl_surface_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_surface, (void**)listener, data);

    public static void wl_surface_attach(wl_surface* wl_surface, wl_buffer* buffer, int32_t x, int32_t y) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_ATTACH,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0,
            [buffer, x, y]
        );

    public static void wl_surface_set_buffer_scale(wl_surface* wl_surface, int32_t scale) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_SET_BUFFER_SCALE,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0,
            [scale]
        );

    public static void wl_surface_commit(wl_surface* wl_surface) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_COMMIT,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0
        );

    public static void wl_surface_damage(wl_surface* wl_surface, int32_t x, int32_t y, int32_t width, int32_t height) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_DAMAGE,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0,
            [x, y, width, height]
        );

    public static void wl_surface_damage_buffer(wl_surface* wl_surface, int32_t x, int32_t y, int32_t width, int32_t height) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_DAMAGE_BUFFER,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0,
            [x, y, width, height]
        );

    public static void wl_surface_destroy(wl_surface* wl_surface) =>
        wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_DESTROY,
            null,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            WL_MARSHAL_FLAG_DESTROY
        );

    public static wl_callback* wl_surface_frame(wl_surface* wl_surface) =>
        (wl_callback*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_surface,
            WL_SURFACE_FRAME,
            wl_callback_interface,
            wl_proxy_get_version((wl_proxy*)wl_surface),
            0,
            [default]
        );

    public static void* wl_surface_get_user_data(wl_surface* wl_surface) =>
        wl_proxy_get_user_data((wl_proxy*)wl_surface);
    #endregion

    #region wl_proxy - wl_seat
    public static int wl_seat_add_listener(wl_seat* wl_seat, wl_seat_listener* listener, void* data) =>
        wl_proxy_add_listener((wl_proxy*)wl_seat, (void**)listener, data);

    public static void wl_seat_destroy(wl_seat* wl_seat) =>
        wl_proxy_destroy((wl_proxy*)wl_seat);

    public static wl_pointer* wl_seat_get_pointer(wl_seat* wl_seat) =>
        (wl_pointer*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_seat,
            WL_SEAT_GET_POINTER,
            wl_pointer_interface,
            wl_proxy_get_version((wl_proxy*)wl_seat),
            0,
            [default]
        );

    public static wl_keyboard* wl_seat_get_keyboard(wl_seat* wl_seat) =>
        (wl_keyboard*)wl_proxy_marshal_flags(
            (wl_proxy*)wl_seat,
            WL_SEAT_GET_KEYBOARD,
            wl_keyboard_interface,
            wl_proxy_get_version((wl_proxy*)wl_seat),
            0,
            [default]
        );

    public static void* wl_seat_get_user_data(wl_seat* wl_seat) =>
        wl_proxy_get_user_data((wl_proxy*)wl_seat);
    #endregion
}
