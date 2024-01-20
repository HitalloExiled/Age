using Age.Vulkan.Enums;
using Age.Vulkan.Types;

namespace Age.Vulkan.Flags;

/// <summary>
/// <para>Bitmask controlling how a pipeline is created.</para>
/// <para>It is valid to set both <see cref="VK_PIPELINE_CREATE_ALLOW_DERIVATIVES_BIT"/> and <see cref="VK_PIPELINE_CREATE_DERIVATIVE_BIT"/>. This allows a pipeline to be both a parent and possibly a child in a pipeline hierarchy. See Pipeline Derivatives for more information.</para>
/// <para>When an implementation is looking up a pipeline in a pipeline cache, if that pipeline is being created using linked libraries, implementations should always return an equivalent pipeline created with <see cref="VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/> if available, whether or not that bit was specified.</para>
/// <remarks>Note: Using <see cref="VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/> (or not) when linking pipeline libraries is intended as a performance tradeoff between host and device. If the bit is omitted, linking should be faster and produce a pipeline more rapidly, but performance of the pipeline on the target device may be reduced. If the bit is included, linking may be slower but should produce a pipeline with device performance comparable to a monolithically created pipeline. Using both options can allow latency-sensitive applications to generate a suboptimal but usable pipeline quickly, and then perform an optimal link in the background, substituting the result for the suboptimally linked pipeline as soon as it is available.</remarks>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[Flags]
public enum VkPipelineCreateFlagBits
{
    /// <summary>
    /// Specifies that the created pipeline will not be optimized. Using this flag may reduce the time taken to create the pipeline.
    /// </summary>
    VK_PIPELINE_CREATE_DISABLE_OPTIMIZATION_BIT = 0x00000001,

    /// <summary>
    /// Specifies that the pipeline to be created is allowed to be the parent of a pipeline that will be created in a subsequent pipeline creation call.
    /// </summary>
    VK_PIPELINE_CREATE_ALLOW_DERIVATIVES_BIT = 0x00000002,

    /// <summary>
    /// Specifies that the pipeline to be created will be a child of a previously created parent pipeline.
    /// </summary>
    VK_PIPELINE_CREATE_DERIVATIVE_BIT = 0x00000004,

    /// <summary>
    /// Specifies that any shader input variables decorated as ViewIndex will be assigned values as if they were decorated as DeviceIndex.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_PIPELINE_CREATE_VIEW_INDEX_FROM_DEVICE_INDEX_BIT = 0x00000008,

    /// <summary>
    /// Specifies that a compute pipeline can be used with <see cref="Vk.CmdDispatchBase"/> with a non-zero base workgroup.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_PIPELINE_CREATE_DISPATCH_BASE_BIT = 0x00000010,


    /// <summary>
    /// Specifies that pipeline creation will fail if a compile is required for creation of a valid <see cref="VkPipeline"/> object; <see cref="VkResult.VK_PIPELINE_COMPILE_REQUIRED"/> will be returned by pipeline creation, and the <see cref="VkPipeline"/> will be set to default.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT = 0x00000100,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_CREATE_EARLY_RETURN_ON_FAILURE_BIT = 0x00000200,

    /// <summary>
    /// Specifies that the pipeline will be used with a fragment shading rate attachment and dynamic rendering.
    /// </summary>
    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_KHR_fragment_shading_rate</remarks>
    VK_PIPELINE_CREATE_RENDERING_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = 0x00200000,

    /// <summary>
    /// Specifies that the pipeline will be used with a fragment density map attachment and dynamic rendering.
    /// </summary>
    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_EXT_fragment_density_map</remarks>
    VK_PIPELINE_CREATE_RENDERING_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT = 0x00400000,

    /// <summary>
    /// Specifies that an any-hit shader will always be present when an any-hit shader would be executed. A NULL any-hit shader is an any-hit shader which is effectively <see cref="Vk.VK_SHADER_UNUSED_KHR"/>, such as from a shader group consisting entirely of zeros.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_NO_NULL_ANY_HIT_SHADERS_BIT_KHR = 0x00004000,

    /// <summary>
    /// Specifies that a closest hit shader will always be present when a closest hit shader would be executed. A NULL closest hit shader is a closest hit shader which is effectively <see cref="Vk.VK_SHADER_UNUSED_KHR"/>, such as from a shader group consisting entirely of zeros.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_NO_NULL_CLOSEST_HIT_SHADERS_BIT_KHR = 0x00008000,

