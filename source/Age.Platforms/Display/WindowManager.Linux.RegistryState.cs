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

        public NativeHashSet<uint>             BoundGlobalNames;
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

            this.Cursors          = new(CURSOR_LENGTH);
            this.BoundGlobalNames = new(16);
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
            this.DisposeCursoState();
            this.DisposeKeyboardState();

            NativeMemory.Free(this.CursorThemeName);

            lib_wayland_cursor.wl_cursor_theme_destroy(this.CursorTheme);

            using var names = this.BoundGlobalNames.ToNativeArray();

            foreach (var name in names)
            {
                this.TryDestroyNamedGlobal(name);
            }

            using var seats = this.GetSeats();

            foreach (var seat in seats)
            {
                var seatState = GetSeatState(seat);

                lib_wayland_client.wl_seat_destroy(seat);

                SeatState.Free(seatState);
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
            this.BoundGlobalNames.Dispose();
        }

        public void TryDestroyNamedGlobal(uint name)
        {
            if (name == this.Shm.Name)
            {
                this.DisposeShm();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.Compositor.Name)
            {
                this.DisposeCompositor();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.DataDeviceManager.Name)
            {
                this.DisposeDataDeviceManager();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (this.RemoveSeat(name))
            {
                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.WmBase.Name)
            {
                this.DisposeWmBase();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.Viewporter.Name)
            {
                this.DisposeViewporter();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.CursorShapeManager.Name)
            {
                this.DisposeCursorShapeManager();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.FractionalScaleManager.Name)
            {
                this.DisposeFractionalScaleManager();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.DecorationManager.Name)
            {
                this.DisposeDecorationManager();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.SystemBell.Name)
            {
                this.DisposeSystemBell();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.Activation.Name)
            {
                this.DisposeActivation();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.RelativePointerManager.Name)
            {
                this.DisposeRelativePointerManager();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.PointerConstraints.Name)
            {
                this.DisposePointerConstraints();

                this.BoundGlobalNames.Remove(name);

                return;
            }

            if (name == this.IdleInhibitManager.Name)
            {
                this.DisposeIdleInhibitManager();

                this.BoundGlobalNames.Remove(name);
            }
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

        public void DisposeActivation()
        {
            if (this.Activation != default)
            {
                xdg_activation.xdg_activation_v1_destroy(this.Activation);

                this.Activation = default;
            }
        }

        public void DisposeCompositor()
        {
            if (this.Compositor != default)
            {
                lib_wayland_client.wl_compositor_destroy(this.Compositor);

                this.Compositor = default;
            }
        }

        public void DisposeCursorShapeManager()
        {
            if (this.CursorShapeManager != default)
            {
                if (this.CursorState != null && this.CursorState->CursorShapeDevice != null)
                {
                    cursor_shape.wp_cursor_shape_device_v1_destroy(this.CursorState->CursorShapeDevice);

                    this.CursorState->CursorShapeDevice = null;
                }

                cursor_shape.wp_cursor_shape_manager_v1_destroy(this.CursorShapeManager);

                this.CursorShapeManager = default;
            }
        }

        public void DisposeDataDeviceManager()
        {
            if (this.DataDeviceManager != default)
            {
                using var seats = this.GetSeats();

                foreach (var seat in seats)
                {
                    var seatState = GetSeatState(seat);

                    if (seatState == default)
                    {
                        continue;
                    }

                    if (seatState->DataDevice != null)
                    {
                        lib_wayland_client.wl_data_device_destroy(seatState->DataDevice);

                        seatState->DataDevice = null;
                    }

                    if (seatState->DataOfferSelection != null)
                    {
                        lib_wayland_client.wl_data_offer_destroy(seatState->DataOfferSelection);

                        seatState->DataOfferSelection = null;
                    }

                    if (seatState->DataSourceSelection != null)
                    {
                        lib_wayland_client.wl_data_source_destroy(seatState->DataSourceSelection);

                        seatState->DataSourceSelection = null;
                    }

                    if (seatState->ClipboardDataSourceData != null)
                    {
                        NativeMemory.Free(seatState->ClipboardDataSourceData);

                        seatState->ClipboardDataSourceData = null;
                    }

                    seatState->ClipboardDataSourceLength = 0;
                }

                lib_wayland_client.wl_proxy_destroy((wl_proxy*)this.DataDeviceManager.Value);

                this.DataDeviceManager = default;
            }
        }

        public void DisposeDecorationManager()
        {
            if (this.DecorationManager != default)
            {
                xdg_decoration.zxdg_decoration_manager_v1_destroy(this.DecorationManager);

                this.DecorationManager = default;
            }
        }

        public void DisposeFractionalScaleManager()
        {
            if (this.FractionalScaleManager != default)
            {
                fractional_scale.wp_fractional_scale_manager_v1_destroy(this.FractionalScaleManager);

                this.FractionalScaleManager = default;
            }
        }

        public void DisposeIdleInhibitManager()
        {
            if (this.IdleInhibitManager != default)
            {
                idle_inhibit.zwp_idle_inhibit_manager_v1_destroy(this.IdleInhibitManager);

                this.IdleInhibitManager = default;
            }
        }

        public void DisposePointerConstraints()
        {
            if (this.PointerConstraints != default)
            {
                if (this.CursorState != null)
                {
                    if (this.CursorState->ConfinedPointer != null)
                    {
                        pointer_constraints.zwp_confined_pointer_v1_destroy(this.CursorState->ConfinedPointer);

                        this.CursorState->ConfinedPointer = null;
                    }

                    if (this.CursorState->LockedPointer != null)
                    {
                        pointer_constraints.zwp_locked_pointer_v1_destroy(this.CursorState->LockedPointer);

                        this.CursorState->LockedPointer = null;
                    }
                }

                pointer_constraints.zwp_pointer_constraints_v1_destroy(this.PointerConstraints);

                this.PointerConstraints = default;
            }
        }

        public void DisposeRelativePointerManager()
        {
            if (this.RelativePointerManager != default)
            {
                if (this.CursorState != null && this.CursorState->RelativePointer != null)
                {
                    relative_pointer.zwp_relative_pointer_v1_destroy(this.CursorState->RelativePointer);

                    this.CursorState->RelativePointer = null;
                }

                relative_pointer.zwp_relative_pointer_manager_v1_destroy(this.RelativePointerManager);

                this.RelativePointerManager = default;
            }
        }

        public void DisposeShm()
        {
            if (this.Shm != default)
            {
                lib_wayland_client.wl_shm_destroy(this.Shm);

                this.Shm = default;
            }
        }

        public void DisposeSystemBell()
        {
            if (this.SystemBell != default)
            {
                xdg_system_bell.xdg_system_bell_v1_destroy(this.SystemBell);

                this.SystemBell = default;
            }
        }

        public void DisposeViewporter()
        {
            if (this.Viewporter != default)
            {
                viewporter.wp_viewporter_destroy(this.Viewporter);

                this.Viewporter = default;
            }
        }

        public void DisposeWmBase()
        {
            if (this.WmBase != default)
            {
                xdg_shell.xdg_wm_base_destroy(this.WmBase);

                this.WmBase = default;
            }
        }

        public bool RemoveSeat(uint name)
        {
            using var seatsSpan = this.GetSeats();

            for (var i = 0; i < seatsSpan.Length; i++)
            {
                var seat = seatsSpan[i];

                var seatState = GetSeatState(seat);

                if (seatState != default && seatState->Seat.Name == name)
                {
                    if (this.CursorState != null && this.CursorState->SeatState == seatState)
                    {
                        this.DisposeCursoState();
                    }

                    if (this.KeyboardState != null && this.KeyboardState->SeatState == seatState)
                    {
                        this.DisposeKeyboardState();
                    }

                    lib_wayland_client.wl_seat_destroy(seat);

                    SeatState.Free(seatState);

                    using (UnsafeLock.Lock(ref this.@lock))
                    {
                        this.seats.RemoveAt(i);
                    }

                    if (this.CurrentSeat == seat)
                    {
                        this.CurrentSeat = default;
                    }

                    return true;
                }
            }

            return false;
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
