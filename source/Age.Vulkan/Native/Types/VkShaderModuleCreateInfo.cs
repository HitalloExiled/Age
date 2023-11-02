using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Flags;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// Structure specifying parameters of a newly created shader module.
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public unsafe struct VkShaderModuleCreateInfo
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
    public VkShaderModuleCreateFlags flags;

    /// <summary>
    /// The size, in bytes, of the code pointed to by pCode.
    /// </summary>
    public ulong codeSize;

    /// <summary>
    /// A pointer to code that is used to create the shader module. The type and format of the code is determined from the content of the memory addressed by pCode.
    /// </summary>
    public uint* pCode;

    public VkShaderModuleCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
}
