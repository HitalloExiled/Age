// #undef LINUX
#if LINUX
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using Age.Platforms.Linux.Libc;
using Age.Platforms.Linux.LibDecor;
using Age.Platforms.Linux.Wayland;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static Age.Platforms.Linux.AsmGenericErrno;
using static Age.Platforms.Linux.Libc.Libc;
using static Age.Platforms.Linux.LibDecor.LibDecor;
using static Age.Platforms.Linux.Wayland.CursorShapeV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.FractionalScaleV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.IdleInhibitUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.PointerConstraintsUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.PointerGesturesUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.RelativePointerUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.TabletUnstableV2ClientProtocol;
using static Age.Platforms.Linux.Wayland.TextInputUnstableV3ClientProtocol;
using static Age.Platforms.Linux.Wayland.ViewporterProtocol;
using static Age.Platforms.Linux.Wayland.WaylandClientProtocol;
using static Age.Platforms.Linux.Wayland.WpPrimarySelectionUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.XdgActivationV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.XdgDecorationUnstableV1ClientProtocol;
using static Age.Platforms.Linux.Wayland.XdgShellClientProtocol;
using static Age.Platforms.Linux.Wayland.XdgSystemBellV1ClientProtocol;

namespace Age.Platforms.Display;

public unsafe partial class Window
{
    private const bool MOCK_BUFFER = true;

    private readonly static NativeList<Pointer<wl_seat>>   seats   = [];
    private readonly static NativeList<Pointer<wl_output>> outputs = [];

    private static readonly Lock                                   @lock           = new();
    private static readonly Thread                                 eventLoopThread = new(EventLoop);
    private readonly static byte**                                 pTag            = (byte**)NativeMemory.AllocSet((nint)MemoryMarshal.CreateUTF8StringBuffer("Age"));
    private readonly static Dictionary<uint, Pointer<ScreenState>> screenStates    = [];

    #region Unmanaged Listeners
    [FixedAddressValueType]
    private static readonly wl_buffer_listener bufferListener = new()
    {
        release = &OnBufferRelease,
    };

    [FixedAddressValueType]
    private static readonly wl_data_device_listener dataDeviceListener = new()
    {
        data_offer = &OnDataDeviceDataOffer,
        enter      = &OnDataDeviceEnter,
        leave      = &OnDataDeviceLeave,
        motion     = &OnDataDeviceMotion,
        drop       = &OnDataDeviceDrop,
        selection  = &OnDataDeviceSelection,
    };

    [FixedAddressValueType]
    private static readonly wp_fractional_scale_v1_listener fractionalScaleListener = new()
    {
        preferred_scale = &OnFractionalScalePreferredScale,
    };

    [FixedAddressValueType]
    private static readonly wl_callback_listener frameCallbackListener = new()
    {
        done = &OnFrameCallbackListenerDone,
    };

    [FixedAddressValueType]
    private readonly static libdecor_frame_interface frameInterface = new()
    {
        configure     = &OnLibdecorFrameConfigure,
        close         = &OnLibdecorFrameClose,
        commit        = &OnLibdecorFrameCommit,
        dismiss_popup = &OnLibdecorFrameDismissPopup,
    };

    [FixedAddressValueType]
    private readonly static libdecor_interface libdecorInterface = new()
    {
        error = &OnLibdecorError
    };

    [FixedAddressValueType]
    private static readonly wl_output_listener outputListener = new()
    {
        geometry    = &OnOutputGeometry,
        mode        = &OnOutputMode,
        done        = &OnOutputDone,
        scale       = &OnOutputScale,
        name        = &OnOutputName,
        description = &OnOutputDescription,
    };

    [FixedAddressValueType]
    private static readonly zwp_primary_selection_device_v1_listener primarySelectionDeviceListener = new()
    {
        data_offer = &OnWpPrimarySelectionDevicedataOffer,
        selection  = &OnWpPrimarySelectionDeviceselection,
    };

    [FixedAddressValueType]
    private static readonly wl_registry_listener registryListener = new()
    {
        global        = &OnRegistryGlobal,
        global_remove = &OnRegistryGlobalRemove,
    };

    [FixedAddressValueType]
    private static readonly wl_seat_listener seatListener = new()
    {
        capabilities = &OnSeatCapabilities,
        name         = &OnSeatName,
    };

    [FixedAddressValueType]
    private static readonly wl_surface_listener surfaceListener = new()
    {
        enter                      = &OnSurfaceEnter,
        leave                      = &OnSurfaceLeave,
        preferred_buffer_scale     = &OnSurfacePreferredBufferScale,
        preferred_buffer_transform = &OnSurfacePreferredBufferTransform,
    };

    [FixedAddressValueType]
    private static readonly zwp_tablet_seat_v2_listener tabletSeatListener = new()
    {
        tablet_added = &OnTabletSeatTabletAdded,
        tool_added   = &OnTabletSeatToolAdded,
        pad_added    = &OnTabletSeatPadAdded,
    };

