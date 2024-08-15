using Age.Numerics;

namespace Age.Elements;

public abstract partial class Element
{
    private struct LayoutInfo
    {
        public readonly List<ContainerNode> NodesToDistribute        = [];
        public readonly List<Element>       PendingChildCalculations = [];

        public Size<uint>         AvaliableSpace;
        public Size<uint>         Border;
        public uint               HightestChild;
        public PendingCalculation PendingCalculation;
        public Size<uint>         Size;

        public LayoutInfo() { }
    }
}
