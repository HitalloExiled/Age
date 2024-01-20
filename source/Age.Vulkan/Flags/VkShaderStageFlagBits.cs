namespace Age.Vulkan.Flags;

/// <summary>
/// <para>Bitmask specifying a pipeline stage.</para>
/// <remarks><see cref="VK_SHADER_STAGE_ALL_GRAPHICS"/> only includes the original five graphics stages included in Vulkan 1.0, and not any stages added by extensions. Thus, it may not have the desired effect in all cases.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkShaderStageFlagBits
{
    /// <summary>
    /// VK_SHADER_STAGE_VERTEX_BIT specifies the vertex stage.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_0</remarks>
    VK_SHADER_STAGE_VERTEX_BIT = 0x00000001,

    /// <summary>
    /// VK_SHADER_STAGE_TESSELLATION_CONTROL_BIT specifies the tessellation control stage.
    /// </summary>
    VK_SHADER_STAGE_TESSELLATION_CONTROL_BIT = 0x00000002,

    /// <summary>
    /// VK_SHADER_STAGE_TESSELLATION_EVALUATION_BIT specifies the tessellation evaluation stage.
    /// </summary>
    VK_SHADER_STAGE_TESSELLATION_EVALUATION_BIT = 0x00000004,

    /// <summary>
    /// VK_SHADER_STAGE_GEOMETRY_BIT specifies the geometry stage.
    /// </summary>
    VK_SHADER_STAGE_GEOMETRY_BIT = 0x00000008,

    /// <summary>
    /// VK_SHADER_STAGE_FRAGMENT_BIT specifies the fragment stage.
    /// </summary>
    VK_SHADER_STAGE_FRAGMENT_BIT = 0x00000010,

    /// <summary>
    /// VK_SHADER_STAGE_COMPUTE_BIT specifies the compute stage.
    /// </summary>
    VK_SHADER_STAGE_COMPUTE_BIT = 0x00000020,

    /// <summary>
    /// VK_SHADER_STAGE_ALL_GRAPHICS is a combination of bits used as shorthand to specify all graphics stages defined above (excluding the compute stage).
    /// </summary>
    VK_SHADER_STAGE_ALL_GRAPHICS = 0x0000001F,

    /// <summary>
    /// VK_SHADER_STAGE_ALL is a combination of bits used as shorthand to specify all shader stages supported by the device, including all additional stages which are introduced by extensions.
    /// </summary>
    VK_SHADER_STAGE_ALL = 0x7FFFFFFF,

    /// <summary>
    /// VK_SHADER_STAGE_RAYGEN_BIT_KHR specifies the ray generation stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_RAYGEN_BIT_KHR = 0x00000100,

    /// <summary>
    /// VK_SHADER_STAGE_ANY_HIT_BIT_KHR specifies the any-hit stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_ANY_HIT_BIT_KHR = 0x00000200,

    /// <summary>
    /// VK_SHADER_STAGE_CLOSEST_HIT_BIT_KHR specifies the closest hit stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_CLOSEST_HIT_BIT_KHR = 0x00000400,

    /// <summary>
    /// VK_SHADER_STAGE_MISS_BIT_KHR specifies the miss stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_MISS_BIT_KHR = 0x00000800,

    /// <summary>
    /// VK_SHADER_STAGE_INTERSECTION_BIT_KHR specifies the intersection stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_INTERSECTION_BIT_KHR = 0x00001000,

    /// <summary>
    /// VK_SHADER_STAGE_CALLABLE_BIT_KHR specifies the callable stage.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_SHADER_STAGE_CALLABLE_BIT_KHR = 0x00002000,

    /// <summary>
    /// VK_SHADER_STAGE_TASK_BIT_EXT specifies the task stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_SHADER_STAGE_TASK_BIT_EXT = 0x00000040,

    /// <summary>
    /// VK_SHADER_STAGE_MESH_BIT_EXT specifies the mesh stage.
    /// </summary>
    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_SHADER_STAGE_MESH_BIT_EXT = 0x00000080,

    /// <remarks>Provided by VK_HUAWEI_subpass_shading</remarks>
    VK_SHADER_STAGE_SUBPASS_SHADING_BIT_HUAWEI = 0x00004000,

    /// <summary>
    /// VK_SHADER_STAGE_CLUSTER_CULLING_BIT_HUAWEI specifies the cluster culling stage.
    /// </summary>
    /// <remarks>Provided by VK_HUAWEI_cluster_culling_shader</remarks>
    VK_SHADER_STAGE_CLUSTER_CULLING_BIT_HUAWEI = 0x00080000,

    /// <inheritdoc cref="VK_SHADER_STAGE_RAYGEN_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_RAYGEN_BIT_NV = VK_SHADER_STAGE_RAYGEN_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_ANY_HIT_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_ANY_HIT_BIT_NV = VK_SHADER_STAGE_ANY_HIT_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_CLOSEST_HIT_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_CLOSEST_HIT_BIT_NV = VK_SHADER_STAGE_CLOSEST_HIT_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_MISS_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_MISS_BIT_NV = VK_SHADER_STAGE_MISS_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_INTERSECTION_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_INTERSECTION_BIT_NV = VK_SHADER_STAGE_INTERSECTION_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_CALLABLE_BIT_KHR" />
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_SHADER_STAGE_CALLABLE_BIT_NV = VK_SHADER_STAGE_CALLABLE_BIT_KHR,

    /// <inheritdoc cref="VK_SHADER_STAGE_TASK_BIT_EXT" />
    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_SHADER_STAGE_TASK_BIT_NV = VK_SHADER_STAGE_TASK_BIT_EXT,

    /// <inheritdoc cref="VK_SHADER_STAGE_MESH_BIT_EXT" />
    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_SHADER_STAGE_MESH_BIT_NV = VK_SHADER_STAGE_MESH_BIT_EXT,

}
