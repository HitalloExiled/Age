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

    public required Point<short> Relative;
    public required Point<short> Velocity;

    public required float ScrollDelta;

    public required bool LeftHanded;

    public readonly bool IsPrimaryButtonPressed =>
        this.LeftHanded ? this.Button == MouseButton.Right : this.Button == MouseButton.Left;

    public readonly bool IsHoldingPrimaryButton =>
        this.LeftHanded ? this.PressedButtons.HasFlags(MouseButton.Right) : this.PressedButtons.HasFlags(MouseButton.Left);
}
