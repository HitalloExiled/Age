namespace Age.Vulkan.Native.Flags;

/// <summary>
/// Bitmask specifying how execution and memory dependencies are formed
/// </summary>
[Flags]
public enum VkDependencyFlagBits
{
    /// <summary>
    /// Specifies that dependencies will be <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-framebuffer-regions">framebuffer-local</see>.
    /// </summary>
    VK_DEPENDENCY_BY_REGION_BIT = 0x00000001,

    /// <summary>
    /// Specifies that dependencies will be <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-view-local-dependencies">view-local</see>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_DEPENDENCY_DEVICE_GROUP_BIT = 0x00000004,

    /// <summary>
    /// Specifies that dependencies are <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#synchronization-device-local-dependencies">non-device-local</see>.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_DEPENDENCY_VIEW_LOCAL_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the render pass will write to and read from the same image using the <see cref="VkImageLayout.VK_IMAGE_LAYOUT_ATTACHMENT_FEEDBACK_LOOP_OPTIMAL_EXT"/> layout.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_DEPENDENCY_FEEDBACK_LOOP_BIT_EXT = 0x00000008,

    /// <inheritdoc cref="VK_DEPENDENCY_VIEW_LOCAL_BIT" />
    /// <remarks>Provided by VK_KHR_multiview</remarks>
    VK_DEPENDENCY_VIEW_LOCAL_BIT_KHR = VK_DEPENDENCY_VIEW_LOCAL_BIT,

    /// <inheritdoc cref="VK_DEPENDENCY_DEVICE_GROUP_BIT" />
    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_DEPENDENCY_DEVICE_GROUP_BIT_KHR = VK_DEPENDENCY_DEVICE_GROUP_BIT,
}
