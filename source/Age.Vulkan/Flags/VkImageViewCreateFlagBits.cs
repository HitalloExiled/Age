namespace Age.Vulkan.Flags;

/// <summary>
/// Bitmask specifying additional parameters of an image view.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkImageViewCreateFlagBits
{
    /// <summary>
    /// Specifies that the fragment density map will be read by device during <see cref="VK_PIPELINE_STAGE_FRAGMENT_DENSITY_PROCESS_BIT_EXT"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_IMAGE_VIEW_CREATE_FRAGMENT_DENSITY_MAP_DYNAMIC_BIT_EXT = 0x00000001,

    /// <summary>
    /// Specifies that the fragment density map will be read by the host during <see cref="Vk.EndCommandBuffer"/> for the primary command buffer that the render pass is recorded into
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_IMAGE_VIEW_CREATE_DESCRIPTOR_BUFFER_CAPTURE_REPLAY_BIT_EXT = 0x00000004,

    /// <summary>
    /// Specifies that the image view can be used with descriptor buffers when capturing and replaying (e.g. for trace capture and replay), see <see cref="VkOpaqueCaptureDescriptorDataCreateInfoEXT"/> for more detail.
    /// </summary>
    /// <remarks>Provided by VK_EXT_fragment_density_map2</remarks>
    VK_IMAGE_VIEW_CREATE_FRAGMENT_DENSITY_MAP_DEFERRED_BIT_EXT = 0x00000002,
}
