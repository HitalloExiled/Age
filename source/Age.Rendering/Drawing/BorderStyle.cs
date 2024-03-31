using Age.Numerics;
using Age.Rendering.Interfaces;

namespace Age.Rendering.Drawing;

public record BorderStyle : IMergeable<BorderStyle>
{
    public event Action? Changed;

    private TrackedValue<Color> color;
    private TrackedValue<uint>  radius;
    private TrackedValue<uint>  size;

    public Color Color  { get => this.color.Value;  set => this.color.Value  = value; }
    public uint  Radius { get => this.radius.Value; set => this.radius.Value = value; }
    public uint  Size   { get => this.size.Value;   set => this.size.Value   = value; }

    public BorderStyle()
    {
        this.color.Changed  += this.Changed;
        this.radius.Changed += this.Changed;
        this.size.Changed   += this.Changed;
    }

    public static BorderStyle Merge(BorderStyle left, BorderStyle right) =>
        new()
        {
            color  = TrackedValue.GetValue(left.color,  right.color),
            radius = TrackedValue.GetValue(left.radius, right.radius),
            size   = TrackedValue.GetValue(left.size,   right.size),
        };

    public static void Update(BorderStyle source, BorderStyle target)
    {
        TrackedValue.SetValue(ref target.color,  source.color);
        TrackedValue.SetValue(ref target.radius, source.radius);
        TrackedValue.SetValue(ref target.size,   source.size);;
    }

    public BorderStyle Merge(BorderStyle other) =>
        Merge(other, this);

    public void Update(BorderStyle source) =>
        Update(source, this);
}
