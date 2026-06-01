#if LINUX
using Age.Core;
using Age.Core.Collections;
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibXKBCommon;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct SeatState
    {
        public required RegistryState* Registry;
        public required Named<wl_seat> Seat;

        public WindowState*                     ActiveWindow;
        public zwp_confined_pointer_v1*         ConfinedPointer;
        public wl_callback*                     CursorFrameCallback;
        public wp_cursor_shape_device_v1*       CursorShapeDevice;
        public wl_surface*                      CursorSurface;
        public wl_data_device*                  DataDevice;
        public wl_keyboard*                     Keyboard;
        public NativeBuffer<byte>               KeymapBuffer;
        public long                             LastRepeatStartMsec;
        public zwp_locked_pointer_v1*           LockedPointer;
        public wl_pointer*                      Pointer;
        public zwp_pointer_gesture_pinch_v1*    PointerGesturePinch;
        public zwp_primary_selection_device_v1* PrimarySelectionDevice;
        public zwp_relative_pointer_v1*         RelativePointer;
        public zwp_tablet_seat_v2*              TabletSeat;
        public zwp_text_input_v3*               TextInput;
        public xkb_compose_state*               XkbComposeState;
        public xkb_context*                     XkbContext;
        public xkb_keymap*                      XkbKeymap;
        public xkb_state*                       XkbState;

        public NativeDictionary<uint, Key> PressedKeycodes;

        public uint CurrentLayoutIndex;
        public uint LastKeyPressedSerial;
        public uint RepeatingKeycode;
        public int  RepeatKeyDelayMsec;
        public int  RepeatStartDelayMsec;

        public bool AltPressed;
        public bool CtrlPressed;
        public bool MetaPressed;
        public bool ShiftPressed;

        public SeatState() =>
            this.PressedKeycodes = [];

        public void Dispose() =>
            this.PressedKeycodes.Dispose();
    }
}
#endif
