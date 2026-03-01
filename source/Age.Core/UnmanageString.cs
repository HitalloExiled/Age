using System.Runtime.InteropServices;
using Age.Core.Extensions;

namespace Age.Core;

public unsafe ref struct UnmanagedString(string value)
{
    private readonly byte* buffer = MemoryMarshal.CreateUTF8StringBuffer(value);
    public readonly string Value => value;

    public readonly void Dispose() =>
        NativeMemory.Free(this.buffer);

    public override readonly string ToString() => value;

    public static implicit operator byte*(UnmanagedString unmanageString) => unmanageString.buffer;
}
