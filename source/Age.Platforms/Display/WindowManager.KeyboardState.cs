#if LINUX
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibXKBCommon;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct KeyboardState
    {
        public readonly wl_keyboard* Keyboard;
        public readonly SeatState*   SeatState;

        public xkb_compose_state* ComposeState;
        public xkb_compose_table* ComposeTable;
        public xkb_context*       Context;
        public xkb_keymap*        Keymap;
        public NativeBuffer<byte> KeymapBuffer;
        public long               LastRepeatStartMsec;
        public xkb_state*         State;

        public NativeDictionary<uint, Key> PressedKeycodes = [];

        public uint     CurrentLayoutIndex;
        public uint     LastKeyPressedSerial;
        public Modifier Modifiers;
        public uint     ModsDepressed;
        public uint     ModsLatched;
        public uint     ModsLocked;
        public uint     RepeatingKeycode;
        public int      RepeatKeyDelayMsec;
        public int      RepeatStartDelayMsec;

        private KeyboardState(wl_keyboard* keyboard, SeatState* seatState)
        {
            this.Keyboard  = keyboard;
            this.SeatState = seatState;
        }

        public static KeyboardState* Allocate(wl_keyboard* keyboard, SeatState* seatState) =>
            NativeMemory.Alloc(new KeyboardState(keyboard, seatState));

        public void Dispose()
        {
            this.PressedKeycodes.Dispose();

            if (this.Context != null)
            {
                lib_xkbommon.xkb_context_unref(this.Context);
            }

            if (this.Keyboard != null)
            {
                lib_wayland_client.wl_keyboard_destroy(this.Keyboard);
            }
        }
    }
}
#endif
