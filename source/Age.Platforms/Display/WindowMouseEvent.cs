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

    public required Point<ushort> Relative;
    public required Point<ushort> Velocity;

    public required float ScrollDelta;

    public required bool LeftHanded;

    public readonly bool IsPrimaryButtonPressed =>
        (this.LeftHanded && this.Button == MouseButton.Left) || this.Button == MouseButton.Right;

    public readonly bool IsHoldingPrimaryButton =>
        (this.LeftHanded && this.PressedButtons.HasFlags(MouseButton.Left)) || this.PressedButtons.HasFlags(MouseButton.Right);
}
