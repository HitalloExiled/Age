using Age.Numerics;
using Age.Rendering.Scene;
using Age.Rendering.Scene.Resources;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;

namespace Age.Editor;

public class DemoScene : Scene3D
{
    public override string NodeName { get; } = nameof(DemoScene);

    public DemoScene()
    {
        this.Camera = new()
        {
            FoV  = Angle.Radians(45f),
            Near = 0.1f,
            Far  = 10,
        };

        this.AppendChild(this.Camera);
        this.AppendChild(LoadMesh());
    }

    private static Mesh LoadMesh()
    {
        var texture = Texture.Load(Path.Join(AppContext.BaseDirectory, "Assets", "Textures", "viking_room.png"));
        var data    = WavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Assets", "Models", "viking_room.obj"));

        var uniqueVertices = new Dictionary<Vertex, uint>();

        var indices  = new List<uint>();
        var vertices = new List<Vertex>();

        foreach (var obj in data.Objects)
        {
            foreach (var item in obj.Mesh.Faces.SelectMany(x => x.Indices))
            {
                var pos      = data.Attributes.Vertices[item.Index];
                var color    = item.ColorIndex > -1 ? data.Attributes.Colors[item.ColorIndex] : new(1, 0, 0);
                var texCoord = data.Attributes.TexCoords[item.TexCoordIndex];

                texCoord.Y = 1 - texCoord.Y;

                var vertex = new Vertex(pos, color, texCoord);

                if (!uniqueVertices.TryGetValue(vertex, out var index))
                {
                    uniqueVertices[vertex] = index = (uint)vertices.Count;
                    vertices.Add(vertex);
                }

                indices.Add(index);
            }
        }

        return new([..vertices], [..indices])
        {
            Material = new()
            {
                Diffuse = texture
            }
        };
    }
}
