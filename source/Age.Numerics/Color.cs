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

    public override readonly string ToString() =>
        $"#{Convert.ToString((uint)this, 16)}";

    public readonly Color WithAlpha(float value) =>
        new(this.R, this.G, this.B, value);

    public static Color operator +(Color color, float value) =>
        new(color.R + value, color.G + value, color.B + value, color.A + value);

    public static Color operator +(Color left, Color right) =>
        new(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);

    public static Color operator -(Color color, float value) =>
        new(color.R - value, color.G - value, color.B - value, color.A - value);

    public static Color operator -(Color left, Color right) =>
        new(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);

    public static Color operator *(Color color, float value) =>
        new(color.R * value, color.G * value, color.B * value, color.A * value);

    public static Color operator *(Color left, Color right) =>
        new(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);

    public static Color operator /(Color color, float value) =>
        new(color.R / value, color.G / value, color.B / value, color.A / value);

    public static Color operator /(Color left, Color right) =>
        new(left.R / right.R, left.G / right.G, left.B / right.B, left.A / right.A);

    public static implicit operator Color(uint value) =>
        new(
            (value       & 255) / 255f,
            (value >> 8  & 255) / 255f,
            (value >> 16 & 255) / 255f,
            (value >> 24 & 255) / 255f
        );

    public static implicit operator uint(Color value) =>
        (uint)Math.Min(value.R * 255, 255)
        | ((uint)Math.Min(value.G * 255, 255) <<  8)
        | ((uint)Math.Min(value.B * 255, 255) << 16)
        | ((uint)Math.Min(value.A * 255, 255) << 24);
}
