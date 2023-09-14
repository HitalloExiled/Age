using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Platform.Windows.Native.Types;

namespace Age.Platform.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public record struct WNDPROC(nint Value = default)
    {
        public delegate LRESULT Function(HWND hwnd, WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam);

        public WNDPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(WNDPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static explicit operator Function(WNDPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static explicit operator WNDPROC(Function value) => new(value);
    }
}
