using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ThirdParty.Shaderc;

[DebuggerDisplay("{Value}")]
public readonly struct NativeDelegate<T>(T value) where T : Delegate
{
    private readonly nint handle = Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(NativeDelegate<T> value) => value.handle;
}
