using Age.Extensions;

namespace Age.Elements;

public record struct TextLine(uint Start, uint Length)
{
    public readonly uint End => (this.Start + this.Length).ClampSubtract(1);

    public override readonly string ToString() =>
        $"[{this.Start}..{this.End}]";
}
