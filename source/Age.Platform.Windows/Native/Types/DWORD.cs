using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native.Types;

[DebuggerDisplay("{Value}")]
public readonly record struct DWORD(uint Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(DWORD value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator DWORD(uint value) => new(value);
}
