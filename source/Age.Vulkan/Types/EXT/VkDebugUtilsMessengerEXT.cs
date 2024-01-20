using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Age.Vulkan.Types.EXT;

/// <summary>
/// <para>Opaque handle to a debug messenger object.</para>
/// <para>The debug messenger will provide detailed feedback on the application’s use of Vulkan when events of interest occur. When an event of interest does occur, the debug messenger will submit a debug message to the debug callback that was provided during its creation. Additionally, the debug messenger is responsible with filtering out debug messages that the callback is not interested in and will only provide desired debug messages.</para>
/// </summary>
[DebuggerDisplay("{Value}")]
public record struct VkDebugUtilsMessengerEXT(nint Value)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator VkDebugUtilsMessengerEXT(nint value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator nint(VkDebugUtilsMessengerEXT value) => value.Value;
}
