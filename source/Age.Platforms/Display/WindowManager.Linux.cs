#if LINUX
using Age.Core.Exceptions;
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
using Age.Core.Collections;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    #region Unmanaged Listeners
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

    private readonly static byte** pTag = (byte**)NativeMemory.Alloc((nint)MemoryMarshal.CreateUTF8StringBuffer("Age"));

    private readonly Lock           @lock = new();
    private readonly Thread         eventLoopThread;
    private readonly RegistryState* registryState = NativeMemory.Alloc(new RegistryState());

    private bool stopped;

    public nint                 Display => (nint)this.registryState->Display;

    public partial WindowManager(string id)
    {
        SingletonViolationException.ThrowIfNoSingleton(Instance);

        Instance = this;

        this.Id = id;

        var display = this.registryState->Display = wl_display_connect(null);

        NullReferenceException.ThrowIfNull(display, "Can't connect to a Wayland display.");

        this.eventLoopThread = new(this.EventLoop);
        this.eventLoopThread.Start();

        var registry = this.registryState->Registry = wl_display_get_registry(display);

        NullReferenceException.ThrowIfNull(registry, "Can't obtain the Wayland registry global.");

        fixed (wl_registry_listener* pRegistryListener = &registryListener)
        {
            wl_registry_add_listener(registry, pRegistryListener, this.registryState);
        }

        _ = wl_display_roundtrip(display);

        NullReferenceException.ThrowIfNull(this.registryState->Shm, "Can't obtain the Wayland shared memory global.");
	    NullReferenceException.ThrowIfNull(this.registryState->Compositor, "Can't obtain the Wayland compositor global.");
	    NullReferenceException.ThrowIfNull(this.registryState->WmBase, "Can't obtain the Wayland XDG shell global.");

        fixed (libdecor_interface* pLibdecorInterface = &libdecorInterface)
        {
            var libdecorContext = this.registryState->LibdecorContext = libdecor_new(display, pLibdecorInterface);

            NullReferenceException.ThrowIfNull(libdecorContext, "Can't create libdecor Context.");
        }
    }

    private static SeatState* GetSeatState(wl_seat* seat) =>
        seat != null && ProxyIsAge((wl_proxy*)seat) ? (SeatState*)wl_seat_get_user_data(seat) : default;

    #region Unmanaged Callers
    [UnmanagedCallersOnly]
    private static void OnBufferRelease(void* data, wl_buffer* buffer)
    {
        var bufferData = (BufferData*)data;

        wl_buffer_destroy(buffer);

        _ = munmap(bufferData->Data, bufferData->Size);

        NativeMemory.Free(bufferData);
    }

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

        state->AddMessage(WindowMessage.Closed());
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
        var registryState = (RegistryState*)data;

        if (string.Compare(@interface, wl_shm_interface->name))
        {
            registryState->Shm = new(name, (wl_shm*)wl_registry_bind(registry, name, wl_shm_interface, Math.Clamp(version, 1, 6)));

            return;
        }

        if (string.Compare(@interface, wl_compositor_interface->name))
        {
            registryState->Compositor = new(name, (wl_compositor*)wl_registry_bind(registry, name, wl_compositor_interface, Math.Clamp(version, 1, 6)));

            return;
        }

        if (string.Compare(@interface, wl_data_device_manager_interface->name))
        {
            registryState->DataDeviceManager = new(name, (wl_data_device_manager*)wl_registry_bind(registry, name, wl_data_device_manager_interface, Math.Clamp(version, 1, 6)));

            using var seats = registryState->GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                if (seatState->DataDevice == default)
                {
                    seatState->DataDevice = wl_data_device_manager_get_data_device(registryState->DataDeviceManager, seat);

                    fixed (wl_data_device_listener* pDataDeviceListener = &dataDeviceListener)
                    {
                        wl_data_device_add_listener(seatState->DataDevice, pDataDeviceListener, null);
                    }
                }
            }

            return;
        }

        if (string.Compare(@interface, wl_output_interface->name))
        {
            var output = (wl_output*)wl_registry_bind(registry, name, wl_output_interface, Math.Clamp(version, 1, 4));

            registryState->AddOutput(output);

            var screenState = NativeMemory.Alloc<ScreenState>();

            registryState->AddScreenState(name, screenState);

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

            var seatState = NativeMemory.Alloc(
                new SeatState
                {
                    Seat = new(name, seat),
                }
            );

            if (seatState->DataDevice == default && registryState->DataDeviceManager != default)
            {
                // Clipboard & DnD.
                seatState->DataDevice = wl_data_device_manager_get_data_device(registryState->DataDeviceManager, seat);

                fixed (wl_data_device_listener* pDataDeviceListener = &dataDeviceListener)
                {
                    wl_data_device_add_listener(seatState->DataDevice, pDataDeviceListener, seatState);
                }
            }

            if (seatState->PrimarySelectionDevice == default && registryState->PrimarySelectionDeviceManager != default)
            {
                // Primary selection.
                seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(registryState->PrimarySelectionDeviceManager, seat);

                fixed (zwp_primary_selection_device_v1_listener* pPrimarySelectionDeviceListener = &primarySelectionDeviceListener)
                {
                    zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, pPrimarySelectionDeviceListener, seatState);
                }
            }

            if (seatState->TextInput == default && registryState->TextInputManager != default) {
                // IME.
                seatState->TextInput = zwp_text_input_manager_v3_get_text_input(registryState->TextInputManager, seat);

                fixed (zwp_text_input_v3_listener* pTextInputListener = &textInputListener)
                {
                    zwp_text_input_v3_add_listener(seatState->TextInput, pTextInputListener, seatState);
                }
            }

            registryState->AddSeat(seat);

            fixed (wl_seat_listener* pSeatListener = &seatListener)
            {
                wl_seat_add_listener(seat, pSeatListener, seatState);
            }

            if (registryState->CurrentSeat == default)
            {
			    registryState->CurrentSeat = seat;
		    }

            return;
        }

        if (string.Compare(@interface, xdg_wm_base_interface->name))
        {
            registryState->WmBase = new(name, (xdg_wm_base*)wl_registry_bind(registry, name, xdg_wm_base_interface, Math.Clamp(version, 1, 6)));

            fixed (xdg_wm_base_listener* pWmBaseListener = &wmBaseListener)
            {
                xdg_wm_base_add_listener(registryState->WmBase, pWmBaseListener, null);
            }

            return;
        }

        if (string.Compare(@interface, wp_viewporter_interface->name))
        {
            registryState->Viewporter = new(name, (wp_viewporter*)wl_registry_bind(registry, name, wp_viewporter_interface, 1));

            return;
        }

        if (string.Compare(@interface, wp_cursor_shape_manager_v1_interface->name))
        {
            registryState->CursorShapeManager = new(name, (wp_cursor_shape_manager_v1*)wl_registry_bind(registry, name, wp_cursor_shape_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, wp_fractional_scale_manager_v1_interface->name))
        {
            registryState->FractionalScaleManager = new(name, (wp_fractional_scale_manager_v1*)wl_registry_bind(registry, name, wp_fractional_scale_manager_v1_interface, 1));

            return;

            // NOTE: We're not mapping the fractional scale object here because this is
            // supposed to be a "startup global". If for some reason this isn't true (who
            // knows), add a conditional branch for creating the add-on object.
        }

        if (string.Compare(@interface, zxdg_decoration_manager_v1_interface->name))
        {
            registryState->DecorationManager = new(name, (zxdg_decoration_manager_v1*)wl_registry_bind(registry, name, zxdg_decoration_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, xdg_system_bell_v1_interface->name))
        {
            registryState->SystemBell = new(name, (xdg_system_bell_v1*)wl_registry_bind(registry, name, xdg_system_bell_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, xdg_activation_v1_interface->name))
        {
            registryState->Activation = new(name, (xdg_activation_v1*)wl_registry_bind(registry, name, xdg_activation_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_primary_selection_device_manager_v1_interface->name))
        {
            registryState->PrimarySelectionDeviceManager = (zwp_primary_selection_device_manager_v1*)wl_registry_bind(registry, name, zwp_primary_selection_device_manager_v1_interface, 1);

            using var seats = registryState->GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ThrowIfNull(seatState);

                if (seatState->PrimarySelectionDevice == default)
                {
                    seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(registryState->PrimarySelectionDeviceManager, seat);

                    fixed (zwp_primary_selection_device_v1_listener* pPrimarySelectionDeviceListener = &primarySelectionDeviceListener)
                    {
                        zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, pPrimarySelectionDeviceListener, seatState);
                    }
                }
            }
        }

        if (string.Compare(@interface, zwp_relative_pointer_manager_v1_interface->name))
        {
            registryState->RelativePointerManager = new(name, (zwp_relative_pointer_manager_v1*)wl_registry_bind(registry, name, zwp_relative_pointer_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_pointer_constraints_v1_interface->name))
        {
            registryState->PointerConstraints = new(name, (zwp_pointer_constraints_v1*)wl_registry_bind(registry, name, zwp_pointer_constraints_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_pointer_gestures_v1_interface->name))
        {
            registryState->PointerGestures = new(name, (zwp_pointer_gestures_v1*)wl_registry_bind(registry, name, zwp_pointer_gestures_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_idle_inhibit_manager_v1_interface->name))
        {
            registryState->IdleInhibitManager = new(name, (zwp_idle_inhibit_manager_v1*)wl_registry_bind(registry, name, zwp_idle_inhibit_manager_v1_interface, 1));

            return;
        }

        if (string.Compare(@interface, zwp_tablet_manager_v2_interface->name))
        {
            registryState->TabletManager = new(name, (zwp_tablet_manager_v2*)wl_registry_bind(registry, name, zwp_tablet_manager_v2_interface, 1));

            using var seats = registryState->GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ThrowIfNull(seatState);

                seatState->TabletSeat = zwp_tablet_manager_v2_get_tablet_seat(registryState->TabletManager, seat);

                fixed (zwp_tablet_seat_v2_listener* pTabletSeatListener = &tabletSeatListener)
                {
                    zwp_tablet_seat_v2_add_listener(seatState->TabletSeat, pTabletSeatListener, seatState);
                }
            }

            return;
        }

        if (string.Compare(@interface, zwp_text_input_manager_v3_interface->name))
        {
            registryState->TextInputManager = new(name, (zwp_text_input_manager_v3*)wl_registry_bind(registry, name, zwp_text_input_manager_v3_interface, 1));

            using var seats = registryState->GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                NullReferenceException.ThrowIfNull(seatState);

                seatState->TextInput = zwp_text_input_manager_v3_get_text_input(registryState->TextInputManager, seat);

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
        var registryState = (RegistryState*)data;

        if (registryState->RemoveScreenState(name, out var screenState))
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

    private static bool ProxyIsAge(wl_proxy* proxy)
    {
        NullReferenceException.ThrowIfNull(proxy);

        return wl_proxy_get_tag(proxy) == pTag;
    }

    private static void SetProxyTag(wl_proxy* proxy)
    {
        NullReferenceException.ThrowIfNull(proxy);

        wl_proxy_set_tag(proxy, pTag);
    }

    private void EventLoop()
    {
        var display = this.registryState->Display;

        var poolFd = new pollfd
        {
            fd     = wl_display_get_fd(display),
            events = POLLIN | POLLHUP
        };

        while (true)
        {
            while (wl_display_prepare_read(display) != 0)
            {
                lock (this.@lock)
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

            if (this.stopped)
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

            lock (this.@lock)
            {
                _ = wl_display_dispatch_pending(display);
            }
        }
    }

    protected override partial void OnDisposed(bool disposing)
    {
        this.stopped = true;

        _ = wl_display_roundtrip(this.registryState->Display);

        this.eventLoopThread.Join();

        using var seats = this.registryState->GetSeats();

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

        using var outputs = this.registryState->GetOutputs();

        foreach (var output in outputs)
        {
            wl_output_destroy(output);
        }

		// wl_cursor_theme_destroy(cursorTheme);
		if (this.registryState->IdleInhibitManager != default)
        {
            zwp_idle_inhibit_manager_v1_destroy(this.registryState->IdleInhibitManager);
        }

        if (this.registryState->PointerConstraints != default)
        {
            zwp_pointer_constraints_v1_destroy(this.registryState->PointerConstraints);
        }

        if (this.registryState->PointerGestures != default)
        {
            zwp_pointer_gestures_v1_destroy(this.registryState->PointerGestures);
        }

        if (this.registryState->RelativePointerManager != default)
        {
            zwp_relative_pointer_manager_v1_destroy(this.registryState->RelativePointerManager);
        }

        if (this.registryState->Activation != default)
        {
            xdg_activation_v1_destroy(this.registryState->Activation);
        }

        if (this.registryState->SystemBell != default)
        {
            xdg_system_bell_v1_destroy(this.registryState->SystemBell);
        }

        if (this.registryState->DecorationManager != default)
        {
            zxdg_decoration_manager_v1_destroy(this.registryState->DecorationManager);
        }

        if (this.registryState->CursorShapeManager != default)
        {
            wp_cursor_shape_manager_v1_destroy(this.registryState->CursorShapeManager);
        }

        if (this.registryState->FractionalScaleManager != default)
        {
            wp_fractional_scale_manager_v1_destroy(this.registryState->FractionalScaleManager);
        }

        if (this.registryState->Viewporter != default)
        {
            wp_viewporter_destroy(this.registryState->Viewporter);
        }

        if (this.registryState->WmBase != default)
        {
            xdg_wm_base_destroy(this.registryState->WmBase);
        }

        if (this.registryState->Shm != default)
        {
            wl_shm_destroy(this.registryState->Shm);
        }

        if (this.registryState->Compositor != default)
        {
            wl_compositor_destroy(this.registryState->Compositor);
        }

        if (this.registryState->Registry != default)
        {
            wl_registry_destroy(this.registryState->Registry);
        }

        if (this.registryState->Display != default)
        {
            wl_display_disconnect(this.registryState->Display);
        }

        this.registryState->Dispose();

        NativeMemory.Free(this.registryState);
    }

    internal partial void CloseWindow(Window window)
    {
        wp_viewport_destroy(window.State->Viewport);
        wl_surface_destroy(window.State->Surface);

        window.State->Dispose();

        NativeMemory.Free(window.State);
    }

    internal partial WindowState* CreateWindow(string title, Size<int> size, Window? parent)
    {
        var state = NativeMemory.Alloc(
            new WindowState
            {
                Surface = wl_compositor_create_surface(this.registryState->Compositor),
                Size    = size.Cast<int>(),
            }
        );

        SetProxyTag((wl_proxy*)state->Surface);

        fixed (wl_surface_listener* pSurfaceListener = &surfaceListener)
        {
            wl_surface_add_listener(state->Surface, pSurfaceListener, null);
        }

        if (this.registryState->Viewporter != default)
        {
            state->Viewport = wp_viewporter_get_viewport(this.registryState->Viewporter, state->Surface);

            if (this.registryState->FractionalScaleManager != default)
            {
                state->FractionalScale = wp_fractional_scale_manager_v1_get_fractional_scale(this.registryState->FractionalScaleManager, state->Surface);

                fixed (wp_fractional_scale_v1_listener* pFractionalScaleListener = &fractionalScaleListener)
                {
                    wp_fractional_scale_v1_add_listener(state->FractionalScale, pFractionalScaleListener, state);
                }
            }
        }

        fixed (libdecor_frame_interface* pFrameInterface = &frameInterface)
        {
            state->Frame = libdecor_decorate(this.registryState->LibdecorContext, state->Surface, pFrameInterface, state);
        }

        libdecor_frame_map(state->Frame);

        state->FrameCallBack = wl_surface_frame(state->Surface);

        fixed (wl_callback_listener* pFrameCallbackListener = &frameCallbackListener)
        {
            wl_callback_add_listener(state->FrameCallBack, pFrameCallbackListener, state);
        }

        wl_surface_commit(state->Surface);

        _ = wl_display_roundtrip(this.registryState->Display);

        UpdateSize(state, state->Size);

        using var uId = new UnmanagedString(this.Id);

        libdecor_frame_set_app_id(state->Frame, uId);

        return state;
    }

    internal partial NativeArray<WindowMessage> FlushWindowEvents(Window window) =>
        window.State->GetMessages();

    internal partial string? GetClipboardData(Window window) =>
        throw new NotImplementedException();

    internal partial void HideWindow(Window window) =>
        throw new NotImplementedException();

    internal partial void MaximizeWindow(Window window) =>
        throw new NotImplementedException();

    internal partial void MinimizeWindow(Window window) =>
        throw new NotImplementedException();

    internal partial void RestoreWindow(Window window) =>
        throw new NotImplementedException();

    internal partial void SetWindowClipboardData(Window window, string value) =>
        throw new NotImplementedException();

    internal partial void SetWindowTitle(Window window, string value) =>
        throw new NotImplementedException();

    internal partial void ShowWindow(Window window) =>
        throw new NotImplementedException();

    internal void UpdateCursor(Cursor cursor) =>
        throw new NotImplementedException();

    internal static void UpdateSize(WindowState* state, Size<int> size)
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
            state->AddMessage(WindowMessage.Resized());
        }

        state->PendingLibdecorConfiguration = null;
    }
}
#endif
