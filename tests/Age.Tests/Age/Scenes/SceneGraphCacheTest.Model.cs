using Age.Commands;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class SceneGraphCacheTest
{
    private class Model : Spatial3D
    {
        public override string NodeName => nameof(Model);

        public Model() { }
        public Model(string name) => this.Name = name;

        internal void AddCommands(ReadOnlySpan<Command3D> commands)
        {
            foreach (var command in commands)
            {
                this.AddCommand(command);
            }
        }
    }
}
