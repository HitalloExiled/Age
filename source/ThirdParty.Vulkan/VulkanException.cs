using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public class VulkanException(Result result) : Exception($"Vulkan Error: {result}")
{
    public Result Result { get; } = result;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(Result result)
    {
        if (result != Result.Success)
        {
            throw new VulkanException(result);
        }
    }
}
