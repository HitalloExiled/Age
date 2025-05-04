namespace Age.Styling;

public record struct PointUnit
{
    public Unit X;
    public Unit Y;

    public PointUnit(Unit value) : this(value, value)
    { }

    public PointUnit(Unit x, Unit y)
    {
        this.X = x;
        this.Y = y;
    }

    public override readonly string ToString() =>
        $"X: {this.X}, Y: {this.Y}";
}
