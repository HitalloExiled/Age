namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkStencilFaceFlags.html">VkStencilFaceFlags</see>
/// </summary>
[Flags]
public enum VkStencilFaceFlags
{
    Front        = 0x00000001,
    Back         = 0x00000002,
    FrontAndBack = Front | Back,
}
