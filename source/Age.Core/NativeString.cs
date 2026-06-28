using Age.Core.Extensions;
using System.Runtime.InteropServices;
using System.Text;

namespace Age.Core;

public readonly unsafe struct NativeString(string? value) : IDisposable, IEquatable<NativeString>
{
    private readonly byte* buffer = MemoryMarshal.CreateUTF8StringBuffer(value);

    public readonly void Dispose()
    {
        if (this.buffer != null)
        {
            NativeMemory.Free(this.buffer);
        }
    }

    public bool Equals(NativeString other) =>
        this.buffer == other.buffer;

    public override bool Equals(object? obj) =>
        obj is NativeString nativeString && this.Equals(nativeString);

    public override int GetHashCode() =>
        ((nint)this.buffer).GetHashCode();

    public override readonly string? ToString() =>
        this.buffer == null ? null : Encoding.GetStringFromNullTerminated(this.buffer);

    public static implicit operator byte*(NativeString unmanageString) => unmanageString.buffer;

    public static bool operator ==(NativeString left, NativeString right) => left.Equals(right);
    public static bool operator !=(NativeString left, NativeString right) => !left.Equals(right);
}
