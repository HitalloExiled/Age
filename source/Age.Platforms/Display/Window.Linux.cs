#if LINUX
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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

using Age.Platforms.Linux.Wayland;

namespace Age.Platforms.Display;

internal unsafe struct Named<T>(uint name, T* value) : IEquatable<Named<T>> where T : unmanaged
{
    public T* Value = value;

    public uint Name = name;

    public readonly bool Equals(Named<T> other) =>
        this.Name == other.Name && this.Value == other.Value;

    public override readonly bool Equals([NotNullWhen(true)] object? obj) =>
        obj is Named<T> named && this.Equals(named);

    public override readonly int GetHashCode() =>
        HashCode.Combine(this.Name, (nint)this.Value);

    public static implicit operator T*(Named<T> named) => named.Value;

    public static bool operator ==(Named<T> left, Named<T> right) => left.Equals(right);
    public static bool operator !=(Named<T> left, Named<T> right) => !left.Equals(right);
}

public unsafe partial class Window
{
    private struct SeatState
    {
        public wl_data_device*                  DataDevice;
        public zwp_primary_selection_device_v1* PrimarySelectionDevice;
        public Named<wl_seat>                   Seat;
        public zwp_tablet_seat_v2*              TabletSeat;
        public zwp_text_input_v3*               TextInput;
    }

    private struct ScreenData
    {
        public byte*      Make;
        public byte*      Model;
        public Size<int>  PhysicalSize;
        public Point<int> Position;
        public float      RefreshRate = -1;
        public int        Scale = 1;
        public Size<int>  Size;

        public ScreenData()
        { }
    };

    private struct ScreenState
    {
        public ScreenData Data;
        public ScreenData PendingData;
    }

    private readonly static NativeList<Pointer<wl_seat>>   seats = [];
    private readonly static NativeList<Pointer<wl_output>> outputs = [];

    private readonly static byte** pTag = (byte**)NativeMemory.AllocSet((nint)MemoryMarshal.CreateUTF8StringBuffer("Age"));
    private readonly static Dictionary<uint, Pointer<ScreenState>> screenStates = [];

    private static readonly wl_data_device_listener* dataDeviceListener = NativeMemory.AllocSet(
        new wl_data_device_listener()
        {
            data_offer = &OnDataDeviceDataOffer,
            enter      = &OnDataDeviceEnter,
            leave      = &OnDataDeviceLeave,
            motion     = &OnDataDeviceMotion,
            drop       = &OnDataDeviceDrop,
            selection  = &OnDataDeviceSelection,
        }
    );

    private static readonly wl_output_listener* outputListener = NativeMemory.AllocSet(
        new wl_output_listener()
        {
            geometry    = &OnOutputGeometry,
            mode        = &OnOutputMode,
            done        = &OnOutputDone,
            scale       = &OnOutputScale,
            name        = &OnOutputName,
            description = &OnOutputDescription,
        }
    );

    private static readonly zwp_primary_selection_device_v1_listener* primarySelectionDeviceListener = NativeMemory.AllocSet(
        new zwp_primary_selection_device_v1_listener
        {
            data_offer = &OnWpPrimarySelectionDevicedataOffer,
		    selection  = &OnWpPrimarySelectionDeviceselection,
        }
    );

    private static readonly wl_registry_listener* registryListener = NativeMemory.AllocSet(
        new wl_registry_listener()
        {
            global        = &OnRegistryGlobal,
            global_remove = &OnRegistryGlobalRemove,
        }
    );

    private static readonly wl_seat_listener* wlSeatListener = NativeMemory.AllocSet(
        new wl_seat_listener()
        {
            capabilities = &OnSeatCapabilities,
		    name         = &OnSeatName,
        }
    );

    private static readonly zwp_tablet_seat_v2_listener* tabletSeatListener = NativeMemory.AllocSet(
        new zwp_tablet_seat_v2_listener
        {
            tablet_added = &OnTabletSeatTabletAdded,
            tool_added   = &OnTabletSeatToolAdded,
            pad_added    = &OnTabletSeatPadAdded,
        }
    );




    private static readonly zwp_text_input_v3_listener* textInputListener = NativeMemory.AllocSet(
        new zwp_text_input_v3_listener()
        {
            enter                   = &OntextInputEnter,
            leave                   = &OntextInputLeave,
            preedit_string          = &OntextInputPreeditString,
            commit_string           = &OntextInputCommitString,
            delete_surrounding_text = &OntextInputDeleteSurroundingText,
            done                    = &OntextInputDone,
        }
    );

    private static Named<xdg_activation_v1>                 activation;
    private static Named<wl_compositor>                     compositor;
    private static wl_seat*                                 currentSeat;
    private static Named<wp_cursor_shape_manager_v1>        cursorShapeManager;
    private static Named<wl_data_device_manager>            dataDeviceManager;
    private static Named<zxdg_decoration_manager_v1>        decorationManager;
    private static wl_display*                              display;
    private static Named<wp_fractional_scale_manager_v1>    fractionalScaleManager;
    private static Named<zwp_idle_inhibit_manager_v1>       idleInhibitManager;
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
    private static xdg_wm_base_listener*                    wmBaseListener;

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

