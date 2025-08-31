using Age.Elements;

namespace Age.Scene;

public sealed partial class RenderTree
{
    private record struct VirtualRelation(Element VirtualParent, uint VirtualChildIndex);
}

