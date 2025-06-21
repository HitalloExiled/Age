using Age.Core.Extensions;

namespace Age.Platforms.Display;

public struct MouseEvent
{
    public ushort         X;
    public ushort         Y;
    public MouseButton    Button;
    public MouseButton    PrimaryButton;
    public MouseKeyStates KeyStates;
    public float          Delta;

    public readonly bool IsPrimaryButtonPressed => this.Button == this.PrimaryButton;

    public readonly bool IsHoldingPrimaryButton =>
        (this.PrimaryButton == MouseButton.Left && this.KeyStates.HasFlags(MouseKeyStates.LeftButton))
        || (this.PrimaryButton == MouseButton.Right && this.KeyStates.HasFlags(MouseKeyStates.RightButton));
};

public struct ContextEvent
{
    public ushort X;
    public ushort Y;
    public ushort ScreenX;
    public ushort ScreenY;
};
