namespace Age.Styling;

public record struct SizeUnit
{
    public Unit? Width;
    public Unit? Height;

    public SizeUnit(Unit? value) : this(value, value)
    { }

    public SizeUnit(Unit? width, Unit? height)
    {
        this.Width  = width;
        this.Height = height;
    }

    public override readonly string ToString() =>
        $"Width: {this.Width}, Height: {this.Height}";
}
