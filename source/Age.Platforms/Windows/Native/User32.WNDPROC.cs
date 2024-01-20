using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Platforms.Windows.Native.Types;

namespace Age.Platforms.Windows.Native;

internal static partial class User32
{
    [DebuggerDisplay("{Value}")]
    public record struct WNDPROC(nint Value = default)
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate LRESULT Function(HWND hwnd, WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam);

        public WNDPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(WNDPROC value) => value.Value;
    }
}
