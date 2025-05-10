using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Age.Core;

[InlineArray(SIZE)]
public struct MD5Hash
{
    private const int SIZE = 16;

    private byte element;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MD5Hash Create(ReadOnlySpan<byte> source)
    {
        var hash = new MD5Hash();

        MD5.HashData(source, hash);

        return hash;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Update(ReadOnlySpan<byte> source, ref MD5Hash hash) =>
        MD5.HashData(source, hash);

    public unsafe Span<byte> AsSpan() =>
        MemoryMarshal.CreateSpan(ref this.element, SIZE);

    public readonly unsafe ReadOnlySpan<byte> AsReadOnlySpan() =>
        MemoryMarshal.CreateReadOnlySpan(in this.element, SIZE);

    public bool Equals(MD5Hash other) =>
        this.AsSpan().SequenceEqual(other);

    public override bool Equals(object? obj) =>
        obj is MD5Hash key && this.Equals(key);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.AddBytes(this.AsSpan());

        return hashCode.ToHashCode();
    }

    public override readonly string ToString() =>
        Convert.ToHexStringLower(this.AsReadOnlySpan());

    public static bool operator ==(MD5Hash left, MD5Hash right) => left.Equals(right);

    public static bool operator !=(MD5Hash left, MD5Hash right) => !(left == right);
}
