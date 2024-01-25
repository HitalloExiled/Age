namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkShaderStageFlagBits.html">VkShaderStageFlagBits</see>
/// </summary>
[Flags]
public enum ShaderStageFlags
{
    Vertex                 = 0x00000001,
    TessellationControl    = 0x00000002,
    TessellationEvaluation = 0x00000004,
    Geometry               = 0x00000008,
    Fragment               = 0x00000010,
    Compute                = 0x00000020,
    AllGraphics            = 0x0000001F,
    All                    = 0x7FFFFFFF,
    RaygenKhr              = 0x00000100,
    AnyHitKhr              = 0x00000200,
    ClosestHitKhr          = 0x00000400,
    MissKhr                = 0x00000800,
    IntersectionKhr        = 0x00001000,
    CallableKhr            = 0x00002000,
    TaskExt                = 0x00000040,
    MeshExt                = 0x00000080,
    SubpassShadingHuawei   = 0x00004000,
    ClusterCullingHuawei   = 0x00080000,
    RaygenNv               = RaygenKhr,
    AnyHitNv               = AnyHitKhr,
    ClosestHitNv           = ClosestHitKhr,
    MissNv                 = MissKhr,
    IntersectionNv         = IntersectionKhr,
    CallableNv             = CallableKhr,
    TaskNv                 = TaskExt,
    MeshNv                 = MeshExt,
}
