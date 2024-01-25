namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineBindPoint.html">VkPipelineBindPoint</see>
/// </summary>
public enum PipelineBindPoint
{
    Graphics             = 0,
    Compute              = 1,

#if VK_ENABLE_BETA_EXTENSIONS
    ExecutionGraphAmdx   = 1000134000,
#endif

    RayTracingKhr        = 1000165000,
    SubpassShadingHuawei = 1000369003,
    RayTracingNv         = RayTracingKhr,
}
