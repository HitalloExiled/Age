using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkFreeFunction.html">PFN_vkFreeFunction</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkFreeFunction
{
    public unsafe delegate void* Function(void* pUserData, void* pMemory);

    private readonly nint handle;

    public PFN_vkFreeFunction(Function value) =>
        Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nint(PFN_vkFreeFunction value) => value.handle;
}
