using System.Runtime.CompilerServices;

namespace Age.Platforms.Windows.Native.Types;

public record struct WCHAR(char Value = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator char(WCHAR value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator WCHAR(char value) => new(value);
}
