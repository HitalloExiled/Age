namespace Age.Elements;

public record struct RectEdges
{
    public ushort Top;
    public ushort Right;
    public ushort Bottom;
    public ushort Left;

    public readonly uint Horizontal => (uint)(this.Left + this.Right);
    public readonly uint Vertical   => (uint)(this.Top + this.Bottom);

    public override readonly string ToString() =>
        $"{{ Top: {this.Top}, Right: {this.Right}, Bottom: {this.Bottom}, Left: {this.Left} }}";
}
