namespace Age.Vulkan.Native;

/// <summary>
/// <para>Applications may change the name associated with an object simply by calling <see cref="VK.SetDebugUtilsObjectNameEXT"/> again with a new string. If <see cref="pObjectName"/> is either NULL or an empty string, then any previously set name is removed.</para>
/// <para>The graphicsPipelineLibrary feature allows the specification of pipelines without the creation of <see cref="VkShaderModule"/> objects beforehand. In order to continue to allow naming these shaders independently, <see cref="VkDebugUtilsObjectNameInfoEXT"/> can be included in the pNext chain of <see cref="VkPipelineShaderStageCreateInfo"/>, which associates a static name with that particular shader.</para>
/// </summary>
public unsafe struct VkDebugUtilsObjectNameInfoEXT
{
    /// <summary>
    /// A VkStructureType value identifying this structure.
    /// </summary>
    public VkStructureType sType;

    /// <summary>
    /// NULL or a pointer to a structure extending this structure.
    /// </summary>
    public void* pNext;

    /// <summary>
    /// A VkObjectType specifying the type of the object to be named.
    /// </summary>
    public VkObjectType objectType;

    /// <summary>
    /// The object to be named.
    /// </summary>
    public long objectHandle;

    /// <summary>
    /// Either NULL or a null-terminated UTF-8 string specifying the name to apply to objectHandle.
    /// </summary>
    public byte* pObjectName;
}
