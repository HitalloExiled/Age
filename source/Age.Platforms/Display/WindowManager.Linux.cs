#if LINUX
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core.Exceptions;
using Age.Core;
using Age.Numerics;
using Age.Platforms.Linux.Libc;
using Age.Platforms.Linux.LibDecor;
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibWaylandCursor;
using Age.Platforms.Linux.LibXKBCommon;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ThirdParty.FreeDesktop;

using static Age.Platforms.Linux.AsmGenericErrno;
using static Age.Platforms.Linux.Libc.lib_c;
using static Age.Platforms.Linux.LibDecor.lib_decor;
using static Age.Platforms.Linux.LibWaylandClient.cursor_shape;
using static Age.Platforms.Linux.LibWaylandClient.fractional_scale;
using static Age.Platforms.Linux.LibWaylandClient.idle_inhibit;
using static Age.Platforms.Linux.LibWaylandClient.lib_wayland_client;
using static Age.Platforms.Linux.LibWaylandClient.pointer_constraints;
using static Age.Platforms.Linux.LibWaylandClient.primary_selection;
using static Age.Platforms.Linux.LibWaylandClient.relative_pointer;
using static Age.Platforms.Linux.LibWaylandClient.viewporter;
using static Age.Platforms.Linux.LibWaylandClient.xdg_activation;
using static Age.Platforms.Linux.LibWaylandClient.xdg_decoration;
using static Age.Platforms.Linux.LibWaylandClient.xdg_shell;
using static Age.Platforms.Linux.LibWaylandClient.xdg_system_bell;
using static Age.Platforms.Linux.LibWaylandCursor.lib_wayland_cursor;
using static Age.Platforms.Linux.LibXKBCommon.lib_xkbommon;
using Age.Platforms.Linux;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private static string[] cursorNames =
    [
        "left_ptr",       /// <see cref="Cursor.Arrow"/>
        "left_ptr_watch", /// <see cref="Cursor.Busy"/>
        "cross",          /// <see cref="Cursor.Cross"/>
        "size_bdiag",     /// <see cref="Cursor.DiagonalResizeNESW"/>
        "size_fdiag",     /// <see cref="Cursor.DiagonalResizeNWSE"/>
        "fleur",          /// <see cref="Cursor.Drag"/>
        "dnd-move",       /// <see cref="Cursor.Drop"/>
        "crossed_circle", /// <see cref="Cursor.Forbidden"/>
        "hand2",          /// <see cref="Cursor.Hand"/>
        "question_arrow", /// <see cref="Cursor.Help"/>
        "h_double_arrow", /// <see cref="Cursor.HorizontalResize"/>
        "col_resize",     /// <see cref="Cursor.HorizontalSplit"/>
        "move",           /// <see cref="Cursor.Move"/>
        "xterm",          /// <see cref="Cursor.Text"/>
        "v_double_arrow", /// <see cref="Cursor.VerticalResize"/>
        "row_resize",     /// <see cref="Cursor.VerticalSplit"/>
        "watch",          /// <see cref="Cursor.Wait"/>
    ];

    private static string?[] cursorNamesFallback =
    [
        null,                /// <see cref="Cursor.Arrow"/>
        "progress",          /// <see cref="Cursor.Busy"/>
        "cross",             /// <see cref="Cursor.Cross"/>
        "fd_double_arrow",   /// <see cref="Cursor.DiagonalResizeNESW"/>
        "bd_double_arrow",   /// <see cref="Cursor.DiagonalResizeNWSE"/>
        "grabbing",          /// <see cref="Cursor.Drag"/>
        "hand1",             /// <see cref="Cursor.Drop"/>
        "forbidden",         /// <see cref="Cursor.Forbidden"/>
        "pointer",           /// <see cref="Cursor.Hand"/>
        "help",              /// <see cref="Cursor.Help"/>
        "ew-resize",         /// <see cref="Cursor.HorizontalResize"/>
        "sb_h_double_arrow", /// <see cref="Cursor.HorizontalSplit"/>
        "fleur",             /// <see cref="Cursor.Move"/>
        null,                /// <see cref="Cursor.Text"/>
        "ns-resize",         /// <see cref="Cursor.VerticalResize"/>
        "sb_v_double_arrow", /// <see cref="Cursor.VerticalSplit"/>
        "wait",              /// <see cref="Cursor.Wait"/>
    ];

    private static readonly wp_cursor_shape_device_v1_shape[] standardCursors =
    [
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_DEFAULT,     /// <see cref="Cursor.Arrow"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_WAIT,        /// <see cref="Cursor.Busy"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_CROSSHAIR,   /// <see cref="Cursor.Cross"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_NESW_RESIZE, /// <see cref="Cursor.DiagonalResizeNESW"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_NWSE_RESIZE, /// <see cref="Cursor.DiagonalResizeNWSE"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_GRAB,        /// <see cref="Cursor.Drag"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_GRABBING,    /// <see cref="Cursor.Drop"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_NO_DROP,     /// <see cref="Cursor.Forbidden"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_POINTER,     /// <see cref="Cursor.Hand"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_HELP,        /// <see cref="Cursor.Help"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_EW_RESIZE,   /// <see cref="Cursor.HorizontalResize"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_COL_RESIZE,  /// <see cref="Cursor.HorizontalSplit"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_MOVE,        /// <see cref="Cursor.Move"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_PROGRESS,    /// <see cref="Cursor.Progress"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_TEXT,        /// <see cref="Cursor.Text"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_NS_RESIZE,   /// <see cref="Cursor.VerticalResize"/>
		wp_cursor_shape_device_v1_shape.WP_CURSOR_SHAPE_DEVICE_V1_SHAPE_ROW_RESIZE,  /// <see cref="Cursor.VerticalSplit"/>
    ];

    #region Unmanaged Listeners
    [FixedAddressValueType]
    private static readonly wl_callback_listener cursorFrameCallbackListener = new()
    {
		done = &OnCursorFrameCallbackDone,
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
    private readonly static wl_keyboard_listener keyboardListener = new()
    {
        keymap       = &OnKeyboardKeymap,
        enter        = &OnKeyboardEnter,
        leave        = &OnKeyboardLeave,
        key          = &OnKeyboardKey,
        modifiers    = &OnKeyboardModifiers,
        repeat_info  = &OnKeyboardRepeatInfo,
    };

    [FixedAddressValueType]
    private readonly static libdecor_interface libdecorInterface = new()
    {
        error = &OnLibdecorError
    };

    [FixedAddressValueType]
    private static readonly wl_pointer_listener pointerListener = new()
    {
        enter                   = &OnPointerEnter,
        leave                   = &OnPointerLeave,
        motion                  = &OnPointerMotion,
        button                  = &OnPointerButton,
        axis                    = &OnPointerAxis,
        frame                   = &OnPointerFrame,
        axis_source             = &OnPointerAxisSource,
        axis_stop               = &OnPointerAxisStop,
        axis_discrete           = &OnPointerAxisDiscrete,
        axis_value120           = &OnPointerAxisValue120,
        axis_relative_direction = &OnPointerAxisRelativeDirection,
    };

    [FixedAddressValueType]
    private static readonly zwp_primary_selection_device_v1_listener primarySelectionDeviceListener = new()
    {
        data_offer = &OnPrimarySelectionDevicedataOffer,
        selection  = &OnPrimarySelectionDeviceselection,
    };

    [FixedAddressValueType]
    private static readonly zwp_primary_selection_offer_v1_listener primarySelectionOfferListener = new()
    {
        offer = &OnPrimarySelectionOffer,
    };

    [FixedAddressValueType]
    private static readonly zwp_primary_selection_source_v1_listener primarySelectionSourceListener = new()
    {
        send      = &OnPrimarySelectionSourceSend,
        cancelled = &OnPrimarySelectionSourceCancelled,
    };

    [FixedAddressValueType]
    private static readonly wl_registry_listener registryListener = new()
    {
        global        = &OnRegistryGlobal,
        global_remove = &OnRegistryGlobalRemove,
    };

    [FixedAddressValueType]
    private static readonly zwp_relative_pointer_v1_listener relativePointerListener = new()
    {
        relative_motion = &OnRelativePointerV1RelativeMotion,
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
    private static readonly xdg_wm_base_listener wmBaseListener = new()
    {
        ping = &OnWmBasePing,
    };
    #endregion

    private readonly static byte** pTag = (byte**)NativeMemory.Alloc((nint)MemoryMarshal.CreateUTF8StringBuffer("Age"));

    private readonly Lock           @lock = new();
    private readonly Thread         eventLoopThread;
    private readonly RegistryState* registryState = RegistryState.Allocate();

    private bool stopped;

    public partial Cursor Cursor
    {
        get => this.registryState->CursorState->Cursor;
        set
        {
            if (this.registryState->CursorState->Cursor == value)
            {
                return;
            }

            this.registryState->CursorState->Cursor = value;

            this.UpdateCursor();
        }
    }

    public partial int CursorScale
    {
        get => this.registryState->CursorState->CursorScale;
        set
        {
            if (this.registryState->CursorState->CursorScale == value)
            {
                return;
            }

            this.registryState->CursorState->CursorScale = value;

            this.UpdateCursor();
        }
    }

    public partial bool CursorVisible
    {
        get => this.registryState->CursorState->CursorVisible;
        set
        {
            if (this.registryState->CursorState->CursorVisible == value)
            {
                return;
            }

            this.registryState->CursorState->CursorVisible = value;

            this.UpdateCursor();
        }
    }

    public nint Display => (nint)this.registryState->Display;

    public partial WindowManager(string id)
    {
        SingletonViolationException.ThrowIfNoSingleton(Instance);

        Instance = this;

        this.Id              = id;
        this.eventLoopThread = new(this.EventLoop);

        using var freeDesktopPortal = new FreeDesktopPortal();

        this.registryState->DoubleClikInterval = freeDesktopPortal.DoubleClick;
        this.registryState->LeftHandedMouse    = freeDesktopPortal.LeftHanded;

        var display = this.registryState->Display = wl_display_connect(null);

        NullReferenceException.ThrowIfNull(display, "Can't connect to a Wayland display.");

        fixed (libdecor_interface* pLibdecorInterface = &libdecorInterface)
        {
            var libdecorContext = this.registryState->LibdecorContext = libdecor_new(display, pLibdecorInterface);

            NullReferenceException.ThrowIfNull(libdecorContext, "Can't create libdecor Context.");
        }

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

        if (Environment.GetEnvironmentVariable("XCURSOR_THEME") is string cursorTheme)
        {
            this.registryState->CursorThemeName = MemoryMarshal.CreateUTF8StringBuffer(cursorTheme);
        }

        if (Environment.GetEnvironmentVariable("XCURSOR_SIZE") is string cursorSize)
        {
            var unscaledCursorSize = int.Parse(cursorSize);

            if (unscaledCursorSize > 0)
            {
                this.registryState->UnscaledCursorSize = unscaledCursorSize;
            }
            else
            {
                Logger.Warn("Detected invalid cursor size preference, defaulting to 24.");
            }
        }

        this.LoadCursorTheme();

        this.eventLoopThread.Start();
    }

    private static SeatState* GetSeatState(wl_seat* seat) =>
        seat != null && ProxyIsAge((wl_proxy*)seat) ? (SeatState*)wl_seat_get_user_data(seat) : default;

    private static WindowState* GetWindowState(wl_surface* surface) =>
        surface != null && ProxyIsAge((wl_proxy*)surface) ? (WindowState*)wl_surface_get_user_data(surface) : default;

    private static WindowKeyEvent GetKeyEvent(KeyboardState *keyboardState, uint keycode, bool pressed)
    {
        Debug.Assert(keyboardState != null);

        var shiftedKey = KeyMapping.GetKeycode(xkb_state_key_get_one_sym(keyboardState->State, keycode));

        var plainKey = Key.None;

        uint* syms = null;

        var numSys = xkb_keymap_key_get_syms_by_level(keyboardState->Keymap, keycode, keyboardState->CurrentLayoutIndex, 0, &syms);

        if (numSys > 0 && syms != null)
        {
            plainKey = KeyMapping.GetKeycode(syms[0]);
        }

        var physicalKey = KeyMapping.GetScancode(keycode);
        var keyLocation = KeyMapping.GetLocation(keycode);
        var unicode     = xkb_state_key_get_utf32(keyboardState->State, keycode);

        var key = Key.None;

        if ((shiftedKey & Key.Special) != Key.None || (plainKey & Key.Special) != Key.None)
        {
            key = shiftedKey;
        }

        if (key == default)
        {
            key = plainKey;
        }

        if (key == default)
        {
            key = physicalKey;
        }

        if (key >= Key.A + 32 && key <= Key.Z + 32)
        {
            key -= 'a' - 'A';
        }

        if (physicalKey == default && key == default && unicode == 0)
        {
            return default;
        }

        var @char = (char)unicode;

        return new WindowKeyEvent
        {
            Char        = char.IsAscii(@char) ? (char)unicode : default,
            IsPressed   = pressed,
            Key         = key,
            Location    = keyLocation,
            Modifiers   = keyboardState->Modifiers,
            PhysicalKey = physicalKey,
        };
    }

    private static WindowKeyEvent GetUnstuckKeyEvent(KeyboardState* keyboardState, uint keycode, bool pressed, Key key)
    {
        WindowKeyEvent windowKeyEvent = default;

        if (pressed)
        {
            if (keyboardState->PressedKeycodes.TryGetValue(keycode, out var oldKey) && oldKey != key)
            {
                Logger.Warn($"{oldKey} and {key} have same keycode. Generating release event for {oldKey}");

                windowKeyEvent = GetKeyEvent(keyboardState, keycode, false);

                if (windowKeyEvent != default)
                {
                    windowKeyEvent = windowKeyEvent with { Key = oldKey };
                }
            }

            keyboardState->PressedKeycodes[keycode] = key;
        }
        else
        {
            keyboardState->PressedKeycodes.Remove(keycode);
        }

        return windowKeyEvent;
    }

    private static void HandleKeyEvent(KeyboardState* keyboardState, uint keycode, bool pressed, bool echo)
    {
        Debug.Assert(keyboardState != null);

        var lastKey = Key.None;

        var composeStatus = xkb_compose_state_get_status(keyboardState->ComposeState);

        var registryState = keyboardState->SeatState->RegistryState;

        if (pressed)
        {
            var keysym        = xkb_state_key_get_one_sym(keyboardState->State, keycode);
            var composeResult = xkb_compose_state_feed(keyboardState->ComposeState, keysym);

            composeStatus = xkb_compose_state_get_status(keyboardState->ComposeState);

            if (composeResult == xkb_compose_feed_result.XKB_COMPOSE_FEED_ACCEPTED && composeStatus == xkb_compose_status.XKB_COMPOSE_COMPOSED)
            {
                var buffer     = stackalloc byte[256];
                var bufferSize = xkb_compose_state_get_utf8(keyboardState->ComposeState, buffer, 255);

                var chatCount = Encoding.UTF8.GetCharCount(buffer, bufferSize);

                Span<char> decoded = stackalloc char[chatCount];

                Encoding.UTF8.GetChars(new Span<byte>(buffer, bufferSize), decoded);

                var windowKeyEvent = GetKeyEvent(keyboardState, keycode, pressed);

                if (windowKeyEvent != default)
                {
                    for (var i = 0; i < decoded.Length; ++i)
                    {
                        if (windowKeyEvent == default)
                        {
                            continue;
                        }

                        var composedWindowKeyEvent = windowKeyEvent with
                        {
                            Char = decoded[i],
                            Echo = echo,
                        };

                        registryState->ActiveWindow->AddMessage(WindowMessage.KeyPress(composedWindowKeyEvent));
                    }
                }
            }
        }

        if (lastKey == Key.None && composeStatus == xkb_compose_status.XKB_COMPOSE_NOTHING)
        {
            // If we continued with other compose status (e.g. XKB_COMPOSE_COMPOSING) we
            // would get the composing keys _and_ the result.
            var windowKeyEvent = GetKeyEvent(keyboardState, keycode, pressed);

            if (windowKeyEvent != default)
            {
                windowKeyEvent = windowKeyEvent with { Echo = echo };

                registryState->ActiveWindow->AddMessage(WindowMessage.KeyPress(windowKeyEvent));

                lastKey = windowKeyEvent.Key;
            }
        }

        if (lastKey != Key.None)
        {
            var unstuckKeyEvent = GetUnstuckKeyEvent(keyboardState, keycode, pressed, lastKey);

            if (unstuckKeyEvent != default)
            {
                registryState->ActiveWindow->AddMessage(WindowMessage.KeyPress(unstuckKeyEvent));
            }
        }
    }

    #region Unmanaged Callers
    [UnmanagedCallersOnly]
    private static void OnCursorFrameCallbackDone(void* data, wl_callback* callback, uint timeMs)
    {
        wl_callback_destroy(callback);

        var state = (CursorState*)data;

        Debug.Assert(state != null);

        state->CursorFrameCallback = null;

        state->CursorTimeMs = timeMs;

        UpdateCursor(state);
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
    private static void OnFrameCallbackListenerDone(void* data, wl_callback* callback, uint callbackData)
    { }

    [UnmanagedCallersOnly]
    private static void OnKeyboardEnter(void* data, wl_keyboard* pointer, uint serial, wl_surface* surface, wl_array* keys)
    {
        var seatState  = (SeatState*)data;
        var windowState = GetWindowState(surface);

        Debug.Assert(seatState != null);
        Debug.Assert(windowState != null);

        seatState->RegistryState->ActiveWindow = windowState;

        windowState->AddMessage(WindowMessage.FocusIn());
    }

    [UnmanagedCallersOnly]
    private static void OnKeyboardKey(void* data, wl_keyboard* pointer, uint serial, uint time, uint key, uint state)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var keyboardState = seatState->RegistryState->KeyboardState;

        Debug.Assert(keyboardState != null);

        // We have to add 8 to the scancode to get an XKB-compatible keycode.
        var xkbKeycode = key + 8;

        var pressed = ((wl_keyboard_key_state)state).HasFlags(wl_keyboard_key_state.WL_KEYBOARD_KEY_STATE_PRESSED);

        if (pressed)
        {
            if (xkb_keymap_key_repeats(keyboardState->Keymap, xkbKeycode) == 1)
            {
                keyboardState->LastRepeatStartMsec = DateTime.Now.Ticks;
                keyboardState->RepeatingKeycode    = xkbKeycode;
            }

            keyboardState->LastKeyPressedSerial = serial;
        }
        else if (keyboardState->RepeatingKeycode == xkbKeycode)
        {
            keyboardState->RepeatingKeycode = XKB_KEYCODE_INVALID;
        }

        HandleKeyEvent(keyboardState, xkbKeycode, pressed, false);
    }

    [UnmanagedCallersOnly]
    private static void OnKeyboardKeymap(void* data, wl_keyboard* pointer, uint format, int fd, uint size)
    {
        Debug.Assert((wl_keyboard_keymap_format)format == wl_keyboard_keymap_format.WL_KEYBOARD_KEYMAP_FORMAT_XKB_V1, "Unsupported keymap format announced from the Wayland compositor.");

        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var keyboard = seatState->RegistryState->KeyboardState;

        Debug.Assert(keyboard != null);

        if (keyboard->KeymapBuffer != default)
        {
            // We have already a mapped buffer, so we unmap it. There's no need to reset
            // its pointer or size, as we're gonna set them below.
            _ = munmap(keyboard->KeymapBuffer, (ulong)keyboard->KeymapBuffer.Length);

            keyboard->KeymapBuffer = default;
        }

        keyboard->KeymapBuffer = new((byte*)mmap(null, size, PROT_READ, MAP_PRIVATE, fd, 0), (int)size);

        xkb_keymap_unref(keyboard->Keymap);

        keyboard->Keymap = xkb_keymap_new_from_string(
            keyboard->Context,
            keyboard->KeymapBuffer,
            xkb_keymap_format.XKB_KEYMAP_FORMAT_TEXT_V1,
            xkb_keymap_compile_flags.XKB_KEYMAP_COMPILE_NO_FLAGS
        );

        xkb_state_unref(keyboard->State);

        keyboard->State = xkb_state_new(keyboard->Keymap);

        xkb_compose_table_unref(keyboard->ComposeTable);

        var locale = Environment.GetEnvironmentVariable("LC_ALL")
            ?? Environment.GetEnvironmentVariable("LC_CTYPE")
            ?? Environment.GetEnvironmentVariable("LANG")
            ?? "C";

        using var uLocale = locale.ToUnmanaged();

        keyboard->ComposeTable = xkb_compose_table_new_from_locale(
            keyboard->Context,
            uLocale,
            xkb_compose_compile_flags.XKB_COMPOSE_COMPILE_NO_FLAGS
        );

        xkb_compose_state_unref(keyboard->ComposeState);

        keyboard->ComposeState = xkb_compose_state_new(keyboard->ComposeTable, xkb_compose_state_flags.XKB_COMPOSE_STATE_NO_FLAGS);

        xkb_state_update_mask(
            keyboard->State,
            keyboard->ModsDepressed,
            keyboard->ModsLatched,
            keyboard->ModsLocked,
            0,
            0,
            keyboard->CurrentLayoutIndex
        );
    }

    [UnmanagedCallersOnly]
    private static void OnKeyboardLeave(void* data, wl_keyboard* pointer, uint serial, wl_surface* surface)
    {
        if (surface == null || !ProxyIsAge((wl_proxy*)surface))
        {
            return;
        }

        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var keyboard = seatState->RegistryState->KeyboardState;

        Debug.Assert(seatState != null);

        keyboard->RepeatingKeycode = XKB_KEYCODE_INVALID;

        if (seatState->RegistryState->ActiveWindow == null)
        {
            // We're probably on a decoration or some other third-party thing.
            return;
        }

        var windowState = seatState->RegistryState->ActiveWindow;

        Debug.Assert(windowState != null);

        windowState->AddMessage(WindowMessage.FocusOut());

        windowState = null;

        keyboard->Modifiers = default;

        if (keyboard->State != null)
        {
            xkb_state_update_mask(keyboard->State, 0, 0, 0, 0, 0, 0);
        }

        Logger.Warn($"Keyboard unfocused window {(nint)windowState}");
    }

    [UnmanagedCallersOnly]
    private static void OnKeyboardModifiers(void* data, wl_keyboard* pointer, uint serial, uint modsDepressed, uint modsLatched, uint modsLocked, uint group)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var keyboard = seatState->RegistryState->KeyboardState;

        Debug.Assert(keyboard != null);

        keyboard->ModsDepressed      = modsDepressed;
        keyboard->ModsLatched        = modsLatched;
        keyboard->ModsLocked         = modsLocked;
        keyboard->CurrentLayoutIndex = group;

        xkb_state_update_mask(
            keyboard->State,
            modsDepressed,
            modsLatched,
            modsLocked,
            0,
            0,
            group
        );

        using var shift = new UnmanagedString(XKB_MOD_NAME_SHIFT);
        using var ctrl  = new UnmanagedString(XKB_MOD_NAME_CTRL);
        using var alt   = new UnmanagedString(XKB_MOD_NAME_ALT);
        using var logo  = new UnmanagedString(XKB_MOD_NAME_LOGO);

        Debug.Assert(keyboard->Modifiers == default);

        if (xkb_state_mod_name_is_active(keyboard->State, shift, xkb_state_component.XKB_STATE_MODS_DEPRESSED) == 1)
        {
            keyboard->Modifiers |= Modifier.Shift;
        }

        if (xkb_state_mod_name_is_active(keyboard->State, ctrl, xkb_state_component.XKB_STATE_MODS_DEPRESSED) == 1)
        {
            keyboard->Modifiers |= Modifier.Ctrl;
        }

        if (xkb_state_mod_name_is_active(keyboard->State, alt, xkb_state_component.XKB_STATE_MODS_DEPRESSED) == 1)
        {
            keyboard->Modifiers |= Modifier.Alt;
        }

        if (xkb_state_mod_name_is_active(keyboard->State, logo, xkb_state_component.XKB_STATE_MODS_DEPRESSED) == 1)
        {
            keyboard->Modifiers |= Modifier.Meta;
        }

        keyboard->CurrentLayoutIndex = group;
    }

    [UnmanagedCallersOnly]
    private static void OnKeyboardRepeatInfo(void* data, wl_keyboard* pointer, int rate, int delay)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var keyboard = seatState->RegistryState->KeyboardState;

        Debug.Assert(keyboard != null);

        keyboard->RepeatKeyDelayMsec   = 1000 / rate;
        keyboard->RepeatStartDelayMsec = delay;
    }

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
    private static void OnPointerEnter(void* data, wl_pointer* pointer, uint serial, wl_surface* surface, int surfaceX, int surfaceY)
    {
        var windowState = GetWindowState(surface);

        if (windowState == null)
        {
            return;
        }

        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        seatState->RegistryState->ActiveWindow = windowState;

        Debug.Assert(cursorState->CursorSurface != null);

        cursorState->PointerEnterSerial = serial;

        var pointerData = &cursorState->PointerDataBuffer;

        pointerData->WindowState     = windowState;
        pointerData->LastWindowState = windowState;
        pointerData->Position.X    = (float)lib_wayland_client.wl_fixed_to_double(surfaceX);
        pointerData->Position.Y    = (float)lib_wayland_client.wl_fixed_to_double(surfaceY);

        UpdateCursor(cursorState);

        if (wl_pointer_get_version(pointer) < WL_POINTER_FRAME_SINCE_VERSION)
        {
            delegate* unmanaged<void*, wl_pointer*, void> onPointerFrame = &OnPointerFrame;

            onPointerFrame(data, pointer);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerLeave(void* data, wl_pointer* pointer, uint serial, wl_surface* surface)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        if (seatState->RegistryState->ActiveWindow == null)
        {
            return;
        }

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        pointerData->WindowState = null;

        seatState->RegistryState->ActiveWindow = null;

        if (wl_pointer_get_version(pointer) < WL_POINTER_FRAME_SINCE_VERSION)
        {
            delegate* unmanaged<void*, wl_pointer*, void> onPointerFrame = &OnPointerFrame;

            onPointerFrame(data, pointer);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerMotion(void* data, wl_pointer* pointer, uint time, int surfaceX, int surfaceY)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        pointerData->Position.X = (float)lib_wayland_client.wl_fixed_to_double(surfaceX);
        pointerData->Position.Y = (float)lib_wayland_client.wl_fixed_to_double(surfaceY);
        pointerData->MotionTime = time;

        if (wl_pointer_get_version(pointer) < WL_POINTER_FRAME_SINCE_VERSION)
        {
            delegate* unmanaged<void*, wl_pointer*, void> onPointerFrame = &OnPointerFrame;

            onPointerFrame(data, pointer);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerButton(void* data, wl_pointer* pointer, uint serial, uint time, uint button, uint state)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var buttonPressed = MouseButton.None;

        switch ((input_event_codes)button)
        {
            case input_event_codes.BTN_LEFT:
                buttonPressed = MouseButton.Left;
                break;

            case input_event_codes.BTN_RIGHT:
                buttonPressed = MouseButton.Right;
                break;

            case input_event_codes.BTN_MIDDLE:
                buttonPressed = MouseButton.Middle;
                break;

            case input_event_codes.BTN_SIDE:
                buttonPressed = MouseButton.MbXbutton1;
                break;

            case input_event_codes.BTN_EXTRA:
                buttonPressed = MouseButton.MbXbutton2;
                break;
        }

        var pointerData = &cursorState->PointerDataBuffer;

        if ((state & (uint)wl_pointer_button_state.WL_POINTER_BUTTON_STATE_PRESSED) != 0)
        {
            pointerData->PressedButton |= buttonPressed;
            pointerData->LastButtonPressed = buttonPressed;
            pointerData->DoubleClickBegun  = true;
        }
        else
        {
            pointerData->PressedButton &= ~buttonPressed;
        }

        pointerData->ButtonTime   = time;
        pointerData->ButtonSerial = serial;

        if (wl_pointer_get_version(pointer) < WL_POINTER_FRAME_SINCE_VERSION)
        {
            delegate* unmanaged<void*, wl_pointer*, void> onPointerFrame = &OnPointerFrame;

            onPointerFrame(data, pointer);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerAxis(void* data, wl_pointer* pointer, uint time, uint axis, int value)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        switch ((wl_pointer_axis)axis)
        {
            case wl_pointer_axis.WL_POINTER_AXIS_VERTICAL_SCROLL:
                pointerData->Scroll.Y = (float)lib_wayland_client.wl_fixed_to_double(value);
                break;

            case wl_pointer_axis.WL_POINTER_AXIS_HORIZONTAL_SCROLL:
                pointerData->Scroll.X = (float)lib_wayland_client.wl_fixed_to_double(value);
                break;
        }

        pointerData->ButtonTime = time;

        if (wl_pointer_get_version(pointer) < WL_POINTER_FRAME_SINCE_VERSION)
        {
            delegate* unmanaged<void*, wl_pointer*, void> onPointerFrame = &OnPointerFrame;

            onPointerFrame(data, pointer);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerFrame(void* data, wl_pointer* pointer)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var previousPointerData = &cursorState->PointerData;
        var pointerData         = &cursorState->PointerDataBuffer;

        var hoverChanged = false;

        WindowState* windowState = null;

        var registryState = seatState->RegistryState;

        if (pointerData->WindowState != previousPointerData->WindowState)
        {
            if (previousPointerData->WindowState != null)
            {
                pointerData->PressedButton = default;
            }

            hoverChanged = true;

            if (previousPointerData->WindowState != null)
            {
                windowState = previousPointerData->WindowState;
            }
        }

        if (windowState == null && pointerData->WindowState != null)
        {
            windowState = pointerData->WindowState;
        }

        if (windowState != null)
        {
            const int SCALE = 1;

            registryState->CurrentSeat = seatState->Seat;

            if (previousPointerData->MotionTime != pointerData->MotionTime || previousPointerData->RelativeMotionTime != pointerData->RelativeMotionTime)
            {
                var deltaPosisiton = (pointerData->Position - previousPointerData->Position) * SCALE;

                Point<short> relative;
                Point<short> velocity;

                if (previousPointerData->RelativeMotionTime != pointerData->RelativeMotionTime)
                {
                    var time_delta = pointerData->RelativeMotionTime - previousPointerData->RelativeMotionTime;

                    relative = (pointerData->RelativeMotion * SCALE).ToPoint<short>();
                    velocity = (deltaPosisiton / time_delta).Cast<short>();
                }
                else
                {
                    var deltaTime = pointerData->MotionTime - previousPointerData->MotionTime;

                    relative = deltaPosisiton.Cast<short>();
                    velocity = (deltaPosisiton / deltaTime).Cast<short>();
                }

                var mouseEvent = new WindowMouseEvent
                {
                    Button         = default,
                    LeftHanded     = registryState->LeftHandedMouse,
                    Modifiers      = seatState->RegistryState->KeyboardState->Modifiers,
                    PressedButtons = pointerData->PressedButton,
                    Relative       = relative,
                    ScrollDelta    = default,
                    Velocity       = velocity,
                    X              = (ushort)(pointerData->Position.X * SCALE),
                    Y              = (ushort)(pointerData->Position.Y * SCALE),
                };

                pointerData->WindowState->AddMessage(WindowMessage.MouseMove(mouseEvent));
            }

            if (pointerData->DiscreteScrollVector120 - previousPointerData->DiscreteScrollVector120 != default)
            {
                if (pointerData->Scroll.Y != 0)
                {
                    var button = pointerData->Scroll.Y > 0 ? MouseButton.WheelDown : MouseButton.WheelUp;

                    pointerData->PressedButton |= button;
                }

                if (pointerData->Scroll.X != 0)
                {
                    var button = pointerData->Scroll.X > 0 ? MouseButton.WheelRight : MouseButton.WheelLeft;

                    pointerData->PressedButton |= button;
                }
            }
            else if (pointerData->Scroll - previousPointerData->Scroll != default)
            {
                // This is a continuous scroll, so we'll emit a pan gesture.
            }

            if (previousPointerData->PressedButton != pointerData->PressedButton)
            {
                var deltaButtons = previousPointerData->PressedButton ^ pointerData->PressedButton;

                Span<MouseButton> buttonsToTest =
                [
                    MouseButton.Left,
                    MouseButton.Middle,
                    MouseButton.Right,
                    MouseButton.WheelUp,
                    MouseButton.WheelDown,
                    MouseButton.WheelLeft,
                    MouseButton.WheelRight,
                    MouseButton.MbXbutton1,
                    MouseButton.MbXbutton2,
                ];

                foreach (var button in buttonsToTest)
                {
                    if (deltaButtons.HasFlags(button))
                    {
                        var scrollDelta = 0;

                        if (button == MouseButton.WheelUp || button == MouseButton.WheelDown)
                        {
                            scrollDelta = (int)Math.Abs(pointerData->DiscreteScrollVector120.Y / (float)120);
                        }

                        if (button == MouseButton.WheelRight || button == MouseButton.WheelLeft)
                        {
                            scrollDelta = (int)Math.Abs(pointerData->DiscreteScrollVector120.X / (float)120);
                        }

                        var mouseEvent = new WindowMouseEvent
                        {
                            Button         = button,
                            LeftHanded     = registryState->LeftHandedMouse,
                            Modifiers      = registryState->KeyboardState->Modifiers,
                            PressedButtons = pointerData->PressedButton,
                            Relative       = default,
                            ScrollDelta    = scrollDelta,
                            Velocity       = default,
                            X              = (ushort)(pointerData->Position.X * SCALE),
                            Y              = (ushort)(pointerData->Position.Y * SCALE),
                        };

                        var pressed = false;

                        if (pointerData->PressedButton.HasFlags(button))
                        {
                            pointerData->LastPressedPosition = pointerData->Position;

                            pressed = true;
                        }

                        var isDoubleClick = previousPointerData->DoubleClickBegun
                            && pressed
                            && pointerData->LastButtonPressed == previousPointerData->LastButtonPressed
                            && pointerData->ButtonTime - previousPointerData->ButtonTime < registryState->DoubleClikInterval
                            && ((previousPointerData->LastPressedPosition * SCALE) - (pointerData->LastPressedPosition * SCALE)).ToVector2().Length < 5;

                        if (isDoubleClick)
                        {
                            pointerData->DoubleClickBegun = false;
                            registryState->ActiveWindow->AddMessage(WindowMessage.DoubleClick(mouseEvent));
                        }
                        else
                        {
                            registryState->ActiveWindow->AddMessage(WindowMessage.Click(mouseEvent));

                            if (pressed)
                            {
                                registryState->ActiveWindow->AddMessage(WindowMessage.MouseDown(mouseEvent));
                            }
                            else
                            {
                                registryState->ActiveWindow->AddMessage(WindowMessage.MouseUp(mouseEvent));
                            }
                        }

                        if (button is MouseButton.WheelUp or MouseButton.WheelDown or MouseButton.WheelLeft or MouseButton.WheelRight)
                        {
                            pointerData->PressedButton = default;

                            var mouseWheelEvent = new WindowMouseEvent
                            {
                                Button         = button,
                                LeftHanded     = registryState->LeftHandedMouse,
                                Modifiers      = registryState->KeyboardState->Modifiers,
                                PressedButtons = default,
                                Relative       = default,
                                ScrollDelta    = scrollDelta,
                                Velocity       = default,
                                X              = mouseEvent.X,
                                Y              = mouseEvent.Y,
                            };

                            registryState->ActiveWindow->AddMessage(WindowMessage.MouseWheel(mouseWheelEvent));
                        }
                    }
                }
            }
        }

        pointerData->Scroll                  = default;
        pointerData->DiscreteScrollVector120 = default;

        *previousPointerData = *pointerData;

        if (hoverChanged && registryState->ActiveWindow != null)
        {
            registryState->ActiveWindow->AddMessage(WindowMessage.FocusIn());
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerAxisDiscrete(void* data, wl_pointer* pointer, uint axis, int discrete)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        switch ((wl_pointer_axis)axis)
        {
            case wl_pointer_axis.WL_POINTER_AXIS_VERTICAL_SCROLL:
                pointerData->DiscreteScrollVector120.Y = discrete * 120;
                break;

            case wl_pointer_axis.WL_POINTER_AXIS_HORIZONTAL_SCROLL:
                pointerData->DiscreteScrollVector120.X = discrete * 120;
                break;
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPointerAxisRelativeDirection(void* data, wl_pointer* pointer, uint axis, uint direction)
    { }

    [UnmanagedCallersOnly]
    private static void OnPointerAxisSource(void* data, wl_pointer* pointer, uint axisSource)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        pointerData->ScrollType = (wl_pointer_axis_source)axisSource;
    }

    [UnmanagedCallersOnly]
    private static void OnPointerAxisStop(void* data, wl_pointer* pointer, uint time, uint axis)
    { }

    [UnmanagedCallersOnly]
    private static void OnPointerAxisValue120(void* data, wl_pointer* pointer, uint axis, int value120)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        switch ((wl_pointer_axis)axis)
        {
            case wl_pointer_axis.WL_POINTER_AXIS_VERTICAL_SCROLL:
                pointerData->DiscreteScrollVector120.Y += value120;
                break;

            case wl_pointer_axis.WL_POINTER_AXIS_HORIZONTAL_SCROLL:
                pointerData->DiscreteScrollVector120.X += value120;
                break;
        }
    }

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

        if (string.Compare(@interface, wl_seat_interface->name))
        {
            var seat = (wl_seat*)wl_registry_bind(registry, name, wl_seat_interface, Math.Clamp(version, 1, 9));

            SetProxyTag((wl_proxy*)seat);

            var seatState = SeatState.Allocate(new(name, seat), registryState);

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

        if (string.Compare(@interface, zwp_idle_inhibit_manager_v1_interface->name))
        {
            registryState->IdleInhibitManager = new(name, (zwp_idle_inhibit_manager_v1*)wl_registry_bind(registry, name, zwp_idle_inhibit_manager_v1_interface, 1));
        }
    }

    [UnmanagedCallersOnly]
    private static void OnRegistryGlobalRemove(void* data, wl_registry* registry, uint name)
    {
        var registryState = (RegistryState*)data;
    }

    [UnmanagedCallersOnly]
    private static void OnPrimarySelectionDevicedataOffer(void* data, zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_offer_v1* id)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        fixed (zwp_primary_selection_offer_v1_listener* pOfferListener = &primarySelectionOfferListener)
        {
            zwp_primary_selection_offer_v1_add_listener(id, pOfferListener, seatState);
        }
    }

    [UnmanagedCallersOnly]
    private static void OnPrimarySelectionDeviceselection(void* data, zwp_primary_selection_device_v1* zwp_primary_selection_device_v1, zwp_primary_selection_offer_v1* offer)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        if (seatState->PrimarySelectionCurrentOffer != null && seatState->PrimarySelectionCurrentOffer != offer)
        {
            zwp_primary_selection_offer_v1_destroy(seatState->PrimarySelectionCurrentOffer);
        }

        seatState->PrimarySelectionCurrentOffer = offer;
    }

    [UnmanagedCallersOnly]
    private static void OnPrimarySelectionOffer(void* data, zwp_primary_selection_offer_v1* zwp_primary_selection_offer_v1, byte* mimeType)
    {
    }

    [UnmanagedCallersOnly]
    private static void OnPrimarySelectionSourceSend(void* data, zwp_primary_selection_source_v1* zwp_primary_selection_source_v1, byte* mimeType, int fd)
    {
    }

    [UnmanagedCallersOnly]
    private static void OnPrimarySelectionSourceCancelled(void* data, zwp_primary_selection_source_v1* zwp_primary_selection_source_v1)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        if (seatState->PrimarySelectionCurrentSource != null)
        {
            primary_selection.zwp_primary_selection_source_v1_destroy(seatState->PrimarySelectionCurrentSource);

            seatState->PrimarySelectionCurrentSource = null;
        }
    }

    [UnmanagedCallersOnly]
    private static void OnRelativePointerV1RelativeMotion(void* data, zwp_relative_pointer_v1* pointer, uint utimeHi, uint utimeLo, int dx, int dy, int dxUnaccel, int dyUnaccel)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var cursorState = (CursorState*)(void*)seatState->RegistryState->CursorState;

        var pointerData = &cursorState->PointerDataBuffer;

        pointerData->RelativeMotion.X    = (float)lib_wayland_client.wl_fixed_to_double(dx);
        pointerData->RelativeMotion.Y    = (float)lib_wayland_client.wl_fixed_to_double(dy);
        pointerData->RelativeMotionTime  = (utimeHi << 32) | utimeLo;
    }

    [UnmanagedCallersOnly]
    private static void OnSeatName(void* data, wl_seat* seat, byte* name) =>
        Console.WriteLine(nameof(OnSeatName));

    [UnmanagedCallersOnly]
    private static void OnSeatCapabilities(void* data, wl_seat* seat, uint capabilities)
    {
        var seatState = (SeatState*)data;

        Debug.Assert(seatState != null);

        var seatCapabilities = (wl_seat_capability)capabilities;

        if (seatCapabilities.HasFlags(wl_seat_capability.WL_SEAT_CAPABILITY_POINTER))
        {
            if (seatState->RegistryState->CursorState == null)
            {
                var cursorState = seatState->RegistryState->CursorState = CursorState.Allocate(seatState);

                var cursorSurface = wl_compositor_create_surface(seatState->RegistryState->Compositor);

                wl_surface_commit(cursorSurface);

                var pointer = wl_seat_get_pointer(seat);

                cursorState->CursorSurface = wl_compositor_create_surface(seatState->RegistryState->Compositor);
                wl_surface_commit(cursorState->CursorSurface);

                cursorState->Pointer = wl_seat_get_pointer(seat);

                fixed (wl_pointer_listener* pPointerListener = &pointerListener)
                {
                    wl_pointer_add_listener(cursorState->Pointer, pPointerListener, seatState);
                }

                if (seatState->RegistryState->CursorShapeManager != default)
                {
                    cursorState->CursorShapeDevice = wp_cursor_shape_manager_v1_get_pointer(seatState->RegistryState->CursorShapeManager, cursorState->Pointer);
                }

                if (seatState->RegistryState->RelativePointerManager != default)
                {
                    cursorState->RelativePointer = zwp_relative_pointer_manager_v1_get_relative_pointer(seatState->RegistryState->RelativePointerManager, cursorState->Pointer);

                    fixed (zwp_relative_pointer_v1_listener* pRelativePointerListener = &relativePointerListener)
                    {
                        zwp_relative_pointer_v1_add_listener(cursorState->RelativePointer, pRelativePointerListener, seatState);
                    }
                }
            }
        }
        else
        {
            seatState->RegistryState->DisposeCursoState();
        }

        if (seatCapabilities.HasFlags(wl_seat_capability.WL_SEAT_CAPABILITY_KEYBOARD))
        {
            if (seatState->RegistryState->KeyboardState == null)
            {
                var keyboard = wl_seat_get_keyboard(seat);

                var keyboardState = seatState->RegistryState->KeyboardState = KeyboardState.Allocate(keyboard, seatState);

                keyboardState->Context = xkb_context_new(xkb_context_flags.XKB_CONTEXT_NO_FLAGS);

                Debug.Assert(keyboardState->Context != null);

                fixed (wl_keyboard_listener* pKeyboardListener = &keyboardListener)
                {
                    wl_keyboard_add_listener(keyboardState->Keyboard, pKeyboardListener, seatState);
                }
            }
        }
        else if (seatState->RegistryState->KeyboardState != null)
        {

        }
    }

    [UnmanagedCallersOnly]
    private static void OnSurfacePreferredBufferScale(void* data, wl_surface* surface, int factor) =>
        Console.WriteLine(nameof(OnSurfacePreferredBufferScale));

    [UnmanagedCallersOnly]
    private static void OnSurfacePreferredBufferTransform(void* data, wl_surface* surface, int transform) =>
        Console.WriteLine(nameof(OnSurfacePreferredBufferTransform));

    [UnmanagedCallersOnly]
    private static void OnSurfaceEnter(void* data, wl_surface* surface, wl_output* output)
    {
        var windowState = GetWindowState(surface);

        if (windowState == null)
        {
            return;
        }

        windowState->AddOutput(output);
    }

    [UnmanagedCallersOnly]
    private static void OnSurfaceLeave(void* data, wl_surface* surface, wl_output* output)
    {
        var windowState = GetWindowState(surface);

        if (windowState == null)
        {
            return;
        }

        windowState->RemoveOutput(output);
    }

    [UnmanagedCallersOnly]
    private static void OnWmBasePing(void* data, xdg_wm_base* wmBase, uint serial) =>
        xdg_wm_base_pong(wmBase, serial);
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

    private static void UpdateCursor(CursorState* cursorState)
    {
        Debug.Assert(cursorState != null);

        wl_buffer* cursorBuffer = null;

        var hotspotX = 0;
        var hotspotY = 0;
        var scale    = 1;

        if (cursorState->CursorVisible)
        {
            if (cursorState->CustomCursors.TryGetValue((uint)cursorState->Cursor, out var customCursor))
            {
                cursorBuffer = customCursor.Buffer;
                hotspotX     = customCursor.Hotspot.X;
                hotspotY     = customCursor.Hotspot.Y;

                scale = 1;
            }
            else if (cursorState->SeatState->RegistryState->CursorShapeManager != default)
            {
                var shape = standardCursors[(uint)cursorState->Cursor];

                wp_cursor_shape_device_v1_set_shape(cursorState->CursorShapeDevice, cursorState->PointerEnterSerial, (uint)shape);

                return;
            }
            else
            {
                var cursor = (wl_cursor*)cursorState->SeatState->RegistryState->Cursors[(int)cursorState->Cursor];

                if (cursor == null)
                {
                    return;
                }

                var frameIndex = 0;

                if (cursor->image_count > 1)
                {
                    frameIndex = wl_cursor_frame(cursor, cursorState->CursorTimeMs);

                    if (cursorState->CursorFrameCallback == null)
                    {
                        cursorState->CursorFrameCallback = wl_surface_frame(cursorState->CursorSurface);

                        fixed (wl_callback_listener* pCursorFrameCallbackListener = &cursorFrameCallbackListener)
                        {
                            wl_callback_add_listener(cursorState->CursorFrameCallback, pCursorFrameCallbackListener, cursorState);
                        }
                    }
                }

                var cursorImage = cursor->images[frameIndex];

                scale = cursorState->CursorScale;

                cursorBuffer = wl_cursor_image_get_buffer(cursorImage);

                hotspotX = (int)(cursorImage->hotspotX / scale);
                hotspotY = (int)(cursorImage->hotspotY / scale);
            }
        }

        wl_pointer_set_cursor(cursorState->Pointer, cursorState->PointerEnterSerial, cursorState->CursorSurface, hotspotX, hotspotY);
        wl_surface_set_buffer_scale(cursorState->CursorSurface, scale);
        wl_surface_attach(cursorState->CursorSurface, cursorBuffer, 0, 0);
        wl_surface_damage_buffer(cursorState->CursorSurface, 0, 0, int.MaxValue, int.MaxValue);

        wl_surface_commit(cursorState->CursorSurface);
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

    private void LoadCursorTheme()
    {
        if (this.registryState->CursorTheme != null)
        {
            wl_cursor_theme_destroy(this.registryState->CursorTheme);

            this.registryState->CursorTheme = null;
        }

        if (this.registryState->CursorThemeName == null)
        {
            this.registryState->CursorThemeName = MemoryMarshal.CreateUTF8StringBuffer("default");
        }

        var cursorSize = this.registryState->UnscaledCursorSize * 1;

        this.registryState->CursorTheme = wl_cursor_theme_load(this.registryState->CursorThemeName, cursorSize, this.registryState->Shm);

        Debug.Assert(this.registryState->CursorTheme != null);

        for (var i = 0; i < Cursor.Length; i++)
        {
            using var cursorName = cursorNames[i].ToUnmanaged();

            var cursor = wl_cursor_theme_get_cursor(this.registryState->CursorTheme, cursorName);

            if (cursor == null && cursorNamesFallback[i] != null)
            {
                using var cursorNameFallback = cursorNamesFallback[i]!.ToUnmanaged();

                cursor = wl_cursor_theme_get_cursor(this.registryState->CursorTheme, cursorNameFallback);
            }

            if (cursor != null && cursor->image_count > 0)
            {
                this.registryState->Cursors[i] = cursor;
            }
            else
            {
                this.registryState->Cursors[i] = null;

                Logger.Warn($"Failed loading cursor: {cursorNames[i]}");
            }
        }
    }


    protected override partial void OnDisposed(bool disposing)
    {
        this.stopped = true;

        _ = wl_display_roundtrip(this.registryState->Display);

        this.eventLoopThread.Join();

        RegistryState.Free(this.registryState);
    }

    internal partial void CloseWindow(Window window) =>
        WindowState.Free(window.State);

    internal partial WindowState* CreateWindow(string title, Size<int> size, Window? parent)
    {
        var windowState = WindowState.Allocate
            (wl_compositor_create_surface(this.registryState->Compositor),
            size
        );

        SetProxyTag((wl_proxy*)windowState->Surface);

        fixed (wl_surface_listener* pSurfaceListener = &surfaceListener)
        {
            wl_surface_add_listener(windowState->Surface, pSurfaceListener, windowState);
        }

        if (this.registryState->Viewporter != default)
        {
            windowState->Viewport = wp_viewporter_get_viewport(this.registryState->Viewporter, windowState->Surface);

            if (this.registryState->FractionalScaleManager != default)
            {
                windowState->FractionalScale = wp_fractional_scale_manager_v1_get_fractional_scale(this.registryState->FractionalScaleManager, windowState->Surface);

                fixed (wp_fractional_scale_v1_listener* pFractionalScaleListener = &fractionalScaleListener)
                {
                    wp_fractional_scale_v1_add_listener(windowState->FractionalScale, pFractionalScaleListener, windowState);
                }
            }
        }

        fixed (libdecor_frame_interface* pFrameInterface = &frameInterface)
        {
            windowState->Frame = libdecor_decorate(this.registryState->LibdecorContext, windowState->Surface, pFrameInterface, windowState);
        }

        libdecor_frame_map(windowState->Frame);

        windowState->FrameCallBack = wl_surface_frame(windowState->Surface);

        fixed (wl_callback_listener* pFrameCallbackListener = &frameCallbackListener)
        {
            wl_callback_add_listener(windowState->FrameCallBack, pFrameCallbackListener, windowState);
        }

        wl_surface_commit(windowState->Surface);

        _ = wl_display_roundtrip(this.registryState->Display);

        UpdateSize(windowState, windowState->Size);

        using var uId = new UnmanagedString(this.Id);

        libdecor_frame_set_app_id(windowState->Frame, uId);

        return windowState;
    }

    internal partial NativeArray<WindowMessage> FlushWindowEvents(Window window)
    {
        var messages = window.State->GetMessages();

        window.State->ClearMessages();

        return messages;
    }

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

    internal partial void SetCursorCustomImage(Cursor cursor, CursorImage image, Point<int> hotpot) => throw new NotImplementedException();

    internal partial void SetWindowTitle(Window window, string value)
    {
        using var title = value.ToUnmanaged();

        libdecor_frame_set_title(window.State->Frame, title);
    }

    internal partial void ShowWindow(Window window) =>
        throw new NotImplementedException();

    internal partial void UpdateCursor() =>
        UpdateCursor(this.registryState->CursorState);

    internal static void UpdateSize(WindowState* windowState, Size<int> size)
    {
        var sizeHasChanged = false;

        if (windowState->Size != size)
        {
            windowState->Size = size;

            sizeHasChanged = true;
        }

        if (windowState->Surface != null && windowState->Viewport != null)
        {
            wp_viewport_set_destination(windowState->Viewport, size.Width, size.Height);
        }

        var libdecorState = libdecor_state_new(size.Width, size.Height);

        libdecor_frame_commit(windowState->Frame, libdecorState, windowState->PendingLibdecorConfiguration);
        libdecor_state_free(libdecorState);

        if (sizeHasChanged)
        {
            windowState->AddMessage(WindowMessage.Resized());
        }

        windowState->PendingLibdecorConfiguration = null;
    }
}
#endif
