using Age.Numerics.Converters;

namespace Age.Tests.Numerics.Converters;

public class BitTest
{
    private static void AssertIt(Span<byte> actual, Span<byte> expected) =>
        Assert.True(actual.SequenceEqual(expected));

    private static void AssertIt(Span<ushort> actual, Span<ushort> expected) =>
        Assert.True(actual.SequenceEqual(expected));

    [Fact]
    public void Combine32()
    {
        var actual   = Bit.Combine(0x0, 0xFF, 0x0, 0xFF);
        var expected = BitConverter.ToUInt32([0x0, 0xFF, 0x0, 0xFF]);

        Assert.Equal(actual, expected);
    }

    [Fact]
    public void Combine64()
    {
        var value    = Bit.Combine(0x0, 0xFFFF, 0x0, 0xFFFF);
        var expected = BitConverter.ToUInt64([0x0, 0x0, 0xFF, 0xFF, 0x0, 0x0, 0xFF, 0xFF]);

        Assert.Equal(value, expected);
    }

    [Fact]
    public void Split32()
    {
        var value = BitConverter.ToUInt32([0x0, 0xFF, 0x0, 0xFF]);

        Bit.Split(value, out var r, out var g, out var b, out var a);

        AssertIt([r, g, b, a], [0x0, 0xFF, 0x0, 0xFF]);
    }

    [Fact]
    public void Split64()
    {
        var value = BitConverter.ToUInt64([0x0, 0x0, 0xFF, 0xFF, 0x0, 0x0, 0xFF, 0xFF]);

        Bit.Split(value, out var r, out var g, out var b, out var a);

        AssertIt([r, g, b, a], [0x0, 0xFFFF, 0x0, 0xFFFF]);
    }
}
