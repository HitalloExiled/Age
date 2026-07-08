#if LINUX

using System.Diagnostics;
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

        public Named<xdg_activation_v1>                       Activation;
        public WindowState*                                   ActiveWindow;
        public Named<wl_compositor>                           Compositor;
        public SeatState*                                     CurrentSeatState;
        public Named<wp_cursor_shape_manager_v1>              CursorShapeManager;
        public wl_cursor_theme*                               CursorTheme;
        public NativeString                                   CursorThemeName;
        public Named<wl_data_device_manager>                  DataDeviceManager;
        public Named<zxdg_decoration_manager_v1>              DecorationManager;
        public wl_display*                                    Display;
        public Named<wp_fractional_scale_manager_v1>          FractionalScaleManager;
        public Named<zwp_idle_inhibit_manager_v1>             IdleInhibitManager;
        public libdecor*                                      LibdecorContext;
        public Named<zwp_pointer_constraints_v1>              PointerConstraints;
        public Named<zwp_primary_selection_device_manager_v1> PrimarySelectionDeviceManager;
        public wl_registry*                                   Registry;
        public Named<zwp_relative_pointer_manager_v1>         RelativePointerManager;
        public Named<wl_shm>                                  Shm;
        public Named<xdg_system_bell_v1>                      SystemBell;
        public Named<wp_viewporter>                           Viewporter;
        public NativeList<Pointer<WindowState>>               Windows;
        public Named<xdg_wm_base>                             WmBase;
        #endregion

        #region 4-bytes
        private UnsafeLock @lock;

        public Cursor Cursor;
        public int    CursorScale = 1;
        public int    DoubleClikInterval;
        public int    UnscaledCursorSize = 24;
        #endregion

        #region 1-bytes
        public bool CursorVisible = true;
        public bool LeftHandedMouse;
        #endregion

        public RegistryState()
        {
            this.seats = [];

            this.BoundGlobalNames = new(16);
            this.Cursors          = new(CURSOR_LENGTH);
            this.Windows          = [];
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
            this.CursorThemeName.Dispose();

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

            this.BoundGlobalNames.Dispose();
            this.Cursors.Dispose();
            this.Windows.Dispose();
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

        public void TryDestroyNamedGlobal(uint name)
        {
            if (!this.BoundGlobalNames.Remove(name))
            {
                Debug.WriteLine("Received remove event for unknown Wayland global!");
            }

            if (name == this.Shm.Name)
            {
                if (this.Shm != default)
                {
                    lib_wayland_client.wl_shm_destroy(this.Shm);

                    this.Shm = default;
                }
            }
            else if (name == this.Compositor.Name)
            {
                if (this.Compositor != default)
                {
                    lib_wayland_client.wl_compositor_destroy(this.Compositor);

                    this.Compositor = default;
                }
            }
            else if (name == this.DataDeviceManager.Name)
            {
                if (this.DataDeviceManager != default)
                {
                    lib_wayland_client.wl_data_device_manager_destroy(this.DataDeviceManager);

                    this.DataDeviceManager = default;
                }

                foreach (var seat in this.seats)
                {
                    var seatState = GetSeatState(seat);

                    Debug.Assert(seatState != null);

                    if (seatState->DataDevice != null)
                    {
                        lib_wayland_client.wl_data_device_destroy(seatState->DataDevice);

                        seatState->DataDevice = null;
                    }
                }
            }
            else if (name == this.WmBase.Name)
            {
                if (this.WmBase != default)
                {
                    xdg_shell.xdg_wm_base_destroy(this.WmBase);

                    this.WmBase = default;
                }
            }
            else if (name == this.Viewporter.Name)
            {
                if (this.Viewporter != default)
                {
                    viewporter.wp_viewporter_destroy(this.Viewporter);

                    this.Viewporter = default;
                }

                foreach (var window in this.Windows)
                {
                    var windowState = window.Value;

                    if (windowState->Viewport != null)
                    {
                        viewporter.wp_viewport_destroy(windowState->Viewport);

                        windowState->Viewport = null;
                    }
                }
            }
            else if (name == this.CursorShapeManager.Name)
            {
                if (this.CursorShapeManager != default)
                {
                    cursor_shape.wp_cursor_shape_manager_v1_destroy(this.CursorShapeManager);

                    this.CursorShapeManager = default;
                }

                foreach (var seat in this.seats)
                {
                    var seatState = GetSeatState(seat);

                    Debug.Assert(seatState != null);

                    if (seatState->CursorShapeDevice != null)
                    {
                        cursor_shape.wp_cursor_shape_device_v1_destroy(seatState->CursorShapeDevice);

                        seatState->CursorShapeDevice = null;
                    }
                }
            }
            else if (name == this.FractionalScaleManager.Name)
            {
                if (this.FractionalScaleManager != default)
                {
                    fractional_scale.wp_fractional_scale_manager_v1_destroy(this.FractionalScaleManager);

                    this.FractionalScaleManager = default;
                }

                foreach (var window in this.Windows)
                {
                    var windowState = window.Value;

                    if (windowState->FractionalScale != default)
                    {
                        fractional_scale.wp_fractional_scale_v1_destroy(windowState->FractionalScale);

                        windowState->FractionalScale = null;
                    }
                }
            }
            else if (name == this.DecorationManager.Name)
            {
                if (this.DecorationManager != default)
                {
                    xdg_decoration.zxdg_decoration_manager_v1_destroy(this.DecorationManager);

                    this.DecorationManager = default;
                }
            }
            else if (name == this.SystemBell.Name)
            {
                if (this.SystemBell != default)
                {
                    xdg_system_bell.xdg_system_bell_v1_destroy(this.SystemBell);

                    this.SystemBell = default;
                }
            }
            else if (name == this.Activation.Name)
            {
                if (this.Activation != default)
                {
                    xdg_activation.xdg_activation_v1_destroy(this.Activation);

                    this.Activation = default;
                }
            }
            else if (name == this.PrimarySelectionDeviceManager.Name)
            {
                if (this.PrimarySelectionDeviceManager != default)
                {
                    primary_selection.zwp_primary_selection_device_manager_v1_destroy(this.PrimarySelectionDeviceManager);

                    this.PrimarySelectionDeviceManager = default;
                }

                foreach (var seat in this.seats)
                {
                    var seatState = GetSeatState(seat);

                    Debug.Assert(seatState != null);

                    if (seatState->PrimarySelectionDevice != null)
                    {
                        primary_selection.zwp_primary_selection_device_v1_destroy(seatState->PrimarySelectionDevice);

                        seatState->PrimarySelectionDevice = null;
                    }

                    if (seatState->PrimarySelectionSource != null)
                    {
                        primary_selection.zwp_primary_selection_source_v1_destroy(seatState->PrimarySelectionSource);

                        seatState->PrimarySelectionSource = null;
                    }

                    if (seatState->PrimarySelectionOffer != null)
                    {
                        OfferState.Free(GetOfferState(seatState->PrimarySelectionOffer));

                        primary_selection.zwp_primary_selection_offer_v1_destroy(seatState->PrimarySelectionOffer);

                        seatState->PrimarySelectionOffer = null;
                    }
                }
            }
            else if (name == this.RelativePointerManager.Name)
            {
                if (this.RelativePointerManager != default)
                {
                    relative_pointer.zwp_relative_pointer_manager_v1_destroy(this.RelativePointerManager);

                    this.RelativePointerManager = default;
                }

                foreach (var seat in this.seats)
                {
                    var seatState = GetSeatState(seat);

                    Debug.Assert(seatState != null);

                    if (seatState->RelativePointer != null)
                    {
				        relative_pointer.zwp_relative_pointer_v1_destroy(seatState->RelativePointer);

				        seatState->RelativePointer = null;
			        }
                }
            }
            else if (name == this.PointerConstraints.Name)
            {
                if (this.PointerConstraints != default)
                {
                    pointer_constraints.zwp_pointer_constraints_v1_destroy(this.PointerConstraints);

                    this.PointerConstraints = default;
                }

                foreach (var seat in this.seats)
                {
                    var seatState = GetSeatState(seat);

                    Debug.Assert(seatState != null);

                    if (seatState->RelativePointer != null)
                    {
                        relative_pointer.zwp_relative_pointer_v1_destroy(seatState->RelativePointer);

                        seatState->RelativePointer = null;
                    }

                    if (seatState->LockedPointer != null)
                    {
                        pointer_constraints.zwp_locked_pointer_v1_destroy(seatState->LockedPointer);

                        seatState->LockedPointer = null;
                    }

                    if (seatState->ConfinedPointer != null)
                    {
                        pointer_constraints.zwp_confined_pointer_v1_destroy(seatState->ConfinedPointer);

                        seatState->ConfinedPointer = null;
                    }
                }
            }
            else if (name == this.IdleInhibitManager.Name)
            {
                if (this.IdleInhibitManager != default)
                {
                    idle_inhibit.zwp_idle_inhibit_manager_v1_destroy(this.IdleInhibitManager);

                    this.IdleInhibitManager = default;
                }
            }
            else
            {
                using var seats = this.GetSeats();

                for (var i = 0; i < seats.Length; i++)
                {
                    var seatState = GetSeatState(seats[i]);

                    Debug.Assert(seatState != null);

                    if (seatState->Seat.Name == name)
                    {
                        if (seatState->Seat != default)
                        {
                            lib_wayland_client.wl_seat_destroy(seatState->Seat);
                        }

                        if (seatState->DataDevice != null)
                        {
                            lib_wayland_client.wl_data_device_destroy(seatState->DataDevice);
                        }

                        SeatState.Free(seatState);

                        this.seats.RemoveAt(i);

                        break;
                    }
                }
            }
        }
    }
}
#endif
