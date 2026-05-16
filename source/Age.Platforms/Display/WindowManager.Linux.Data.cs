#if LINUX

using Age.Core;
using Age.Core.Collections;
using Age.Platforms.Linux.LibDecor;
using Age.Platforms.Linux.Wayland;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private struct RegistryState : IDisposable
    {
        public libdecor*                                    LibdecorContext;
        public Named<wl_compositor>                         Compositor;
        public Named<wl_data_device_manager>                DataDeviceManager;
        public Named<wl_shm>                                Shm;
        public Named<wp_cursor_shape_manager_v1>            CursorShapeManager;
        public Named<wp_fractional_scale_manager_v1>        FractionalScaleManager;
        public Named<wp_viewporter>                         Viewporter;
        public Named<xdg_activation_v1>                     Activation;
        public Named<xdg_system_bell_v1>                    SystemBell;
        public Named<xdg_wm_base>                           WmBase;
        public Named<zwp_idle_inhibit_manager_v1>           IdleInhibitManager;
        public Named<zwp_pointer_constraints_v1>            PointerConstraints;
        public Named<zwp_pointer_gestures_v1>               PointerGestures;
        public Named<zwp_relative_pointer_manager_v1>       RelativePointerManager;
        public Named<zwp_tablet_manager_v2>                 TabletManager;
        public Named<zwp_text_input_manager_v3>             TextInputManager;
        public Named<zxdg_decoration_manager_v1>            DecorationManager;
        public NativeDictionary<uint, Pointer<ScreenState>> ScreenStates;
        public NativeList<Pointer<wl_output>>               Outputs;
        public NativeList<Pointer<wl_seat>>                 Seats;
        public UnsafeLock                                   Lock;
        public wl_display*                                  Display;
        public wl_registry*                                 Registry;
        public wl_seat*                                     CurrentSeat;
        public zwp_primary_selection_device_manager_v1*     PrimarySelectionDeviceManager;

        public RegistryState()
        {
            this.Seats        = [];
            this.Outputs      = [];
            this.ScreenStates = [];
        }

        public void Dispose()
        {
            this.Seats.Dispose();
            this.Outputs.Dispose();
            this.ScreenStates.Dispose();
        }
    }
}
#endif
