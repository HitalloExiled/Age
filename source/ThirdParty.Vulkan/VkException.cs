using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public class VkException(VkResult result) : Exception($"Vulkan Error: {result}")
{
    public VkResult Result { get; } = result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(VkResult result)
    {
        if (result != VkResult.Success)
        {
            throw new VkException(result);
        }
    }
}
