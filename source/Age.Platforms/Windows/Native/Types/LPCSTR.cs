using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPCSTR(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte*(LPCSTR value) => (byte*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPCSTR(byte* value) => new((nint)value);
}
