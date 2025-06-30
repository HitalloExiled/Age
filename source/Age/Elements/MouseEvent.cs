using Age.Core.Extensions;
using Age.Platforms.Display;

namespace Age.Elements;

public struct MouseEvent
{
    public required EventTarget Target;
    public MouseButton    Button;
    public float          Delta;
    public MouseKeyStates KeyStates;
    public MouseButton    PrimaryButton;
    public ushort         X;
    public ushort         Y;

    public bool Indirect;

    public readonly bool IsPrimaryButtonPressed => this.Button == this.PrimaryButton;

    public readonly bool IsHoldingPrimaryButton =>
        this.PrimaryButton == MouseButton.Left && this.KeyStates.HasFlags(MouseKeyStates.LeftButton)
        || this.PrimaryButton == MouseButton.Right && this.KeyStates.HasFlags(MouseKeyStates.RightButton);
}
