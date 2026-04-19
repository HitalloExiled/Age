#if LINUX
using Age.Platforms.Linux.Wayland;

namespace Age.Platforms.Display;

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
}
#endif
