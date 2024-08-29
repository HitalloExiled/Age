namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkShaderStageFlagBits.html">VkShaderStageFlagBits</see>
/// </summary>
[Flags]
public enum VkShaderStageFlags
{
    Vertex                 = 0x00000001,
    TessellationControl    = 0x00000002,
    TessellationEvaluation = 0x00000004,
    Geometry               = 0x00000008,
    Fragment               = 0x00000010,
    Compute                = 0x00000020,
    AllGraphics            = 0x0000001F,
    All                    = 0x7FFFFFFF,
    RaygenKHR              = 0x00000100,
    AnyHitKHR              = 0x00000200,
    ClosestHitKHR          = 0x00000400,
    MissKHR                = 0x00000800,
    IntersectionKHR        = 0x00001000,
    CallableKHR            = 0x00002000,
    TaskEXT                = 0x00000040,
    MeshEXT                = 0x00000080,
    SubpassShadingHUAWEI   = 0x00004000,
    ClusterCullingHUAWEI   = 0x00080000,
    RaygenNV               = RaygenKHR,
    AnyHitNV               = AnyHitKHR,
    ClosestHitNV           = ClosestHitKHR,
    MissNV                 = MissKHR,
    IntersectionNV         = IntersectionKHR,
    CallableNV             = CallableKHR,
    TaskNV                 = TaskEXT,
    MeshNV                 = MeshEXT,
}
