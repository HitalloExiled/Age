using Age.Numerics;

namespace Age.Elements;

public abstract partial class Element
{
    private struct LayoutInfo
    {
        public readonly HashSet<Element> Dependents = [];

        public Size<uint> AvaliableSpace;
        public Size<uint> Border;
        public Size<uint> ContentDynamicSize;
        public Size<uint> ContentStaticSize;
        public Dependency Dependencies;
        public uint       HightestChild;
        public uint       RenderableNodesCount;
        public Size<uint> Size;

        public LayoutInfo() { }
    }
}
