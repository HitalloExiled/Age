#if LINUX
using System.Runtime.InteropServices;
using Age.Core.Extensions;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct SeatState
    {
        public RegistryState* RegistryState;
        public Named<wl_seat> Seat;

        public wl_data_device*                  DataDevice;
        public ExtendedState                    ExtendedState;
        public zwp_pointer_gesture_pinch_v1*    PointerGesturePinch;
        public zwp_primary_selection_device_v1* PrimarySelectionDevice;
        public zwp_tablet_seat_v2*              TabletSeat;
        public zwp_text_input_v3*               TextInput;

        private SeatState(Named<wl_seat> seat, RegistryState* registry)
        {
            this.Seat          = seat;
            this.RegistryState = registry;
        }

        public static SeatState* Allocate(Named<wl_seat> seat, RegistryState* registry) =>
            NativeMemory.Alloc(new SeatState(seat, registry));

        public void Dispose()
        {
            if (this.DataDevice != null)
            {
                lib_wayland_client.wl_data_device_destroy(this.DataDevice);

                this.DataDevice = null;
            }

            if (this.TabletSeat != null)
            {
                tablet.zwp_tablet_seat_v2_destroy(this.TabletSeat);

                this.TabletSeat = null;
            }
        }

        internal static void Free(SeatState* seatState)
        {
            seatState->Dispose();

            NativeMemory.Free(seatState);
        }
    }
}
#endif
