using System.Runtime.CompilerServices;

namespace Age.Platform.Windows.Native;

internal unsafe static partial class User32
{
    public readonly record struct LPMONITORINFO(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator MONITORINFO*(LPMONITORINFO value) => (MONITORINFO*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator LPMONITORINFO(MONITORINFO* value) => new(new(value));
    }
}
