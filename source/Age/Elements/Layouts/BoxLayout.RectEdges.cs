namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    public record struct RectEdges
    {
        public uint Top;
        public uint Right;
        public uint Bottom;
        public uint Left;

        public readonly uint Horizontal => this.Left + this.Right;
        public readonly uint Vertical   => this.Top + this.Bottom;
    }
}
