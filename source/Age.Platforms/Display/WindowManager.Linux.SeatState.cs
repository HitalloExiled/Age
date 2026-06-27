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

        public byte*                            ClipboardDataSourceData;
        public int                              ClipboardDataSourceLength;
        public wl_data_device*                  DataDevice;
        public wl_data_offer*                   DataOfferSelection;
        public wl_data_source*                  DataSourceSelection;
        public zwp_primary_selection_offer_v1*  PrimarySelectionCurrentOffer;
        public zwp_primary_selection_source_v1* PrimarySelectionCurrentSource;
        public zwp_primary_selection_device_v1* PrimarySelectionDevice;

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

            if (this.DataOfferSelection != null)
            {
                lib_wayland_client.wl_data_offer_destroy(this.DataOfferSelection); // Investigate

                this.DataOfferSelection = null;
            }

            if (this.DataSourceSelection != null)
            {
                lib_wayland_client.wl_data_source_destroy(this.DataSourceSelection);

                this.DataSourceSelection = null;
            }

            if (this.ClipboardDataSourceData != null)
            {
                NativeMemory.Free(this.ClipboardDataSourceData);

                this.ClipboardDataSourceData = null;
            }

            this.ClipboardDataSourceLength = 0;
        }

        internal static void Free(SeatState* seatState)
        {
            seatState->Dispose();

            NativeMemory.Free(seatState);
        }
    }
}
#endif
