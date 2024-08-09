using Age.Core.Extensions;
using Age.Numerics;
using Age.Scene;
using Age.Resources;
using Age.Platforms.Display;

using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;

using static Age.Rendering.Shaders.GeometryShader;

namespace Age.Editor;

public class DemoScene : Scene3D
{
    public override string NodeName { get; } = nameof(DemoScene);
    private readonly Mesh axis;
    private readonly Mesh mesh;

    public Camera3D FreeCamera  { get; }
    public Camera3D BlueCamera  { get; }
    public Camera3D RedCamera   { get; }
    public Camera3D GreenCamera { get; }

    private float angle;

    public DemoScene()
    {
        this.FreeCamera = new()
        {
            Transform = Transform3D.Translated(new(10, 0, 10)),
        };

        this.RedCamera = new()
        {
            Transform = Transform3D.Translated(new(10, 0, 0)),
        };

        this.GreenCamera = new()
        {
            Transform = Transform3D.Translated(new(0, 10, 0)),
        };

        this.BlueCamera = new()
        {
            Transform = Transform3D.Translated(new(0, 0, 10)),
        };

        this.AppendChild(this.FreeCamera);
        this.AppendChild(this.RedCamera);
        this.AppendChild(this.GreenCamera);
        this.AppendChild(this.BlueCamera);

        this.axis = LoadMesh("Axis.obj", "Axis.png");
        this.mesh = LoadMesh("viking_room_2.obj", "viking_room.png");

        this.AppendChild(this.axis);
        this.AppendChild(this.mesh);

        this.FreeCamera.LookAt(this.mesh, Vector3<float>.Up);
        this.RedCamera.LookAt(this.mesh, Vector3<float>.Up);
        this.GreenCamera.LookAt(this.mesh, Vector3<float>.Right);
        this.BlueCamera.LookAt(this.mesh, Vector3<float>.Up);

        this.FreeCamera.Transform = this.FreeCamera.Transform with
        {
            Position = this.FreeCamera.Transform.Position with
            {
                Y = 1,
            }
        };

        this.RedCamera.Transform = this.RedCamera.Transform with
        {
            Position = this.RedCamera.Transform.Position with
            {
                Y = 1,
            }
        };

        this.BlueCamera.Transform = this.BlueCamera.Transform with
        {
            Position = this.BlueCamera.Transform.Position with
            {
                Y = 1,
            }
        };
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

        return new(vertices.AsSpan(), indices.AsSpan())
        {
            Name     = modelName,
            Material = new()
            {
                Diffuse = texture
            }
        };
    }

    public override void Update(double deltaTime)
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

        var input = Vector3<float>.Zero;

        if (Input.IsKeyPressed(Key.A))
        {
            input.X = -1;
        }
        else if (Input.IsKeyPressed(Key.D))
        {
            input.X = 1;
        }

        if (Input.IsKeyPressed(Key.Q))
        {
            input.Y = -1;
        }
        else if (Input.IsKeyPressed(Key.E))
        {
            input.Y = 1;
        }

        if (Input.IsKeyPressed(Key.W))
        {
            input.Z = 1;
        }
        else if (Input.IsKeyPressed(Key.S))
        {
            input.Z = -1;
        }

        if (Input.IsKeyJustPressed(Key.Space))
        {
            Console.WriteLine("Jump!!!");
        }

        input = input.Normalized;

        var mouseWheel = Input.GetMouseWheel();

        var movement = -this.FreeCamera.Transform.Forward * input.Z + this.FreeCamera.Transform.Right * input.X + Vector3<float>.Up * input.Y;

        this.FreeCamera.Transform = this.FreeCamera.Transform with
        {
            Position = this.FreeCamera.Transform.Position + movement * (float)deltaTime,
            Rotation = this.FreeCamera.Transform.Rotation * new Quaternion<float>(Vector3<float>.Up, Angle.Radians(5 * mouseWheel))
        };
    }
}
