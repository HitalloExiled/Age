using Age.Numerics;

namespace Age.Rendering;

public record struct Vertex
{
    public Point<float> Position = new();
    public Color        Color    = new();
    public Point<float> UV       = new();

    public Vertex() { }

    public Vertex(Point<float> position, Color color, Point<float> uv)
    {
        this.Position = position;
        this.Color    = color;
        this.UV       = uv;
    }
};
