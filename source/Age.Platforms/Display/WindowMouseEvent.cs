using Age.Core.Extensions;
using Age.Numerics;

namespace Age.Platforms.Display;

public record struct WindowMouseEvent
{
    public required ushort X;
    public required ushort Y;

    public required MouseButton Button;
    public required Modifier    Modifiers;
    public required MouseButton PressedButtons;
    public required MouseButton PrimaryButton;

    public required Point<ushort> Relative;
    public required Point<ushort> Velocity;

    public required float ScrollDelta;

    public readonly bool IsPrimaryButtonPressed => this.Button == this.PrimaryButton;

    public readonly bool IsHoldingPrimaryButton =>
        (this.PrimaryButton == MouseButton.Left && this.PressedButtons.HasFlags(MouseButton.Left))
        || (this.PrimaryButton == MouseButton.Right && this.PressedButtons.HasFlags(MouseButton.Right));
}
