using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private record struct NodeRange(Node node, ShortRange subtree, CommandRange commandRange = default)
    {
        public Node         Node         = node;
        public ShortRange   Subtree      = subtree;
        public CommandRange CommandRange = commandRange;

        public override readonly string ToString() =>
            $"{this.Node} - Subtree: {this.Subtree}, CommandRange: {this.CommandRange}";
    }
}
