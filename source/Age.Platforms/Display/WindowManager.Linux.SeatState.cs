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
    private struct OfferState
    {
        public NativeHashSet<NativeString> MimeTypes;

        public OfferState() =>
            this.MimeTypes = new();

        public static OfferState* Allocate() =>
            NativeMemory.Alloc(new OfferState());

        public static void Free(OfferState* offerState)
        {
            offerState->Dispose();

            NativeMemory.Free(offerState);
        }

        public void Dispose()
        {
            foreach (var mimeType in this.MimeTypes)
            {
                mimeType.Dispose();
            }

            this.MimeTypes.Clear();
            this.MimeTypes.Dispose();
        }
    }

    private struct SeatState
    {
        #region 8-bytes
        public NativeDictionary<uint, Key> PressedKeycodes;

        public RegistryState* RegistryState;
        public Named<wl_seat> Seat;

        public byte*                            ClipboardDataSourceData;
        public zwp_confined_pointer_v1*         ConfinedPointer;
        public wl_callback*                     CursorFrameCallback;
        public wp_cursor_shape_device_v1*       CursorShapeDevice;
        public wl_surface*                      CursorSurface;
        public ulong                            CursorTimeMs;
        public wl_data_device*                  DataDevice;
        public wl_data_offer*                   DataOfferSelection;
        public wl_data_source*                  DataSourceSelection;
        public wl_keyboard*                     Keyboard;
        public NativeBuffer<byte>               KeymapBuffer;
        public ulong                            LastRepeatMs;
        public ulong                            LastRepeatStartMs;
        public zwp_locked_pointer_v1*           LockedPointer;
        public wl_pointer*                      Pointer;
        public PointerData                      PointerData       = new();
        public PointerData                      PointerDataBuffer = new();
        public zwp_primary_selection_device_v1* PrimarySelectionDevice;
        public zwp_primary_selection_offer_v1*  PrimarySelectionOffer;
        public zwp_primary_selection_source_v1* PrimarySelectionSource;
        public zwp_relative_pointer_v1*         RelativePointer;
        public ulong                            RepeatKeyDelayMs;
        public ulong                            RepeatStartDelayMs;
        public xkb_compose_state*               XKBComposeState;
        public xkb_compose_table*               XKBComposeTable;
        public xkb_context*                     XKBContext;
        public xkb_keymap*                      XKBKeymap;
        public xkb_state*                       XKBState;
        #endregion

        #region 4-bytes
        public int      ClipboardDataSourceLength;
        public uint     CurrentLayoutIndex;
        public uint     LastKeyPressedSerial;
        public Modifier Modifiers;
        public uint     ModsDepressed;
        public uint     ModsLatched;
        public uint     ModsLocked;
        public uint     PointerEnterSerial;
        public uint     RepeatingKeycode = lib_xkbommon.XKB_KEYCODE_INVALID;
        #endregion

        private SeatState(Named<wl_seat> seat, RegistryState* registry)
        {
            this.Seat          = seat;
            this.RegistryState = registry;

            this.PressedKeycodes = [];
        }

        public static SeatState* Allocate(Named<wl_seat> seat, RegistryState* registry) =>
            NativeMemory.Alloc(new SeatState(seat, registry));

        public void Dispose() =>
            this.PressedKeycodes.Dispose();

        internal static void Free(SeatState* seatState)
        {
            seatState->Dispose();

            NativeMemory.Free(seatState);
        }
    }
}
#endif
