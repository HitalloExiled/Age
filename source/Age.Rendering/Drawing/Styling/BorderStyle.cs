using Age.Numerics;

namespace Age.Rendering.Drawing;

public record BorderStyle : Trackable<BorderStyle>
{
    internal override event Action? Changed;

    private TrackedValue<Color> color;
    private TrackedValue<uint>  radius;
    private TrackedValue<uint>  size;

    public Color Color
    {
        get => this.color.Value;
        set => this.color.Value = value;
    }

    public uint Radius
    {
        get => this.radius.Value;
        set => this.radius.Value = value;
    }

    public uint Size
    {
        get => this.size.Value;
        set => this.size.Value = value;
    }



    public BorderStyle()
    {
        this.color.Changed  += this.Changed;
        this.radius.Changed += this.Changed;
        this.size.Changed   += this.Changed;
    }

    public override BorderStyle Merge(BorderStyle other) =>
        new()
        {
            color  = GetValue(this.color,  other.color),
            radius = GetValue(this.radius, other.radius),
            size   = GetValue(this.size,   other.size),
        };

    public override void Update(BorderStyle source)
    {
        SetValue(ref this.color,  source.color);
        SetValue(ref this.radius, source.radius);
        SetValue(ref this.size,   source.size);
    }
}
