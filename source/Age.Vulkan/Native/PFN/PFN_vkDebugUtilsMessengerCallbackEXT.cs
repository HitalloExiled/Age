using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT;

namespace Age.Vulkan.Native.PFN;

/// <summary>
/// Application-defined debug messenger callback function
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct PFN_vkDebugUtilsMessengerCallbackEXT(nint Value)
{
    /// <summary>
    /// The callback returns a <see cref="VkBool32"/>, which is interpreted in a layer-specified manner. The application should always return VK_FALSE. The VK_TRUE value is reserved for use in layer development.
    /// </summary>
    /// <param name="messageSeverity">specifies the <see cref="VkDebugUtilsMessageSeverityFlagBitsEXT"/> that triggered this callback.</param>
    /// <param name="messageTypes">a bitmask of <see cref="VkDebugUtilsMessageTypeFlagBitsEXT"/> specifying which type of event(s) triggered this callback.</param>
    /// <param name="pCallbackData">contains all the callback related data in the <see cref="VkDebugUtilsMessengerCallbackDataEXT"/> structure.</param>
    /// <param name="pUserData">the user data provided when the <see cref="VkDebugUtilsMessengerEXT"/> was created.</param>
    public unsafe delegate VkBool32 Function(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData);

    public PFN_vkDebugUtilsMessengerCallbackEXT(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(PFN_vkDebugUtilsMessengerCallbackEXT value) => value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator Function(PFN_vkDebugUtilsMessengerCallbackEXT value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static explicit operator PFN_vkDebugUtilsMessengerCallbackEXT(Function value) => new(value);
}
