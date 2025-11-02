using Age.Commands;
using Age.Elements;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private class SealedComponent : Element
    {
        public override string NodeName => nameof(SealedComponent);

        public SealedComponent(string name)
        {
            this.Name      = name;
            var shadowRoot = new Component()
            {
                Name = $"{name}#",
                Children =
                [
                    TreeFactory.Linear<Component, Command2D, ComponentCommand>(1, 3, 6, 3, $"{name}#.01"),
                    TreeFactory.Linear<Component, Command2D, ComponentCommand>(1, 2, 6, 2, $"{name}#.02"),
                ]
            };

            this.AttachShadowRoot(shadowRoot);
        }
    }
}
