using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ThirdParty.Vulkan;

[DebuggerDisplay("{Value}")]
public readonly struct VkPfn<T>(T value) where T : Delegate
{
    private readonly nint handle = Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkPfn<T> value) => value.handle;
}
