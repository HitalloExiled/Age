using Age.Elements;
using Age.Scenes;

using CommandRangeSegments = (Age.Scenes.CommandRange.Segment Color, Age.Scenes.CommandRange.Segment Index);

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private record struct NodeRange(Node node, SubtreeRange subtree, CommandRangeSegments commandRangeSegments1 = default, CommandRangeSegments commandRangeSegments2 = default)
    {
        public Node         Node          = node;
        public SubtreeRange Subtree       = subtree;
        public CommandRange CommandRange1 = new(commandRangeSegments1.Color, commandRangeSegments1.Index);
        public CommandRange CommandRange2 = new(commandRangeSegments2.Color, commandRangeSegments2.Index);

        public override readonly string ToString() =>
            this.Node switch
            {
                Viewport => $"{this.Node} - Subtree: {this.Subtree}, 2D: {{ {this.CommandRange1} }}, 3D: {{ {this.CommandRange2} }}",
                Element  => $"{this.Node} - Subtree: {this.Subtree}, Pre {{ {this.CommandRange1} }}, Post: {{ {this.CommandRange2} }}",
                _        => $"{this.Node} - Subtree: {this.Subtree}, {this.CommandRange1}",
            };
    }
}
