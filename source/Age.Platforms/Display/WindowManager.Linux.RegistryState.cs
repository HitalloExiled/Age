#if LINUX

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Platforms.Linux.LibDecor;
using Age.Platforms.Linux.LibWaylandClient;
using Age.Platforms.Linux.LibWaylandCursor;

namespace Age.Platforms.Display;

public unsafe sealed partial class WindowManager
{
    private struct RegistryState : IDisposable
    {
        #region 8-bytes
        private NativeList<Pointer<wl_seat>>   seats;

        public NativeArray<Pointer<wl_cursor>> Cursors;

        public Named<xdg_activation_v1>                 Activation;
        public WindowState*                             ActiveWindow;
        public Named<wl_compositor>                     Compositor;
        public wl_seat*                                 CurrentSeat;
        public Named<wp_cursor_shape_manager_v1>        CursorShapeManager;
        public CursorState*                             CursorState;
        public wl_cursor_theme*                         CursorTheme;
        public byte*                                    CursorThemeName;
        public Named<wl_data_device_manager>            DataDeviceManager;
        public Named<zxdg_decoration_manager_v1>        DecorationManager;
        public wl_display*                              Display;
        public Named<wp_fractional_scale_manager_v1>    FractionalScaleManager;
        public Named<zwp_idle_inhibit_manager_v1>       IdleInhibitManager;
        public KeyboardState*                           KeyboardState;
        public libdecor*                                LibdecorContext;
        public Named<zwp_pointer_constraints_v1>        PointerConstraints;
        public zwp_primary_selection_device_manager_v1* PrimarySelectionDeviceManager;
        public wl_registry*                             Registry;
        public Named<zwp_relative_pointer_manager_v1>   RelativePointerManager;
        public Named<wl_shm>                            Shm;
        public Named<xdg_system_bell_v1>                SystemBell;
        public Named<wp_viewporter>                     Viewporter;
        public Named<xdg_wm_base>                       WmBase;
        #endregion

        #region 4-bytes
        private UnsafeLock @lock;

        public int DoubleClikInterval;
        public int UnscaledCursorSize = 24;
        #endregion

        #region 1-bytes
        public bool LeftHandedMouse;
        #endregion

        public RegistryState()
        {
            this.seats = [];

            this.Cursors = new(CURSOR_LENGTH);
        }

        public static RegistryState* Allocate() =>
            NativeMemory.Alloc(new RegistryState());

        public static void Free(RegistryState* registryState)
        {
            registryState->Dispose();

            NativeMemory.Free(registryState);
        }

        public void Dispose()
        {
            using var seats = this.GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                lib_wayland_client.wl_seat_destroy(seat);

                SeatState.Free(seatState);
            }

            this.DisposeCursoState();
            this.DisposeKeyboardState();

            NativeMemory.Free(this.CursorThemeName);

            lib_wayland_cursor.wl_cursor_theme_destroy(this.CursorTheme);

            if (this.IdleInhibitManager != default)
            {
                idle_inhibit.zwp_idle_inhibit_manager_v1_destroy(this.IdleInhibitManager);
            }

            if (this.PointerConstraints != default)
            {
                pointer_constraints.zwp_pointer_constraints_v1_destroy(this.PointerConstraints);
            }

            if (this.RelativePointerManager != default)
            {
                relative_pointer.zwp_relative_pointer_manager_v1_destroy(this.RelativePointerManager);
            }

            if (this.Activation != default)
            {
                xdg_activation.xdg_activation_v1_destroy(this.Activation);
            }

            if (this.SystemBell != default)
            {
                xdg_system_bell.xdg_system_bell_v1_destroy(this.SystemBell);
            }

            if (this.DecorationManager != default)
            {
                xdg_decoration.zxdg_decoration_manager_v1_destroy(this.DecorationManager);
            }

            if (this.CursorShapeManager != default)
            {
                cursor_shape.wp_cursor_shape_manager_v1_destroy(this.CursorShapeManager);
            }

            if (this.FractionalScaleManager != default)
            {
                fractional_scale.wp_fractional_scale_manager_v1_destroy(this.FractionalScaleManager);
            }

            if (this.Viewporter != default)
            {
                viewporter.wp_viewporter_destroy(this.Viewporter);
            }

            if (this.WmBase != default)
            {
                xdg_shell.xdg_wm_base_destroy(this.WmBase);
            }

            if (this.Shm != default)
            {
                lib_wayland_client.wl_shm_destroy(this.Shm);
            }

            if (this.Compositor != default)
            {
                lib_wayland_client.wl_compositor_destroy(this.Compositor);
            }

            if (this.Registry != default)
            {
                lib_wayland_client.wl_registry_destroy(this.Registry);
            }

            if (this.Display != default)
            {
                lib_wayland_client.wl_display_disconnect(this.Display);
            }

            this.seats.Dispose();

            this.Cursors.Dispose();
        }

        public void DisposeCursoState()
        {
            if (this.CursorState != null)
            {
                this.CursorState->Dispose();

                NativeMemory.Free(this.CursorState);

                this.CursorState = null;
            }
        }

        public void DisposeKeyboardState()
        {
            if (this.KeyboardState != null)
            {
                this.KeyboardState->Dispose();

                NativeMemory.Free(this.KeyboardState);

                this.KeyboardState = null;
            }
        }

        public void AddSeat(Pointer<wl_seat> seat)
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                this.seats.Add(seat);
            }
        }

        public NativeArray<Pointer<wl_seat>> GetSeats()
        {
            using (UnsafeLock.Lock(ref this.@lock))
            {
                return this.seats.ToNativeArray();
            }
        }
    }
}
#endif
