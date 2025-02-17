using System.Runtime.CompilerServices;

namespace Age.Numerics.Converters;

public static class Bit
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Combine(byte b1, byte b2, byte b3, byte b4) =>
        ((uint)b4 << 24) | ((uint)b3 << 16) | ((uint)b2 << 8) | b1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Combine(ushort u1, ushort u2, ushort u3, ushort u4) =>
        ((ulong)u4 << 48) | ((ulong)u3 << 32) | ((ulong)u2 << 16) | u1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split(uint value, out byte b1, out byte b2, out byte b3, out byte b4)
    {
        b1 = (byte)(value & 0xFF);
        b2 = (byte)((value >> 8)  & 0xFF);
        b3 = (byte)((value >> 16) & 0xFF);
        b4 = (byte)((value >> 24) & 0xFF);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Split(ulong value, out ushort u1, out ushort u2, out ushort u3, out ushort u4)
    {
        u1 = (ushort)(value & 0xFFFF);
        u2 = (ushort)((value >> 16) & 0xFFFF);
        u3 = (ushort)((value >> 32) & 0xFFFF);
        u4 = (ushort)((value >> 48) & 0xFFFF);
    }
}