    /// <summary>
    /// Specifies that a miss shader will always be present when a miss shader would be executed. A NULL miss shader is a miss shader which is effectively <see cref="Vk.VK_SHADER_UNUSED_KHR"/>, such as from a shader group consisting entirely of zeros.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_NO_NULL_MISS_SHADERS_BIT_KHR = 0x00010000,

    /// <summary>
    /// Specifies that an intersection shader will always be present when an intersection shader would be executed. A NULL intersection shader is an intersection shader which is effectively <see cref="Vk.VK_SHADER_UNUSED_KHR"/>, such as from a shader group consisting entirely of zeros.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_NO_NULL_INTERSECTION_SHADERS_BIT_KHR = 0x00020000,

    /// <summary>
    /// Specifies that triangle primitives will be skipped during traversal using OpTraceRayKHR.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_SKIP_TRIANGLES_BIT_KHR = 0x00001000,

    /// <summary>
    /// Specifies that AABB primitives will be skipped during traversal using OpTraceRayKHR.
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_SKIP_AABBS_BIT_KHR = 0x00002000,

    /// <summary>
    /// Specifies that the shader group handles can be saved and reused on a subsequent run (e.g. for trace capture and replay).
    /// </summary>
    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_SHADER_GROUP_HANDLE_CAPTURE_REPLAY_BIT_KHR = 0x00080000,

    /// <summary>
    /// Specifies that a pipeline is created with all shaders in the deferred state. Before using the pipeline the application must call <see cref="VkNvRayTracing.CompileDeferred"/> exactly once on each shader in the pipeline before using the pipeline.
    /// </summary>
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_PIPELINE_CREATE_DEFER_COMPILE_BIT_NV = 0x00000020,

    /// <summary>
    /// Specifies that the shader compiler should capture statistics for the pipeline executables produced by the compile process which can later be retrieved by calling <see cref="VkKhrPipelineExecutableProperties.GetPipelineExecutableStatistics"/>. Enabling this flag must not affect the final compiled pipeline but may disable pipeline caching or otherwise affect pipeline creation time.
    /// </summary>
    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_PIPELINE_CREATE_CAPTURE_STATISTICS_BIT_KHR = 0x00000040,

    /// <summary>
    /// Specifies that the shader compiler should capture the internal representations of pipeline executables produced by the compile process which can later be retrieved by calling <see cref="VkKhrPipelineExecutableProperties.GetPipelineExecutableInternalRepresentations"/>. Enabling this flag must not affect the final compiled pipeline but may disable pipeline caching or otherwise affect pipeline creation time. When capturing IR from pipelines created with pipeline libraries, there is no guarantee that IR from libraries can be retrieved from the linked pipeline. Applications should retrieve IR from each library, and any linked pipelines, separately.
    /// </summary>
    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_PIPELINE_CREATE_CAPTURE_INTERNAL_REPRESENTATIONS_BIT_KHR = 0x00000080,

    /// <summary>
    /// <para>Specifies that the pipeline can be used in combination with https://registry.khronos.org/vulkan/specs/1.3-extensions/html/vkspec.html#device-generated-commands.</para>
    /// <para>When creating multiple pipelines, <see cref="VK_PIPELINE_CREATE_EARLY_RETURN_ON_FAILURE_BIT"/> specifies that control will be returned to the application if any individual pipeline returns a result which is not <see cref="VkResult.VK_SUCCESS"/> rather than continuing to create additional pipelines.</para>
    /// </summary>
    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_PIPELINE_CREATE_INDIRECT_BINDABLE_BIT_NV = 0x00040000,

    /// <summary>
    /// Specifies that the pipeline cannot be used directly, and instead defines a pipeline library that can be combined with other pipelines using the <see cref="VkPipelineLibraryCreateInfoKHR"/> structure. This is available in ray tracing and graphics pipelines.
    /// </summary>
    /// <remarks>Provided by VK_KHR_pipeline_library</remarks>
    VK_PIPELINE_CREATE_LIBRARY_BIT_KHR = 0x00000800,

    /// <summary>
    /// Specifies that a pipeline will be used with descriptor buffers, rather than descriptor sets.
    /// </summary>
    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_PIPELINE_CREATE_DESCRIPTOR_BUFFER_BIT_EXT = 0x20000000,

    /// <summary>
    /// Specifies that pipeline libraries should retain any information necessary to later perform an optimal link with <see cref="VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT"/>.
    /// </summary>
    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_PIPELINE_CREATE_RETAIN_LINK_TIME_OPTIMIZATION_INFO_BIT_EXT = 0x00800000,

