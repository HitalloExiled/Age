namespace Age.Vulkan.Types;

/// <summary>
/// Structure specifying a two-dimensional subregion.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public struct VkRect2D
{
    /// <summary>
    /// A <see cref="VkOffset2D"/> specifying the rectangle offset.
    /// </summary>
    public VkOffset2D offset;

    /// <summary>
    /// A <see cref="VkExtent2D"/> specifying the rectangle extent.
    /// </summary>
    public VkExtent2D extent;
}
