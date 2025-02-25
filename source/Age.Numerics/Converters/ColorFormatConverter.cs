using System.Runtime.CompilerServices;

namespace Age.Numerics.Converters;

public static class ColorFormatConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RGBAtoBGRA(uint color)
    {
        Bit.Split(color, out var r, out var g, out var b, out var a);

        return Bit.Combine(b, g, r, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RGBAtoBGRA(ulong color)
    {
        Bit.Split(color, out var r, out var g, out var b, out var a);

        return Bit.Combine(b, g, r, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RGBA64toRGBA32(ulong color)
    {
        Bit.Split(color, out var r16, out var g16, out var b16, out var a16);

        var r = (byte)((r16 + 128) / 257);
        var g = (byte)((g16 + 128) / 257);
        var b = (byte)((b16 + 128) / 257);
        var a = (byte)((a16 + 128) / 257);

        return Bit.Combine(r, g, b, a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RGBA32toRGBA64(ulong color)
    {
        Bit.Split(color, out var r8, out var g8, out var b8, out var a8);

        var r = (ushort)(r8 * 257);
        var g = (ushort)(g8 * 257);
        var b = (ushort)(b8 * 257);
        var a = (ushort)(a8 * 257);

        return Bit.Combine(r, g, b, a);
    }
}
