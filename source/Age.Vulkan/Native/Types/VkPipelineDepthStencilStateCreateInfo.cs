using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types
{
    /// <summary>
    /// Structure specifying parameters of a newly created pipeline depth stencil state.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe struct VkPipelineDepthStencilStateCreateInfo
    {
        /// <summary>
        /// A <see cref="VkStructureType"/> value identifying this structure.
        /// </summary>
        public readonly VkStructureType sType;

        /// <summary>
        /// Null or a pointer to a structure extending this structure.
        /// </summary>
        public void* pNext;

        /// <summary>
        /// A bitmask of <see cref="VkPipelineDepthStencilStateCreateFlagBits"/> specifying additional depth/stencil state information.
        /// </summary>
        public VkPipelineDepthStencilStateCreateFlags flags;

        /// <summary>
        /// Controls whether depth testing is enabled.
        /// </summary>
        public VkBool32 depthTestEnable;

        /// <summary>
        /// Controls whether depth writes are enabled when depthTestEnable is VK_TRUE. Depth writes are always disabled when depthTestEnable is VK_FALSE.
        /// </summary>
        public VkBool32 depthWriteEnable;

        /// <summary>
        /// A VkCompareOp value specifying the comparison operator to use in the Depth Comparison step of the depth test.
        /// </summary>
        public VkCompareOp depthCompareOp;

        /// <summary>
        /// controls whether depth bounds testing is enabled.
        /// </summary>
        public VkBool32 depthBoundsTestEnable;

        /// <summary>
        /// controls whether stencil testing is enabled.
        /// </summary>
        public VkBool32 stencilTestEnable;

        /// <summary>
        /// front and back are <see cref="VkStencilOpState"/> values controlling the corresponding parameters of the stencil test.
        /// </summary>
        public VkStencilOpState front;

        /// <summary>
        /// <inheritdoc cref="front" />
        /// </summary>
        public VkStencilOpState back;

        /// <summary>
        /// Is the minimum depth bound used in the depth bounds test.
        /// </summary>
        public float minDepthBounds;
        /// <summary>
        /// Is the maximum depth bound used in the depth bounds test.
        /// </summary>
        public float maxDepthBounds;

        public VkPipelineDepthStencilStateCreateInfo() =>
            this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO;
    }
}
