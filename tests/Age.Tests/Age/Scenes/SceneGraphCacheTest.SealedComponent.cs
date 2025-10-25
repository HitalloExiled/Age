using Age.Commands;
using Age.Elements;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private class SealedComponent : Element
    {
        public override string NodeName => nameof(SealedComponent);

        public SealedComponent() : this(null) { }
        public SealedComponent(string? name = null)
        {
            this.AttachShadowTree();

            this.ShadowTree.Children =
            [
                TreeFactory<Component, Command2D, ComponentCommand>.Linear(1, 3, 6, 3, $"{name}#.01"),
                TreeFactory<Component, Command2D, ComponentCommand>.Linear(1, 2, 6, 2, $"{name}#.02"),
                new Slot(),
            ];
        }
    }
}
