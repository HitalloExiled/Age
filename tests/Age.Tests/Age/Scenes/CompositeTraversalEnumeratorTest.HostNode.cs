using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class CompositeTraversalEnumeratorTest
{
    private class HostNode : Node
    {
        public override string NodeName => nameof(HostNode);

        public HostNode(string name)
        {
            this.Name = name;

            var shadowRoot = new TestNode()
            {
                Name     = $"{name}.#",
                Children =
                [
                    TreeFactory.Linear<TestNode>(2, 2, $"{name}.#.1")
                ]
            };

            this.AttachShadowRoot(shadowRoot);
        }
    }
}
