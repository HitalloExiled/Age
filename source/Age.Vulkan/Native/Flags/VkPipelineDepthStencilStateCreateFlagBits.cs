namespace Age.Vulkan.Native.Flags
{
    /// <summary>
    /// Bitmask specifying additional depth/stencil state information.
    /// </summary>
    /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
    [Flags]
    public enum VkPipelineDepthStencilStateCreateFlagBits
    {
        /// <summary>
        /// Indicates that access to the depth aspects of depth/stencil and input attachments will have implicit framebuffer-local memory dependencies.
        /// </summary>
        /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
        VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT = 0x00000001,

        /// <summary>
        /// Indicates that access to the stencil aspects of depth/stencil and input attachments will have implicit framebuffer-local memory dependencies.
        /// </summary>
        /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
        VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT = 0x00000002,

        /// <inheritdoc cref="VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT" />
        /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
        VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_ARM = VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_DEPTH_ACCESS_BIT_EXT,

        /// <inheritdoc cref="VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT" />
        /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
        VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_ARM = VK_PIPELINE_DEPTH_STENCIL_STATE_CREATE_RASTERIZATION_ORDER_ATTACHMENT_STENCIL_ACCESS_BIT_EXT,
    }
}
