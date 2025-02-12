using Age.Extensions;

namespace Age.Elements.Layouts;

public record struct TextLine(uint Start, uint Lenght)
{
    public readonly uint End => (this.Start + this.Lenght).ClampSubtract(1);

    public override readonly string ToString() =>
        $"[{this.Start}..{this.End}]";
}
