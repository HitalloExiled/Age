namespace Age.Vulkan.Enums;

/// <summary>
/// Specify the bind point of a pipeline object to a command buffer.
/// </summary>
public enum VkPipelineBindPoint
{
    /// <summary>
    /// Specifies binding as a compute pipeline.
    /// </summary>
    VK_PIPELINE_BIND_POINT_GRAPHICS = 0,

    /// <summary>
    /// Specifies binding as a graphics pipeline.
    /// </summary>
    VK_PIPELINE_BIND_POINT_COMPUTE = 1,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies binding as a ray tracing pipeline.
    /// </summary>
    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_PIPELINE_BIND_POINT_EXECUTION_GRAPH_AMDX = 1000134000,
#endif

    /// <summary>
    /// Specifies binding as a subpass shading pipeline.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_BIND_POINT_RAY_TRACING_KHR = 1000165000,

    /// <summary>
    /// Specifies binding as an <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#executiongraphs">execution graph pipeline</see>.
    /// </summary>
    /// <remarks>Provided by VK_HUAWEI_subpass_shading</remarks>
    VK_PIPELINE_BIND_POINT_SUBPASS_SHADING_HUAWEI = 1000369003,

    /// <inheritdoc cref="VK_PIPELINE_BIND_POINT_RAY_TRACING_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_PIPELINE_BIND_POINT_RAY_TRACING_NV = VK_PIPELINE_BIND_POINT_RAY_TRACING_KHR,
}
