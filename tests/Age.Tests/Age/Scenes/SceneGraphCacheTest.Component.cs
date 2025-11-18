using Age.Elements;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private sealed class Component : Element
    {
        public override string NodeName => nameof(Component);

        public Component() : this(null) { }
        public Component(string? name = null) => this.Name = name;
    }
}
