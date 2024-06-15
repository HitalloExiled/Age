using Age.Numerics;

namespace Age.Rendering.Scene.Resources;

public struct Vertex(Vector3<float> position, Color color, in Vector2<float> texCoord)
{
    public Vector3<float> Position = position;
    public Color          Color    = color;
    public Vector2<float> TexCoord = texCoord;
}
