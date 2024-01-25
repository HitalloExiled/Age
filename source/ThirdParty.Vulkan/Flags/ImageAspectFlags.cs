namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageAspectFlagBits.html">VkImageAspectFlagBits</see>
/// </summary>
[Flags]
public enum ImageAspectFlags
{
    Color           = 0x00000001,
    Depth           = 0x00000002,
    Stencil         = 0x00000004,
    Metadata        = 0x00000008,
    Plane0          = 0x00000010,
    Plane1          = 0x00000020,
    Plane2          = 0x00000040,
    None            = 0,
    MemoryPlane0Ext = 0x00000080,
    MemoryPlane1Ext = 0x00000100,
    MemoryPlane2Ext = 0x00000200,
    MemoryPlane3Ext = 0x00000400,
    Plane0Khr       = Plane0,
    Plane1Khr       = Plane1,
    Plane2Khr       = Plane2,
    NoneKhr         = None,
}
