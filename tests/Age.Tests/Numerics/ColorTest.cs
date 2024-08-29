using Age.Numerics;

namespace Age.Tests.Numerics;

public class ColorTest
{
    [Fact]
    public void CreateFromBytes()
    {
        var color = new NewColor(255, 0, 0);
        var raw  = (uint)color;

        Assert.Equal(255, color.R);
        Assert.Equal(0,   color.G);
        Assert.Equal(0,   color.B);
        Assert.Equal(255, color.A);

        Assert.Equal(0b_11111111_00000000_00000000_11111111, raw);
    }

    [Fact]
    public void CreateFromFloat()
    {
        var color = new NewColor(0, 1f, 0.5f);
        var raw   = (uint)color;

        Assert.Equal(0,   color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(127, color.B);
        Assert.Equal(255, color.A);

        Assert.Equal(0b_11111111_01111111_11111111_00000000, raw);
    }

    [Fact]
    public void SetRGBAComponent()
    {
        var color = new NewColor();

        color.R = 127;

        Assert.Equal(0b_11111111_00000000_00000000_01111111, (uint)color);

        Assert.Equal(127,   color.R);
        Assert.Equal(0,     color.G);
        Assert.Equal(0,     color.B);
        Assert.Equal(255,   color.A);

        color.G = 64;

        Assert.Equal(0b_11111111_00000000_01000000_01111111, (uint)color);

        Assert.Equal(127,   color.R);
        Assert.Equal(64,    color.G);
        Assert.Equal(0,     color.B);
        Assert.Equal(255,   color.A);
    }
}
