using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public class VkException : Exception
{
    public VkException()
    { }

    public VkException(string? message) : base(message)
    { }

    public VkException(string? message, Exception? innerException) : base(message, innerException)
    { }

    public VkException(VkResult result) : base($"Vulkan Error: {result}") =>
        this.Result = result;

    public VkResult Result { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Check(VkResult result)
    {
        if (result != VkResult.Success)
        {
            throw new VkException(result);
        }
    }
}
