namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkShaderModuleCreateInfo.html">VkShaderModuleCreateInfo</see>
/// </summary>
public unsafe struct VkShaderModuleCreateInfo
{
    public readonly VkStructureType sType;

    public void*                     pNext;
    public VkShaderModuleCreateFlags flags;
    public ulong                     codeSize;
    public uint*                     pCode;

    public VkShaderModuleCreateInfo() =>
        this.sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO;
}
