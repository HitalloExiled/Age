#if LINUX
using Age.Numerics;
using Age.Platforms.Linux.LibWaylandClient;

namespace Age.Platforms.Display;

public unsafe partial class WindowManager
{
    private struct PointerData
    {
		#region 8-bytes
        public WindowState* WindowState;
        public WindowState* LastWindowState;
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
}
#endif
