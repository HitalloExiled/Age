using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/PFN_vkDebugUtilsMessengerCallbackEXT.html">PFN_vkDebugUtilsMessengerCallbackEXT</see>
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct PFN_vkDebugUtilsMessengerCallbackEXT
{
    public unsafe delegate VkBool32 Function(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData);

    private readonly nint handle;

    public PFN_vkDebugUtilsMessengerCallbackEXT(Function value) =>
        this.handle = Marshal.GetFunctionPointerForDelegate(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkDebugUtilsMessengerCallbackEXT value) => value.handle;
}
