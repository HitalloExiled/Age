namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkClearRect.html">VkClearRect</see>
/// </summary>
public struct VkClearRect
{
    public VkRect2D Rect;
    public uint     BaseArrayLayer;
    public uint     LayerCount;
}
