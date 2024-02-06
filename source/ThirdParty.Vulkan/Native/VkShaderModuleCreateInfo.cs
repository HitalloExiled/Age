namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkShaderModuleCreateInfo.html">VkShaderModuleCreateInfo</see>
/// </summary>
public unsafe struct VkShaderModuleCreateInfo
{
    public readonly VkStructureType SType;

    public void*                     PNext;
    public VkShaderModuleCreateFlags Flags;
    public ulong                     CodeSize;
    public uint*                     PCode;

    public VkShaderModuleCreateInfo() =>
        this.SType = VkStructureType.ShaderModuleCreateInfo;
}
