#if LINUX
using System.Runtime.InteropServices;
using Age.Core.Collections;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct PointerData
    {
		#region 8-bytes
        public WindowState* PointedId;
        public WindowState* LastPointedId;
        #endregion

		#region 4-bytes
        public uint                   ButtonSerial;
        public uint                   ButtonTime;
        public Point<int>             DiscreteScrollVector120;
        public bool                   DoubleClickBegun;
        public Point<float>           LastPressedPosition;
        public uint                   MotionTime;
        public uint                   PinchScale = 1;
        public Point<float>           Position;
        public Vector2<float>         RelativeMotion;
        public uint                   RelativeMotionTime;
        public wl_pointer_axis_source ScrollType = wl_pointer_axis_source.WL_POINTER_AXIS_SOURCE_WHEEL;
        public Vector2<float>         Scroll;
        #endregion

        #region 2-byte
        public MouseButton LastButtonPressed;
        public MouseButton PressedButton;
        #endregion

        public PointerData()
        { }
    };

    private struct CursorState
    {
        #region 8-bytes
        public readonly SeatState* SeatState;

        public NativeDictionary<uint, CustomCursor> CustomCursors = [];

        public zwp_confined_pointer_v1*   ConfinedPointer;
        public wl_callback*               CursorFrameCallback;
        public wp_cursor_shape_device_v1* CursorShapeDevice;
        public wl_surface*                CursorSurface;
        public zwp_locked_pointer_v1*     LockedPointer;
        public wl_pointer*                Pointer;
        public zwp_relative_pointer_v1*   RelativePointer;

        public PointerData PointerData       = new();
        public PointerData PointerDataBuffer = new();
        #endregion

        #region 4-bytes
        public int  CursorScale;
        public uint CursorTimeMs;
        public uint PointerEnterSerial;
        #endregion

        #region 1-byte
        public Cursor Cursor;
        public bool   CursorVisible = true;
        public bool   DoubleClickBegun;
        #endregion

        private CursorState(SeatState* seatState) =>
            this.SeatState = seatState;

        public static CursorState* Allocate(SeatState* seatState) =>
            NativeMemory.Alloc(new CursorState(seatState));

        public static void Free(CursorState* cursorState)
        {
            cursorState->Dispose();

            NativeMemory.Free(cursorState);
        }

        public void Dispose()
        {
            if (this.CursorFrameCallback != null)
            {
                lib_wayland_client.wl_callback_destroy(this.CursorFrameCallback);

                this.CursorFrameCallback = null;
            }

            if (this.CursorSurface != null)
            {
                lib_wayland_client.wl_surface_destroy(this.CursorSurface);

                this.CursorSurface = null;
            }

            if (this.Pointer != null)
            {
                lib_wayland_client.wl_pointer_destroy(this.Pointer);

                this.Pointer = null;
            }

            if (this.CursorShapeDevice != null)
            {
                cursor_shape.wp_cursor_shape_device_v1_destroy(this.CursorShapeDevice);

                this.CursorShapeDevice = null;
            }

            if (this.RelativePointer != null)
            {
                relative_pointer.zwp_relative_pointer_v1_destroy(this.RelativePointer);

                this.RelativePointer = null;
            }

            if (this.ConfinedPointer != null)
            {
                pointer_constraints.zwp_confined_pointer_v1_destroy(this.ConfinedPointer);

                this.ConfinedPointer = null;
            }

            if (this.LockedPointer != null)
            {
                pointer_constraints.zwp_locked_pointer_v1_destroy(this.LockedPointer);

                this.LockedPointer = null;
            }

            this.CustomCursors.Dispose();
        }
    }
}
#endif
