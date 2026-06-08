#if LINUX

using System.Runtime.CompilerServices;
using Age.Core;
using Age.Core.Collections;
using Age.Platforms.Linux.LibDecor;
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibWaylandCursor;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private struct RegistryState : IDisposable
    {
        #region 8-bytes
        private NativeDictionary<uint, Pointer<ScreenState>> screenStates;
        private NativeList<Pointer<wl_output>>               outputs;
        private NativeList<Pointer<wl_seat>>                 seats;

        public NativeArray<Pointer<wl_cursor>> Cursors;

        public Named<xdg_activation_v1>                 Activation;
        public Named<wl_compositor>                     Compositor;
        public wl_seat*                                 CurrentSeat;
        public Named<wp_cursor_shape_manager_v1>        CursorShapeManager;
        public wl_cursor_theme*                         CursorTheme;
        public byte*                                    CursorThemeName;
        public Named<wl_data_device_manager>            DataDeviceManager;
        public Named<zxdg_decoration_manager_v1>        DecorationManager;
        public wl_display*                              Display;
        public Named<wp_fractional_scale_manager_v1>    FractionalScaleManager;
        public Named<zwp_idle_inhibit_manager_v1>       IdleInhibitManager;
        public libdecor*                                LibdecorContext;
        public Named<zwp_pointer_constraints_v1>        PointerConstraints;
        public Named<zwp_pointer_gestures_v1>           PointerGestures;
        public zwp_primary_selection_device_manager_v1* PrimarySelectionDeviceManager;
        public wl_registry*                             Registry;
        public Named<zwp_relative_pointer_manager_v1>   RelativePointerManager;
        public Named<wl_shm>                            Shm;
        public Named<xdg_system_bell_v1>                SystemBell;
        public Named<zwp_tablet_manager_v2>             TabletManager;
        public Named<zwp_text_input_manager_v3>         TextInputManager;
        public Named<wp_viewporter>                     Viewporter;
        public Named<xdg_wm_base>                       WmBase;
        #endregion

        #region 4-bytes
        private UnsafeLock @lock;

        public int UnscaledCursorSize = 24;
        #endregion

        public RegistryState()
        {
            this.outputs      = [];
            this.screenStates = [];
            this.seats        = [];

            this.Cursors = new(Cursor.Length);
        }

        public void Dispose()
        {
            this.outputs.Dispose();
            this.screenStates.Dispose();
            this.seats.Dispose();

            this.Cursors.Dispose();
        }

        public void AddOutput(Pointer<wl_output> output)
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                this.outputs.Add(output);
            }
        }

        public void AddScreenState(uint name, Pointer<ScreenState> screenState)
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                this.screenStates.Add(name, screenState);
            }
        }

        public void AddSeat(Pointer<wl_seat> seat)
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                this.seats.Add(seat);
            }
        }

        public NativeArray<Pointer<wl_output>> GetOutputs()
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                return this.outputs.ToNativeArray();
            }
        }

        public NativeArray<Pointer<ScreenState>> GetScreenStates()
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                return this.screenStates.Values.ToNativeArray();
            }
        }

        public NativeArray<Pointer<wl_seat>> GetSeats()
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                return this.seats.ToNativeArray();
            }
        }

        public bool RemoveScreenState(uint name, out Pointer<ScreenState> screenState)
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                return this.screenStates.Remove(name, out screenState);
            }
        }
    }
}
#endif
