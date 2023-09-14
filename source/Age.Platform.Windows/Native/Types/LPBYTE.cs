using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public unsafe readonly record struct LPBYTE(nint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator BYTE*(LPBYTE value) => (BYTE*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LPBYTE(BYTE* value) => new((nint)value);
}