            UpdateCursor();
        }
    }

    public partial Point<int> Position
    {
        get => this.position;
        set => throw new NotImplementedException();
    }

    public partial Size<uint> Size
    {
        get => this.size;
        set => throw new NotImplementedException();
    }

    public partial string Title
    {
        get => this.title;
        set => throw new NotImplementedException();
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
    private static void OntextInputCommitString(void* data, zwp_text_input_v3* textInput, byte* text) =>
        Console.WriteLine(nameof(OntextInputCommitString));

    [UnmanagedCallersOnly]
    private static void OntextInputDeleteSurroundingText(void* data, zwp_text_input_v3* textInput, uint beforeLength, uint afterLength) =>
        Console.WriteLine(nameof(OntextInputDeleteSurroundingText));

    [UnmanagedCallersOnly]
    private static void OntextInputDone(void* data, zwp_text_input_v3* textInput, uint serial) =>
        Console.WriteLine(nameof(OntextInputDone));

    [UnmanagedCallersOnly]
    private static void OntextInputEnter(void* data, zwp_text_input_v3* textInput, wl_surface* surface) =>
        Console.WriteLine(nameof(OntextInputEnter));

    [UnmanagedCallersOnly]
    private static void OntextInputLeave(void* data, zwp_text_input_v3* textInput, wl_surface* surface) =>
        Console.WriteLine(nameof(OntextInputLeave));

    [UnmanagedCallersOnly]
    private static void OntextInputPreeditString(void* data, zwp_text_input_v3* textInput, byte* text, int cursorBegin, int cursorEnd) =>
        Console.WriteLine(nameof(OntextInputPreeditString));

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
                    wl_data_device_add_listener(state->DataDevice, dataDeviceListener, null);
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

            wl_output_add_listener(output, outputListener, screenState);
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

                wl_data_device_add_listener(seatState->DataDevice, dataDeviceListener, seatState);
            }

            if (seatState->PrimarySelectionDevice == default && primarySelectionDeviceManager != default)
            {
                // Primary selection.
                seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(primarySelectionDeviceManager, seat);

                zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, primarySelectionDeviceListener, seatState);
            }

            if (seatState->TextInput == default && textInputManager != default) {
                // IME.
                seatState->TextInput = zwp_text_input_manager_v3_get_text_input(textInputManager, seat);
                zwp_text_input_v3_add_listener(seatState->TextInput, textInputListener, seatState);
            }

            seats.Add(seat);

            wl_seat_add_listener(seat, wlSeatListener, seatState);

            if (currentSeat == default)
            {
			    currentSeat = seat;
		    }

            return;
        }

        if (string.Compare(@interface, xdg_wm_base_interface->name))
        {
            wmBase = new(name, (xdg_wm_base*)wl_registry_bind(registry, name, xdg_wm_base_interface, Math.Clamp(version, 1, 6)));

            xdg_wm_base_add_listener(wmBase, wmBaseListener, null);

            return;
        }

        if (string.Compare(@interface, wp_viewporter_interface->name))
        {
            viewporter = new(name, (wp_viewporter*)wl_registry_bind(registry, name, wp_viewporter_interface, 1));
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

                if (seatState->PrimarySelectionDevice == default && primarySelectionDeviceManager != default)
                {
                    seatState->PrimarySelectionDevice = zwp_primary_selection_device_manager_v1_get_device(primarySelectionDeviceManager, seat);

                    zwp_primary_selection_device_v1_add_listener(seatState->PrimarySelectionDevice, primarySelectionDeviceListener, seatState);
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

                zwp_tablet_seat_v2_add_listener(seatState->TabletSeat, tabletSeatListener, seatState);
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

                zwp_text_input_v3_add_listener(seatState->TextInput, textInputListener, seatState);
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
    private static void OnTabletSeatPadAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_pad_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatPadAdded));

    [UnmanagedCallersOnly]
    private static void OnTabletSeatTabletAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatTabletAdded));

    [UnmanagedCallersOnly]
    private static void OnTabletSeatToolAdded(void* data, zwp_tablet_seat_v2* tabletSeat, zwp_tablet_tool_v2* id) =>
        Console.WriteLine(nameof(OnTabletSeatToolAdded));

    private partial void Create(string title, Size<uint> size, Point<int> position, Window? parent)
    {

    }

    private partial void UpdateCursor() => throw new NotImplementedException();

    public static partial void Register(string? className)
    {
        var uClassName = new UnmanagedString(className);

        display = wl_display_connect(uClassName);

        NullReferenceException.ValidateNotNull(display, "Can't connect to a Wayland display.");

        registry = wl_display_get_registry(display);

        NullReferenceException.ValidateNotNull(registry, "Can't obtain the Wayland registry global.");

        wl_registry_add_listener(registry, registryListener, null);

        _ = wl_display_roundtrip(display);
    }

    public partial void Close() => throw new NotImplementedException();

    public partial void DoEvents() => throw new NotImplementedException();
    public partial string? GetClipboardData() => throw new NotImplementedException();
    public partial void Hide() => throw new NotImplementedException();
    public partial void Maximize() => throw new NotImplementedException();
    public partial void Minimize() => throw new NotImplementedException();
    public partial void Restore() => throw new NotImplementedException();
    public partial void SetClipboardData(string value) => throw new NotImplementedException();
    public partial void Show() => throw new NotImplementedException();
}
#endif
