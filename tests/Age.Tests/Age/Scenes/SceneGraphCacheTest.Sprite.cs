using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private class Sprite : Spatial2D
    {
        public override string NodeName => nameof(Sprite);

        public Sprite() { }
        public Sprite(string name) => this.Name = name;
    }
}
