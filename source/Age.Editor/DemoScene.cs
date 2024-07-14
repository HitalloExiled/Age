using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Scene;
using Age.Rendering.Scene.Resources;
using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;

using static Age.Rendering.Shaders.GeometryShader;
using Age.Core;

namespace Age.Editor;

public class DemoScene : Scene3D
{
    public override string NodeName { get; } = nameof(DemoScene);
    private readonly Mesh axis;
    private readonly Mesh mesh;

    public Camera3D BlueCamera  { get; }
    public Camera3D RedCamera   { get; }
    public Camera3D GreenCamera { get; }

    private float angle;

    public DemoScene()
    {
        this.RedCamera = new()
        {
            Transform = Transform3D.Translated(new(10, 0, 0)),
            // Transform = new Matrix4x4<double>(
            //     1.0,  0.0,  0.0, 0.0,
            //     0.0,  1.0,  0.0, 0.0,
            //     0.0,  0.0,  1.0, 0.0,
            //     5.0,  0.0,  0.0, 1.0
            // ).Cast<float>(),
        };

        this.GreenCamera = new()
        {
            Transform = Transform3D.Translated(new(0, 10, 0)),
            // Transform = new Matrix4x4<double>(
            //     1.0,  0.0,  0.0, 0.0,
            //     0.0,  1.0,  0.0, 0.0,
            //     0.0,  0.0,  1.0, 0.0,
            //     0.0,  5.0,  0.0, 1.0
            // ).Cast<float>(),
        };

        this.BlueCamera = new()
        {
            Transform = Transform3D.Translated(new(0, 0, 10)),
            // Transform = new Matrix4x4<double>(
            //     -1.0,  0.0,  0.0, 0.0,
            //      0.0,  1.0,  0.0, 0.0,
            //      0.0,  0.0, -1.0, 0.0,
            //      0.0,  0.0,  5.0, 1.0
            // ).Cast<float>(),
        };

        this.AppendChild(this.RedCamera);
        this.AppendChild(this.GreenCamera);
        this.AppendChild(this.BlueCamera);

        this.axis = LoadMesh("Axis.obj", "Axis.png");
        this.mesh = LoadMesh("viking_room_2.obj", "viking_room.png");

        this.AppendChild(this.axis);
        this.AppendChild(this.mesh);

        // this.AppendChild(this.mesh = LoadMesh("cone.obj", "Grid.png"));

        this.axis.Transform = new(new(0, 1, 0), new Quaternion<float>(Vector3<float>.Up, Angle.Radians(-45)), Vector3<float>.One);

        this.RedCamera.LookAt(this.mesh, Vector3<float>.Up);
        this.GreenCamera.LookAt(this.mesh, Vector3<float>.Right);
        this.BlueCamera.LookAt(this.mesh, Vector3<float>.Up);

        Logger.Info($"RedCamera: {this.RedCamera.Transform}");
        Logger.Info($"RedCamera Inverse {this.RedCamera.Transform.Inverse()}");

        Logger.Info($"GreenCamera: {this.GreenCamera.Transform}");
        Logger.Info($"GreenCamera Inverse: {this.GreenCamera.Transform.Inverse()}");

        Logger.Info($"BlueCamera: {this.BlueCamera.Transform}");
        Logger.Info($"BlueCamera Inverse: {this.BlueCamera.Transform.Inverse()}");
    }

    private static Mesh LoadMesh(string modelName, string textureName)
    {
        var texture = Texture.Load(Path.Join(AppContext.BaseDirectory, "Assets", "Textures", textureName));
        var data    = WavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Assets", "Models", modelName));

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

        // vertices =
        // [
        //     new(new(-1, -1, 1), default, new(1,  1)),
        //     new(new( 1, -1, 1), default, new(0,  1)),
        //     new(new( 1,  1, 1), default, new(0,  0)),
        // ];

        // indices = [2, 1, 0];

        return new(vertices.AsSpan(), indices.AsSpan())
        {
            Name     = modelName,
            Material = new()
            {
                Diffuse = texture
            }
        };
    }

    protected override void OnUpdate(double deltaTime)
    {
        var angle = this.angle += 10 * (float)deltaTime;

        if (angle > 360)
        {
            angle -= 360;
        }

        this.axis.Transform = this.axis.Transform with
        {
            Rotation = new(Vector3<float>.Up, Angle.Radians(angle))
        };
    }
}
