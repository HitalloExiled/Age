using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private record struct NodeRange(Node node, ShortRange subtree, CommandRange.Variant color = default, CommandRange.Variant encode = default)
    {
        public Node         Node         = node;
        public ShortRange Subtree      = subtree;
        public CommandRange CommandRange = new(color, encode);

        public override readonly string ToString() =>
            $"{this.Node} - Subtree: {this.Subtree}, {this.CommandRange}";
    }
}
