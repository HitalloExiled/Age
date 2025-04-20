namespace Age.Elements;

public record struct RectEdges
{
    public uint Top;
    public uint Right;
    public uint Bottom;
    public uint Left;

    public readonly uint Horizontal => this.Left + this.Right;
    public readonly uint Vertical   => this.Top + this.Bottom;

    public override readonly string ToString() =>
        $"{{ Top: {this.Top}, Right: {this.Right}, Bottom: {this.Bottom}, Left: {this.Left} }}";
}
