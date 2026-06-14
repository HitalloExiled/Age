using Age.Numerics;
using Age.Platforms.Display;

namespace Age.Elements.Events;

public readonly struct MouseEvent
{
    #region 8-bytes
    public Element Target { get; }
    #endregion

    #region 4-bytes
    private readonly WindowMouseEvent windowMouseEvent;
    #endregion

    #region 1-byte
    public bool Indirect { get; }
    #endregion

    public MouseButton   Button                 => this.windowMouseEvent.Button;
    public float         Delta                  => this.windowMouseEvent.ScrollDelta;
    public bool          IsHoldingPrimaryButton => this.windowMouseEvent.IsHoldingPrimaryButton;
    public bool          IsPrimaryButtonPressed => this.windowMouseEvent.IsPrimaryButtonPressed;
    public Modifier      Modifiers              => this.windowMouseEvent.Modifiers;
    public MouseButton   PressedButtons         => this.windowMouseEvent.PressedButtons;
    public MouseButton   PrimaryButton          => this.windowMouseEvent.PrimaryButton;
    public Point<ushort> Relative               => this.windowMouseEvent.Relative;
    public Point<ushort> Velocity               => this.windowMouseEvent.Velocity;
    public ushort        X                      => this.windowMouseEvent.X;
    public ushort        Y                      => this.windowMouseEvent.Y;

    internal MouseEvent(Element target) =>
        this.Target = target;

    internal MouseEvent(Element target, in WindowMouseEvent windowMouseEvent, bool indirect)
    {
        this.Target           = target;
        this.windowMouseEvent = windowMouseEvent;
        this.Indirect         = indirect;
    }
}