    /// <summary>
    /// Specifies that pipeline libraries being linked into this library should have link time optimizations applied. If this bit is omitted, implementations should instead perform linking as rapidly as possible.
    /// </summary>
    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_PIPELINE_CREATE_LINK_TIME_OPTIMIZATION_BIT_EXT = 0x00000400,

    /// <summary>
    /// Specifies that the pipeline is allowed to use OpTraceRayMotionNV.
    /// </summary>
    /// <remarks>Provided by VK_NV_ray_tracing_motion_blur</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_ALLOW_MOTION_BIT_NV = 0x00100000,

    /// <summary>
    /// Specifies that the pipeline may be used with an attachment feedback loop including color attachments. It is ignored if <see cref="VkDinamicState.VK_DYNAMIC_STATE_ATTACHMENT_FEEDBACK_LOOP_ENABLE_EXT"/> is set in pDynamicStates.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_PIPELINE_CREATE_COLOR_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT = 0x02000000,

    /// <summary>
    /// Specifies that the pipeline may be used with an attachment feedback loop including depth-stencil attachments. It is ignored if <see cref="VkDinamicState.VK_DYNAMIC_STATE_ATTACHMENT_FEEDBACK_LOOP_ENABLE_EXT"/> is set in pDynamicStates.
    /// </summary>
    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_PIPELINE_CREATE_DEPTH_STENCIL_ATTACHMENT_FEEDBACK_LOOP_BIT_EXT = 0x04000000,

    /// <summary>
    /// Specifies that the pipeline can be used with acceleration structures which reference an opacity micromap array.
    /// </summary>
    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_OPACITY_MICROMAP_BIT_EXT = 0x01000000,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <summary>
    /// Specifies that the pipeline can be used with aceleration structures which reference a displacement micromap array.
    /// </summary>
    /// <remarks>Provided by VK_NV_displacement_micromap</remarks>
    VK_PIPELINE_CREATE_RAY_TRACING_DISPLACEMENT_MICROMAP_BIT_NV = 0x10000000,
#endif

    /// <summary>
    /// Specifies that the pipeline must not be bound to a protected command buffer.
    /// </summary>
    /// <remarks>Provided by VK_EXT_pipeline_protected_access</remarks>
    VK_PIPELINE_CREATE_NO_PROTECTED_ACCESS_BIT_EXT = 0x08000000,

    /// <summary>
    /// Specifies that the pipeline must not be bound to an unprotected command buffer.
    /// </summary>
    /// <remarks>Provided by VK_EXT_pipeline_protected_access</remarks>
    VK_PIPELINE_CREATE_PROTECTED_ACCESS_ONLY_BIT_EXT = 0x40000000,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_DISPATCH_BASE_BIT" />
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_PIPELINE_CREATE_DISPATCH_BASE = VK_PIPELINE_CREATE_DISPATCH_BASE_BIT,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_RENDERING_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR" />
    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_KHR_fragment_shading_rate</remarks>
    VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR = VK_PIPELINE_CREATE_RENDERING_FRAGMENT_SHADING_RATE_ATTACHMENT_BIT_KHR,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_RENDERING_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT" />
    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_EXT_fragment_density_map</remarks>
    VK_PIPELINE_RASTERIZATION_STATE_CREATE_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT = VK_PIPELINE_CREATE_RENDERING_FRAGMENT_DENSITY_MAP_ATTACHMENT_BIT_EXT,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_VIEW_INDEX_FROM_DEVICE_INDEX_BIT" />
    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_PIPELINE_CREATE_VIEW_INDEX_FROM_DEVICE_INDEX_BIT_KHR = VK_PIPELINE_CREATE_VIEW_INDEX_FROM_DEVICE_INDEX_BIT,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_DISPATCH_BASE" />
    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_PIPELINE_CREATE_DISPATCH_BASE_KHR = VK_PIPELINE_CREATE_DISPATCH_BASE,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT" />
    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT_EXT = VK_PIPELINE_CREATE_FAIL_ON_PIPELINE_COMPILE_REQUIRED_BIT,

    /// <inheritdoc cref="VK_PIPELINE_CREATE_EARLY_RETURN_ON_FAILURE_BIT" />
    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_PIPELINE_CREATE_EARLY_RETURN_ON_FAILURE_BIT_EXT = VK_PIPELINE_CREATE_EARLY_RETURN_ON_FAILURE_BIT,
}