    [FixedAddressValueType]
    private static readonly zwp_text_input_v3_listener textInputListener = new()
    {
        enter                   = &OnTextInputEnter,
        leave                   = &OnTextInputLeave,
        preedit_string          = &OnTextInputPreeditString,
        commit_string           = &OnTextInputCommitString,
        delete_surrounding_text = &OnTextInputDeleteSurroundingText,
        done                    = &OnTextInputDone,
    };

    [FixedAddressValueType]
    private static readonly xdg_wm_base_listener wmBaseListener = new()
    {
        ping = &OnWmBasePing,
    };
    #endregion

    #region Unmanaged fields
    private static Named<xdg_activation_v1>                 activation;
    private static Named<wl_compositor>                     compositor;
    private static wl_seat*                                 currentSeat;
    private static Named<wp_cursor_shape_manager_v1>        cursorShapeManager;
    private static Named<wl_data_device_manager>            dataDeviceManager;
    private static Named<zxdg_decoration_manager_v1>        decorationManager;
    private static bool                                     destroyed;
    private static wl_display*                              display;
    private static Named<wp_fractional_scale_manager_v1>    fractionalScaleManager;
    private static Named<zwp_idle_inhibit_manager_v1>       idleInhibitManager;
    private static libdecor*                                libdecorContext;
    private static Named<zwp_pointer_constraints_v1>        pointerConstraints;
    private static Named<zwp_pointer_gestures_v1>           pointerGestures;
    private static zwp_primary_selection_device_manager_v1* primarySelectionDeviceManager;
    private static wl_registry*                             registry;
    private static Named<zwp_relative_pointer_manager_v1>   relativePointerManager;
    private static Named<wl_shm>                            shm;
    private static Named<xdg_system_bell_v1>                systemBell;
    private static Named<zwp_tablet_manager_v2>             tabletManager;
    private static Named<zwp_text_input_manager_v3>         textInputManager;
    private static Named<wp_viewporter>                     viewporter;
    private static Named<xdg_wm_base>                       wmBase;
    #endregion

    private readonly WindowState* state;

    public partial Size<uint> ClientSize => throw new NotImplementedException();

    public partial Cursor Cursor
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;

