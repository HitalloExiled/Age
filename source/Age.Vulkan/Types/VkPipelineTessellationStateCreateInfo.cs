using Age.Vulkan.Enums;
using Age.Vulkan.Flags;

namespace Age.Vulkan.Types
{
    /// <summary>
    /// Structure specifying parameters of a newly created pipeline tessellation state.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    public unsafe struct VkPipelineTessellationStateCreateInfo
    {
        /// <summary>
        /// A VkStructureType value identifying this structure.
        /// </summary>
        public readonly VkStructureType sType;

        /// <summary>
        /// Null or a pointer to a structure extending this structure.
        /// </summary>
        public void* pNext;

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public VkPipelineTessellationStateCreateFlags flags;

        /// <summary>
        /// The number of control points per patch.
        /// </summary>
        public uint patchControlPoints;

        public VkPipelineTessellationStateCreateInfo() =>
            this.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_STATE_CREATE_INFO;
    }
}
