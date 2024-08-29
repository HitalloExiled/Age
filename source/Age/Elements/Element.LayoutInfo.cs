using Age.Numerics;

namespace Age.Elements;

public abstract partial class Element
{
    private struct LayoutInfo
    {
        public readonly HashSet<Element> Dependents = [];

        public Size<uint> AvaliableSpace;
        public Size<uint> ContentDynamicSize;
        public Size<uint> ContentStaticSize;
        public Size<uint> Size;
        public RectEdges  Border;
        public RectEdges  Margin;
        public RectEdges  Padding;
        public Dependency ContentDependent;
        public Dependency ParentDependent;
        public uint       HightestChild;
        public uint       RenderableNodesCount;

        public readonly Size<uint> TotalSize =>
            new(
                this.Size.Width + this.Padding.Horizontal + this.Border.Horizontal,
                this.Size.Height + this.Padding.Vertical + this.Border.Vertical
            );

        public LayoutInfo() { }
    }

    private record struct RectEdges
    {
        public uint Top;
        public uint Right;
        public uint Bottom;
        public uint Left;

        public readonly uint Horizontal => this.Left + this.Right;
        public readonly uint Vertical   => this.Top + this.Bottom;
    }
}
