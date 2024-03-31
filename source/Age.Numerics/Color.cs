namespace Age.Numerics;

public record struct Color
{
    public static Color Black    => new();
    public static Color Blue     => new(0, 0, 1);
    public static Color Cyan     => new(0, 1, 1);
    public static Color Green    => new(0, 1, 0);
    public static Color Margenta => new(1, 0, 1);
    public static Color Red      => new(1, 0, 0);
    public static Color White    => new(1, 1, 1);
    public static Color Yellow   => new(1, 1, 0);

    public float R;
    public float G;
    public float B;
    public float A = 1;

    public Color() { }

    public Color(float r, float g, float b, float a = 1)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    public readonly byte[] ToByteArray() =>
        [
            (byte)Math.Ceiling(this.R * 255),
            (byte)Math.Ceiling(this.G * 255),
            (byte)Math.Ceiling(this.B * 255),
            (byte)Math.Ceiling(this.A * 255),
        ];
}
