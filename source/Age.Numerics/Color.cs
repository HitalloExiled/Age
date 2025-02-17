using Age.Numerics.Converters;

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

    public Color(uint value)
    {
        Bit.Split(value, out var r, out var g, out var b, out var a);

        this.R = r / (float)0xFF;
        this.G = g / (float)0xFF;
        this.B = b / (float)0xFF;
        this.A = a / (float)0xFF;
    }

    public Color(ulong value)
    {
        Bit.Split(value, out var r, out var g, out var b, out var a);

        this.R = r / (float)0xFFFF;
        this.G = g / (float)0xFFFF;
        this.B = b / (float)0xFFFF;
        this.A = a / (float)0xFFFF;
    }

    public readonly byte[] ToByteArray() =>
        [
            (byte)Math.Ceiling(this.R * 255),
            (byte)Math.Ceiling(this.G * 255),
            (byte)Math.Ceiling(this.B * 255),
            (byte)Math.Ceiling(this.A * 255),
        ];

    public override readonly string ToString() =>
        $"#{Convert.ToString((uint)this, 16).PadLeft(8, '0')}";

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

    public static implicit operator Color(uint value)  => new(value);
    public static implicit operator Color(ulong value) => new(value);

    public static implicit operator uint(Color value) =>
        Bit.Combine(
            (byte)float.Min(value.R * byte.MaxValue, byte.MaxValue),
            (byte)float.Min(value.G * byte.MaxValue, byte.MaxValue),
            (byte)float.Min(value.B * byte.MaxValue, byte.MaxValue),
            (byte)float.Min(value.A * byte.MaxValue, byte.MaxValue)
        );

    public static implicit operator ulong(Color value) =>
        Bit.Combine(
            (ushort)float.Min(value.R * ushort.MaxValue, ushort.MaxValue),
            (ushort)float.Min(value.G * ushort.MaxValue, ushort.MaxValue),
            (ushort)float.Min(value.B * ushort.MaxValue, ushort.MaxValue),
            (ushort)float.Min(value.A * ushort.MaxValue, ushort.MaxValue)
        );
}
