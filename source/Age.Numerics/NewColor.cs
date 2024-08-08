namespace Age.Numerics;

public record struct NewColor
{
    public static NewColor Black    => new();
    public static NewColor Blue     => new(0, 0, 255);
    public static NewColor Cyan     => new(0, 255, 255);
    public static NewColor Green    => new(0, 255, 0);
    public static NewColor Margenta => new(255, 0, 255);
    public static NewColor Red      => new(255, 0, 0);
    public static NewColor White    => new(255, 255, 255);
    public static NewColor Yellow   => new(255, 255, 0);

    private uint value = 0b_11111111_00000000_00000000_00000000;

    public byte R
    {
        readonly get => (byte)(this.value & 255);
        set => this.value |= value;
    }

    public byte G
    {
        readonly get => (byte)(this.value >> 8 & 255);
        set => this.value = (this.value & 255) | value;
    }

    public byte B
    {
        readonly get => (byte)(this.value >> 16 & 255);
        set => this.value = (this.value & 255) | value;
    }

    public byte A
    {
        readonly get => (byte)(this.value >> 24 & 255);
        set => this.value = (this.value & 255) | value;
    }

    public NewColor() { }

    public NewColor(uint value) =>
        this.value = value;

    public NewColor(byte r, byte g, byte b, byte a = 255) =>
        this.value = (uint)(r | (g << 8) | (b << 16) | (a << 24));

    public NewColor(float r, float g, float b, float a = 1) : this((byte)(Math.Min(r, 1) * 255), (byte)(Math.Min(g, 1) * 255), (byte)(Math.Min(b, 1) * 255), (byte)(Math.Min(a, 1) * 255))
    { }

    public override readonly string ToString() =>
        $"#{Convert.ToString(this.value, 16)}";

    public static NewColor operator +(NewColor color, float value) =>
        new(color.R + value, color.G + value, color.B + value, color.A + value);

    public static NewColor operator +(NewColor left, NewColor right) =>
        new(left.R + right.R, left.G + right.G, left.B + right.B, left.A + right.A);

    public static NewColor operator -(NewColor color, float value) =>
        new(color.R - value, color.G - value, color.B - value, color.A - value);

    public static NewColor operator -(NewColor left, NewColor right) =>
        new(left.R - right.R, left.G - right.G, left.B - right.B, left.A - right.A);

    public static NewColor operator *(NewColor color, float value) =>
        new(color.R * value, color.G * value, color.B * value, color.A * value);

    public static NewColor operator *(NewColor left, NewColor right) =>
        new(left.R * right.R, left.G * right.G, left.B * right.B, left.A * right.A);

    public static NewColor operator /(NewColor color, float value) =>
        new(color.R / value, color.G / value, color.B / value, color.A / value);

    public static NewColor operator /(NewColor left, NewColor right) =>
        new(left.R / right.R, left.G / right.G, left.B / right.B, left.A / right.A);

    public static implicit operator uint(NewColor value) => value.value;

    public static implicit operator NewColor(uint value) => new(value);
}