            this.UpdateCursor();
        }
    }

    public partial Point<int> Position
    {
        get => this.state->Position;
        set => throw new NotImplementedException();
    }

    public partial Size<uint> Size
    {
        get => this.state->Size.Cast<uint>();
        set => UpdateSize(this.state, value.Cast<int>());
    }

    public partial string Title
    {
        get => this.title;
        set => throw new NotImplementedException();
    }

    public partial Window(string? title, Size<uint>? size, Point<int>? position, Window? parent)
    {
        this.title = title ?? "Untitled";

        this.state = NativeMemory.AllocSet(
            new WindowState
            {
                Surface  = wl_compositor_create_surface(compositor),
                Size     = (size ?? defaultSize).Cast<int>(),
                Position = position ?? default,
            }
        );

        this.Handle = (nint)this.state->Surface;

        WindowsMap[this.Handle] = this;

        this.Parent?.Children.Add(this);

        SetProxyTag((wl_proxy*)this.state->Surface);

        fixed (wl_surface_listener* pSurfaceListener = &surfaceListener)
        {
            wl_surface_add_listener(this.state->Surface, pSurfaceListener, null);
        }

        if (viewporter != default)
        {
            this.state->Viewport = wp_viewporter_get_viewport(viewporter, this.state->Surface);

            if (fractionalScaleManager != default)
            {
                this.state->FractionalScale = wp_fractional_scale_manager_v1_get_fractional_scale(fractionalScaleManager, this.state->Surface);

                fixed (wp_fractional_scale_v1_listener* pFractionalScaleListener = &fractionalScaleListener)
                {
                    wp_fractional_scale_v1_add_listener(this.state->FractionalScale, pFractionalScaleListener, this.state);
                }
            }
        }

        fixed (libdecor_frame_interface* pFrameInterface = &frameInterface)
        {
            this.state->Frame = libdecor_decorate(libdecorContext, this.state->Surface, pFrameInterface, this.state);
        }

        libdecor_frame_map(this.state->Frame);

        this.state->FrameCallBack = wl_surface_frame(this.state->Surface);

        fixed (wl_callback_listener* pFrameCallbackListener = &frameCallbackListener)
        {
            wl_callback_add_listener(this.state->FrameCallBack, pFrameCallbackListener, this.state);
        }

        wl_surface_commit(this.state->Surface);

        _ = wl_display_roundtrip(display);

        if (MOCK_BUFFER)
        {
            MockBuffer(state);
        }

        UpdateSize(this.state, this.state->Size);

        using var uAppId = new UnmanagedString(appId);

        libdecor_frame_set_app_id(this.state->Frame, uAppId);
    }

    [UnmanagedCallersOnly]
    private static void OnBufferRelease(void* data, wl_buffer* buffer)
    {
        var bufferData = (BufferData*)data;

        wl_buffer_destroy(buffer);

        _ = munmap(bufferData->Data, bufferData->Size);

        NativeMemory.Free(bufferData);
    }

    private static void MockBuffer(WindowState* windowState)
    {
        using var fdName = new UnmanagedString($"age_buffer.{Guid.NewGuid()}");

        var fd = memfd_create(fdName, 0);

        var width  = windowState->Size.Width;
        var height = windowState->Size.Height;

        var stride = width * 4;
        var size   = stride * height;

        _ = ftruncate(fd, size);

        var pixelData = mmap(null, (ulong)size, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0);

        var pixels = (uint*)pixelData;

        for (var i = 0; i < width * height; i++)
        {
            pixels[i] = 0xFF550055; // ARGB
        }

        var pool = wl_shm_create_pool(shm, fd, size);

        var data = NativeMemory.AllocSet(
            new BufferData
            {
                Buffer = wl_shm_pool_create_buffer(pool, 0, width, height, stride, 1),
                Data   = pixels,
                Size   = (ulong)size,
            }
        );

        fixed (wl_buffer_listener* pbufferListener = &bufferListener)
        {
            wl_buffer_add_listener(data->Buffer, pbufferListener, data);
        }

        wl_shm_pool_destroy(pool);

        _ = close(fd);

        wl_surface_attach(windowState->Surface, data->Buffer, 0, 0);
        wl_surface_damage(windowState->Surface, 0, 0, width, height);
        wl_surface_commit(windowState->Surface);

        _ = wl_display_flush(display);
    }

    private static bool ProxyIsAge(wl_proxy* proxy)
    {
        NullReferenceException.ValidateNotNull(proxy);

        return wl_proxy_get_tag(proxy) == pTag;
    }

    private static void SetProxyTag(wl_proxy* proxy)
    {
        NullReferenceException.ValidateNotNull(proxy);

        wl_proxy_set_tag(proxy, pTag);
    }

    private static SeatState* GetSeatState(wl_seat* seat) =>
        seat != null && ProxyIsAge((wl_proxy*)seat) ? (SeatState*)wl_seat_get_user_data(seat) : default;

    #region Unmanaged Callers
    [UnmanagedCallersOnly]
    private static void OnDataDeviceDrop(void* data, wl_data_device* dataDevice) =>
        Console.WriteLine(nameof(OnDataDeviceDrop));

    [UnmanagedCallersOnly]
    private static void OnDataDeviceEnter(void* data, wl_data_device* dataDevice, uint serial, wl_surface* surface, int x, int y, wl_data_offer* id) =>
        Console.WriteLine(nameof(OnDataDeviceEnter));

    [UnmanagedCallersOnly]
    private static void OnDataDeviceLeave(void* data, wl_data_device* dataDevice) =>
        Console.WriteLine(nameof(OnDataDeviceLeave));

    [UnmanagedCallersOnly]
    private static void OnDataDeviceMotion(void* data, wl_data_device* dataDevice, int time, int x, int y) =>
        Console.WriteLine(nameof(OnDataDeviceMotion));

    [UnmanagedCallersOnly]
    private static void OnDataDeviceDataOffer(void* data, wl_data_device* dataDevice, wl_data_offer* id) =>
        Console.WriteLine(nameof(OnDataDeviceDataOffer));

    [UnmanagedCallersOnly]
    private static void OnDataDeviceSelection(void* data, wl_data_device* dataDevice, wl_data_offer* id) =>
        Console.WriteLine(nameof(OnDataDeviceSelection));

    [UnmanagedCallersOnly]
    private static void OnFractionalScalePreferredScale(void* data, wp_fractional_scale_v1* fractionalScale, uint scale) =>
        Console.WriteLine(nameof(OnFractionalScalePreferredScale));

    [UnmanagedCallersOnly]
    private static void OnFrameCallbackListenerDone(void* data, wl_callback* callback, uint callbackData) =>
        Console.WriteLine(nameof(OnFrameCallbackListenerDone));

    [UnmanagedCallersOnly]
    private static void OnLibdecorError(libdecor* context, libdecor_error error, byte* pMessage)
    {
        var message = Encoding.GetStringFromNullTerminated(pMessage);

        Logger.Error($"libdecor error {message}");
    }

    [UnmanagedCallersOnly]
    private static void OnLibdecorFrameClose(libdecor_frame* frame, void* userData)
    {
        var state = (WindowState*)userData;

        state->Messages.Add(Message.Closed());
    }

    [UnmanagedCallersOnly]
    private static void OnLibdecorFrameCommit(libdecor_frame* frame, void* userData) =>
        Logger.Debug(nameof(OnLibdecorFrameCommit));

    [UnmanagedCallersOnly]
    private static void OnLibdecorFrameConfigure(libdecor_frame* frame, libdecor_configuration* configuration, void* userData)
    {
        var state = (WindowState*)userData;

        state->PendingLibdecorConfiguration = configuration;

        int width;
        int height;

        if (!libdecor_configuration_get_content_size(configuration, frame, &width, &height))
        {
            width  = state->Size.Width;
            height = state->Size.Height;
        }

        if (width == 0 || height == 0)
        {
            throw new InvalidOperationException("Window has an invalid size");
        }

        var windowState = libdecor_window_state.LIBDECOR_WINDOW_STATE_NONE;

        state->Mode      = WindowMode.Windowed;
        state->Suspended = false;

        if (libdecor_configuration_get_window_state(configuration, &windowState))
        {
            if (windowState.HasFlags(libdecor_window_state.LIBDECOR_WINDOW_STATE_MAXIMIZED))
            {
                state->Mode = WindowMode.Maximized;
            }

            if (windowState.HasFlags(libdecor_window_state.LIBDECOR_WINDOW_STATE_FULLSCREEN))
            {
                state->Mode = WindowMode.Fullscreen;
            }

            if (windowState.HasFlags(libdecor_window_state.LIBDECOR_WINDOW_STATE_SUSPENDED))
            {
                state->Suspended = true;
            }
        }

        UpdateSize(state, new(width, height));
    }

    [UnmanagedCallersOnly]
    private static void OnLibdecorFrameDismissPopup(libdecor_frame* frame, byte* seatName, void* userData) =>
        Logger.Debug(nameof(OnLibdecorFrameDismissPopup));

    [UnmanagedCallersOnly]
    private static void OnOutputDescription(void* data, wl_output* output, byte* description) =>
        Console.WriteLine(nameof(OnOutputDescription));

    [UnmanagedCallersOnly]
    private static void OnOutputDone(void* data, wl_output* output) =>
        Console.WriteLine(nameof(OnOutputDone));

    [UnmanagedCallersOnly]
    private static void OnOutputGeometry(void* data, wl_output* output, int x, int y, int physicalWidth, int physicalHeight, int subpixel, byte* make, byte* model, int transform) =>
        Console.WriteLine(nameof(OnOutputGeometry));

    [UnmanagedCallersOnly]
    private static void OnOutputMode(void* data, wl_output* output, uint flags, int width, int height, int refresh) =>
        Console.WriteLine(nameof(OnOutputMode));

    [UnmanagedCallersOnly]
    private static void OnOutputName(void* data, wl_output* output, byte* name) =>
        Console.WriteLine(nameof(OnOutputName));

    [UnmanagedCallersOnly]
    private static void OnOutputScale(void* data, wl_output* output, int factor) =>
        Console.WriteLine(nameof(OnOutputScale));

    [UnmanagedCallersOnly]
    private static void OnTextInputCommitString(void* data, zwp_text_input_v3* textInput, byte* text) =>
        Console.WriteLine(nameof(OnTextInputCommitString));

    [UnmanagedCallersOnly]
    private static void OnTextInputDeleteSurroundingText(void* data, zwp_text_input_v3* textInput, uint beforeLength, uint afterLength) =>
        Console.WriteLine(nameof(OnTextInputDeleteSurroundingText));

    [UnmanagedCallersOnly]
    private static void OnTextInputDone(void* data, zwp_text_input_v3* textInput, uint serial) =>
        Console.WriteLine(nameof(OnTextInputDone));

    [UnmanagedCallersOnly]
    private static void OnTextInputEnter(void* data, zwp_text_input_v3* textInput, wl_surface* surface) =>
        Console.WriteLine(nameof(OnTextInputEnter));

    [UnmanagedCallersOnly]
    private static void OnTextInputLeave(void* data, zwp_text_input_v3* textInput, wl_surface* surface) =>
        Console.WriteLine(nameof(OnTextInputLeave));

    [UnmanagedCallersOnly]
    private static void OnTextInputPreeditString(void* data, zwp_text_input_v3* textInput, byte* text, int cursorBegin, int cursorEnd) =>
        Console.WriteLine(nameof(OnTextInputPreeditString));

    [UnmanagedCallersOnly]
    private static void OnRegistryGlobal(void* data, wl_registry* registry, uint name, byte* @interface, uint version)
    {
        if (string.Compare(@interface, wl_shm_interface->name))
        {
            shm = new(name, (wl_shm*)wl_registry_bind(registry, name, wl_shm_interface, Math.Clamp(version, 1, 6)));

            return;
        }

        if (string.Compare(@interface, wl_compositor_interface->name))
        {
            compositor = new(name, (wl_compositor*)wl_registry_bind(registry, name, wl_compositor_interface, Math.Clamp(version, 1, 6)));

            return;
        }

        if (string.Compare(@interface, wl_data_device_manager_interface->name))
        {
            dataDeviceManager = new(name, (wl_data_device_manager*)wl_registry_bind(registry, name, wl_data_device_manager_interface, Math.Clamp(version, 1, 6)));

            foreach (var seat in seats)
            {
                var state = GetSeatState(seat);

                if (state->DataDevice == default)
                {
                    state->DataDevice = wl_data_device_manager_get_data_device(dataDeviceManager, seat);

                    fixed (wl_data_device_listener* pDataDeviceListener = &dataDeviceListener)
                    {
                        wl_data_device_add_listener(state->DataDevice, pDataDeviceListener, null);
                    }
                }
            }

            return;
        }

        if (string.Compare(@interface, wl_output_interface->name))
        {
            var output = (wl_output*)wl_registry_bind(registry, name, wl_output_interface, Math.Clamp(version, 1, 4));

            outputs.Add(output);

            var screenState = NativeMemory.Alloc<ScreenState>();

            screenStates[name] = screenState;

            SetProxyTag((wl_proxy*)output);

            fixed (wl_output_listener* pOutputListener = &outputListener)
            {
                wl_output_add_listener(output, pOutputListener, screenState);
            }
        }

        if (string.Compare(@interface, wl_seat_interface->name))
        {
            var seat = (wl_seat*)wl_registry_bind(registry, name, wl_seat_interface, Math.Clamp(version, 1, 9));

            SetProxyTag((wl_proxy*)seat);

            var seatState = NativeMemory.AllocSet(
                new SeatState
                {
                    Seat = new(name, seat),
                }
            );

            if (seatState->DataDevice == default && dataDeviceManager != default)
            {
                // Clipboard & DnD.
                seatState->DataDevice = wl_data_device_manager_get_data_device(dataDeviceManager, seat);

                fixed (wl_data_device_listener* pDataDeviceListener = &dataDeviceListener)
                {
                    wl_data_device_add_listener(seatState->DataDevice, pDataDeviceListener, seatState);
                }
            }

            if (seatState->PrimarySelectionDevice == default && primarySelectionDeviceManager != default)
            {
                // Primary selection.
                seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(primarySelectionDeviceManager, seat);

                fixed (zwp_primary_selection_device_v1_listener* pPrimarySelectionDeviceListener = &primarySelectionDeviceListener)
                {
                    zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, pPrimarySelectionDeviceListener, seatState);
                }
            }

            if (seatState->TextInput == default && textInputManager != default) {
                // IME.
                seatState->TextInput = zwp_text_input_manager_v3_get_text_input(textInputManager, seat);

                fixed (zwp_text_input_v3_listener* pTextInputListener = &textInputListener)
                {
                    zwp_text_input_v3_add_listener(seatState->TextInput, pTextInputListener, seatState);
                }
            }

            seats.Add(seat);

            fixed (wl_seat_listener* pSeatListener = &seatListener)
            {
                wl_seat_add_listener(seat, pSeatListener, seatState);
            }

            if (currentSeat == default)
            {
			    currentSeat = seat;
		    }

            return;
        }

        if (string.Compare(@interface, xdg_wm_base_interface->name))
        {
            wmBase = new(name, (xdg_wm_base*)wl_registry_bind(registry, name, xdg_wm_base_interface, Math.Clamp(version, 1, 6)));

            fixed (xdg_wm_base_listener* pWmBaseListener = &wmBaseListener)
            {
                xdg_wm_base_add_listener(wmBase, pWmBaseListener, null);
            }

            return;
        }

        if (string.Compare(@interface, wp_viewporter_interface->name))
        {
            viewporter = new(name, (wp_viewporter*)wl_registry_bind(registry, name, wp_viewporter_interface, 1));

            return;
        }

        if (string.Compare(@interface, wp_cursor_shape_manager_v1_interface->name))
        {
            cursorShapeManager = new(name, (wp_cursor_shape_manager_v1*)wl_registry_bind(registry, name, wp_cursor_shape_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, wp_fractional_scale_manager_v1_interface->name))
        {
            fractionalScaleManager = new(name, (wp_fractional_scale_manager_v1*)wl_registry_bind(registry, name, wp_fractional_scale_manager_v1_interface, 1));

            return;

            // NOTE: We're not mapping the fractional scale object here because this is
            // supposed to be a "startup global". If for some reason this isn't true (who
            // knows), add a conditional branch for creating the add-on object.
        }

        if (string.Compare(@interface, zxdg_decoration_manager_v1_interface->name))
        {
            decorationManager = new(name, (zxdg_decoration_manager_v1*)wl_registry_bind(registry, name, zxdg_decoration_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, xdg_system_bell_v1_interface->name))
        {
            systemBell = new(name, (xdg_system_bell_v1*)wl_registry_bind(registry, name, xdg_system_bell_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, xdg_activation_v1_interface->name))
        {
            activation = new(name, (xdg_activation_v1*)wl_registry_bind(registry, name, xdg_activation_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_primary_selection_device_manager_v1_interface->name))
        {
            primarySelectionDeviceManager = (zwp_primary_selection_device_manager_v1*)wl_registry_bind(registry, name, zwp_primary_selection_device_manager_v1_interface, 1);

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ValidateNotNull(seatState);

                if (seatState->PrimarySelectionDevice == default)
                {
                    seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(primarySelectionDeviceManager, seat);

                    fixed (zwp_primary_selection_device_v1_listener* pPrimarySelectionDeviceListener = &primarySelectionDeviceListener)
                    {
                        zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, pPrimarySelectionDeviceListener, seatState);
                    }
                }
            }
        }

        if (string.Compare(@interface, zwp_relative_pointer_manager_v1_interface->name))
        {
            relativePointerManager = new(name, (zwp_relative_pointer_manager_v1*)wl_registry_bind(registry, name, zwp_relative_pointer_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_pointer_constraints_v1_interface->name))
        {
            pointerConstraints = new(name, (zwp_pointer_constraints_v1*)wl_registry_bind(registry, name, zwp_pointer_constraints_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_pointer_gestures_v1_interface->name))
        {
            pointerGestures = new(name, (zwp_pointer_gestures_v1*)wl_registry_bind(registry, name, zwp_pointer_gestures_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_idle_inhibit_manager_v1_interface->name))
        {
            idleInhibitManager = new(name, (zwp_idle_inhibit_manager_v1*)wl_registry_bind(registry, name, zwp_idle_inhibit_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_tablet_manager_v2_interface->name))
        {
            tabletManager = new(name, (zwp_tablet_manager_v2*)wl_registry_bind(registry, name, zwp_tablet_manager_v2_interface, 1));

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ValidateNotNull(seatState);

                seatState->TabletSeat = zwp_tablet_manager_v2_get_tablet_seat(tabletManager, seat);

                fixed (zwp_tablet_seat_v2_listener* pTabletSeatListener = &tabletSeatListener)
                {
                    zwp_tablet_seat_v2_add_listener(seatState->TabletSeat, pTabletSeatListener, seatState);
                }
            }

            return;
        }

        if (string.Compare(@interface, zwp_text_input_manager_v3_interface->name))
        {
            textInputManager = new(name, (zwp_text_input_manager_v3*)wl_registry_bind(registry, name, zwp_text_input_manager_v3_interface, 1));

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ValidateNotNull(seatState);

                seatState->TextInput = zwp_text_input_manager_v3_get_text_input(textInputManager, seat);

                fixed (zwp_text_input_v3_listener* pTextInputListener = &textInputListener)
                {
                    zwp_text_input_v3_add_listener(seatState->TextInput, pTextInputListener, seatState);
                }
            }
        }
    }

    [UnmanagedCallersOnly]
    private static void OnRegistryGlobalRemove(void* data, wl_registry* registry, uint name)
    {
        if (screenStates.Remove(name, out var screenState))
        {
            NativeMemory.Free(screenState);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnWpPrimarySelectionDeviceselection(void* data, zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_offer_v1* offer) =>
        Console.WriteLine(nameof(OnWpPrimarySelectionDeviceselection));

    [UnmanagedCallersOnly]
    private static void OnWpPrimarySelectionDevicedataOffer(void* data, zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_offer_v1* id) =>
        Console.WriteLine(nameof(OnWpPrimarySelectionDevicedataOffer));

    [UnmanagedCallersOnly]
    private static void OnSeatName(void* data, wl_seat* seat, byte* name) =>
        Console.WriteLine(nameof(OnSeatName));

    [UnmanagedCallersOnly]
    private static void OnSeatCapabilities(void* data, wl_seat* seat, uint* capabilities) =>
        Console.WriteLine(nameof(OnSeatCapabilities));

    [UnmanagedCallersOnly]
    private static void OnSurfaceEnter(void* data, wl_surface* surface, wl_output* output) =>
        Console.WriteLine(nameof(OnSurfaceEnter));

    [UnmanagedCallersOnly]
    private static void OnSurfaceLeave(void* data, wl_surface* surface, wl_output* output) =>
        Console.WriteLine(nameof(OnSurfaceLeave));

    [UnmanagedCallersOnly]
    private static void OnSurfacePreferredBufferScale(void* data, wl_surface* surface, int factor) =>
        Console.WriteLine(nameof(OnSurfacePreferredBufferScale));

    [UnmanagedCallersOnly]
    private static void OnSurfacePreferredBufferTransform(void* data, wl_surface* surface, int transform) =>
        Console.WriteLine(nameof(OnSurfacePreferredBufferTransform));

    [UnmanagedCallersOnly]
    private static void OnTabletSeatPadAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_pad_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatPadAdded));

    [UnmanagedCallersOnly]
    private static void OnTabletSeatTabletAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatTabletAdded));

    [UnmanagedCallersOnly]
    private static void OnTabletSeatToolAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_tool_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatToolAdded));

    [UnmanagedCallersOnly]
    private static void OnWmBasePing(void* data, xdg_wm_base* wmBase, uint serial) =>
        Console.WriteLine(nameof(OnWmBasePing));
    #endregion

    private static void EventLoop()
    {
        var poolFd = new pollfd
        {
            fd     = wl_display_get_fd(display),
            events = POLLIN | POLLHUP
        };

        while (true)
        {
            while (wl_display_prepare_read(display) != 0)
            {
                lock (@lock)
                {
                    if (wl_display_dispatch_pending(display) == -1)
                    {
                        break;
                    }
                }
            }

            var werror = wl_display_get_error(display);

            if (werror > 0)
            {
                if (werror == EPROTO)
                {
                    wl_interface* @interface;
                    uint id;

                    var error_code = wl_display_get_protocol_error(display, &@interface, &id);

                    var insterfaceName = Encoding.GetStringFromNullTerminated(@interface->name) ?? "unknown";

                    throw new Exception($"Wayland protocol error %d on interface {insterfaceName}@{id}.");
                }
                else
                {
                    throw new Exception($"Wayland client error code {werror}.");
                }
            }

            _ = wl_display_flush(display);

            _ = poll(&poolFd, 1, -1);

            if (destroyed)
            {
                wl_display_cancel_read(display);

                break;
            }

            if ((poolFd.revents | POLLIN) != 0)
            {
                _ = wl_display_read_events(display);
            }
            else
            {
                wl_display_cancel_read(display);
            }

            lock (@lock)
            {
                _ = wl_display_dispatch_pending(display);
            }
        }
    }

    private static void UpdateSize(WindowState* state, Size<int> size)
    {
        var sizeHasChanged = false;

        if (state->Size != size)
        {
            state->Size = size;

            sizeHasChanged = true;
        }

        if (state->Surface != null && state->Viewport != null)
        {
            wp_viewport_set_destination(state->Viewport, size.Width, size.Height);
        }

        var libdecorState = libdecor_state_new(size.Width, size.Height);

        libdecor_frame_commit(state->Frame, libdecorState, state->PendingLibdecorConfiguration);
        libdecor_state_free(libdecorState);

        if (sizeHasChanged)
        {
            state->Messages.Add(Message.Resized());

            if (MOCK_BUFFER)
            {
                MockBuffer(state);
            }
        }

        state->PendingLibdecorConfiguration = null;
    }

    private partial void UpdateCursor() => throw new NotImplementedException();

    public static partial void Register(string appId)
    {
        Window.appId = appId;

        using var uAppId = new UnmanagedString(appId);

        display = wl_display_connect(null);

        NullReferenceException.ValidateNotNull(display, "Can't connect to a Wayland display.");

        eventLoopThread.Start();

        registry = wl_display_get_registry(display);

        NullReferenceException.ValidateNotNull(registry, "Can't obtain the Wayland registry global.");

        fixed (wl_registry_listener* pRegistryListener = &registryListener)
        {
            wl_registry_add_listener(registry, pRegistryListener, null);
        }

        _ = wl_display_roundtrip(display);

        NullReferenceException.ValidateNotNull(shm, "Can't obtain the Wayland shared memory global.");
	    NullReferenceException.ValidateNotNull(compositor, "Can't obtain the Wayland compositor global.");
	    NullReferenceException.ValidateNotNull(wmBase, "Can't obtain the Wayland XDG shell global.");

        fixed (libdecor_interface* pLibdecorInterface = &libdecorInterface)
        {
            libdecorContext = libdecor_new(display, pLibdecorInterface);
        }

        NullReferenceException.ValidateNotNull(libdecorContext, "Can't create libdecor Context.");
    }

    public static partial void Destroy()
    {
        destroyed = true;

        _ = wl_display_roundtrip(display);

        eventLoopThread.Join();

        foreach (var window in Windows)
        {
            window.Close();
        }

        foreach (var seat in seats)
        {
            var seatState = GetSeatState(seat);

            wl_seat_destroy(seat);

            // if (seatState->Pointer)
            // {
			//     wl_pointer_destroy(seatState->Pointer);
		    // }

            // if (seatState->CursorFrameCallback) {
            //     // We don't need to set a null userdata for safety as the thread is done.
            //     wl_callback_destroy(seatState->CursorFrameCallback);
            // }

            // if (seatState->CursorSurface)
            // {
            //     wl_surface_destroy(seatState->CursorSurface);
            // }

            // if (seatState->WlDataDevice)
            // {
            //     wl_data_device_destroy(seatState->WlDataDevice);
            // }

            // if (seatState->WpCursorShapeDevice)
            // {
            //     wp_cursor_shape_device_v1_destroy(seatState->WpCursorShapeDevice);
            // }

            // if (seatState->WpRelativePointer)
            // {
            //     zwp_relative_pointer_v1_destroy(seatState->WpRelativePointer);
            // }

            // if (seatState->WpLockedPointer)
            // {
            //     zwp_locked_pointer_v1_destroy(seatState->WpLockedPointer);
            // }

            // if (seatState->WpConfinedPointer)
            // {
            //     zwp_confined_pointer_v1_destroy(seatState->WpConfinedPointer);
            // }

            // if (seatState->WpTabletSeat)
            // {
            //     zwp_tablet_seat_v2_destroy(seatState->WpTabletSeat);
            // }

            NativeMemory.Free(seatState);
        }

        foreach (var output in outputs)
        {
            wl_output_destroy(output);
        }

		// wl_cursor_theme_destroy(cursorTheme);
		if (idleInhibitManager != default)
        {
            zwp_idle_inhibit_manager_v1_destroy(idleInhibitManager);
        }

        if (pointerConstraints != default)
        {
            zwp_pointer_constraints_v1_destroy(pointerConstraints);
        }

        if (pointerGestures != default)
        {
            zwp_pointer_gestures_v1_destroy(pointerGestures);
        }

        if (relativePointerManager != default)
        {
            zwp_relative_pointer_manager_v1_destroy(relativePointerManager);
        }

        if (activation != default)
        {
            xdg_activation_v1_destroy(activation);
        }

        if (systemBell != default)
        {
            xdg_system_bell_v1_destroy(systemBell);
        }

        if (decorationManager != default)
        {
            zxdg_decoration_manager_v1_destroy(decorationManager);
        }

        if (cursorShapeManager != default)
        {
            wp_cursor_shape_manager_v1_destroy(cursorShapeManager);
        }

        if (fractionalScaleManager != default)
        {
            wp_fractional_scale_manager_v1_destroy(fractionalScaleManager);
        }

        if (viewporter != default)
        {
            wp_viewporter_destroy(viewporter);
        }

        if (wmBase != default)
        {
            xdg_wm_base_destroy(wmBase);
        }

        if (shm != default)
        {
            wl_shm_destroy(shm);
        }

        if (compositor != default)
        {
            wl_compositor_destroy(compositor);
        }

        if (registry != default)
        {
            wl_registry_destroy(registry);
        }

        if (display != default)
        {
            wl_display_disconnect(display);
        }
    }

    public partial void Close()
    {
        if (!this.IsClosed)
        {
            this.IsClosed = true;

            this.Closed?.Invoke();

            foreach (var child in this.Children)
            {
                child.IsClosed = true;

                WindowsMap.Remove(child.Handle);

                child.Closed?.Invoke();
            }

            wp_viewport_destroy(this.state->Viewport);
            wl_surface_destroy(this.state->Surface);

            this.state->Dispose();

            NativeMemory.Free(this.state);

            WindowsMap.Remove(this.Handle);

            this.Parent?.Children.Remove(this);
        }
    }

    public partial void DoEvents()
    {
        lock (@lock)
        {
            this.windowChanges = default;

            foreach (var message in this.state->Messages)
            {
                switch (message.Kind)
                {
                    case MessageKind.Click:
                        this.Click?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.Closed:
                        this.windowChanges |= WindowChanges.Close;

                        break;

                    case MessageKind.Context:
                        this.Context?.Invoke(message.Value.WindowContextEvent);

                        break;

                    case MessageKind.DoubleClick:
                        this.DoubleClick?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.Input:
                        this.Input?.Invoke(message.Value.Input);

                        break;

                    case MessageKind.KeyPress:
                        this.KeyPress?.Invoke(message.Value.Key);

                        break;

                    case MessageKind.KeyDown:
                        this.KeyDown?.Invoke(message.Value.Key);

                        break;

                    case MessageKind.KeyUp:
                        this.KeyUp?.Invoke(message.Value.Key);

                        break;

                    case MessageKind.MouseDown:
                        this.MouseDown?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.MouseMove:
                        this.MouseMove?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.MouseUp:
                        this.MouseUp?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.MouseWheel:
                        this.MouseWheel?.Invoke(message.Value.MouseEvent);

                        break;

                    case MessageKind.Resized:
                        this.windowChanges |= WindowChanges.Size;

                        break;
                }
            }

            this.state->Messages.Clear();

            if (this.windowChanges.HasFlags(WindowChanges.Close))
            {
                this.Close();

                return;
            }

            if (this.windowChanges.HasFlags(WindowChanges.Size))
            {
                this.Resized?.Invoke();
            }
        }
    }

    public partial string? GetClipboardData() => throw new NotImplementedException();
    public partial void Hide() => throw new NotImplementedException();
    public partial void Maximize() => throw new NotImplementedException();
    public partial void Minimize() => throw new NotImplementedException();
    public partial void Restore() => throw new NotImplementedException();
    public partial void SetClipboardData(string value) => throw new NotImplementedException();
    public partial void Show() => throw new NotImplementedException();
}
#endif
