namespace Age.Numerics;

public record struct UVRect
{
    public static UVRect Normalized => new(new(0, 0), new(1, 0), new(1, 1), new(0, 1));

    public Point<float> P1;
    public Point<float> P2;
    public Point<float> P3;
    public Point<float> P4;

    public UVRect() { }

    public UVRect(Point<float> p1, Point<float> p2, Point<float> p3, Point<float> p4) : this()
    {
        this.P1 = p1;
        this.P2 = p2;
        this.P3 = p3;
        this.P4 = p4;
    }
}
