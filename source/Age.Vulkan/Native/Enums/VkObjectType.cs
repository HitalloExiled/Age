using Age.Vulkan.Native.Types;

namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Specify an enumeration to track object handle types.
/// </summary>
public enum VkObjectType
{
    VK_OBJECT_TYPE_UNKNOWN = 0,

    /// <summary>
    /// See <see cref="VkInstance"/>
    /// </summary>
    VK_OBJECT_TYPE_INSTANCE = 1,

    /// <summary>
    /// See <see cref="VkPhysicalDevice"/>
    /// </summary>
    VK_OBJECT_TYPE_PHYSICAL_DEVICE = 2,

    /// <summary>
    /// See <see cref="VkDevice"/>
    /// </summary>
    VK_OBJECT_TYPE_DEVICE = 3,

    /// <summary>
    /// See <see cref="VkQueue"/>
    /// </summary>
    VK_OBJECT_TYPE_QUEUE = 4,

    /// <summary>
    /// See <see cref="VkSemaphore"/>
    /// </summary>
    VK_OBJECT_TYPE_SEMAPHORE = 5,

    /// <summary>
    /// See <see cref="VkCommandBuffer"/>
    /// </summary>
    VK_OBJECT_TYPE_COMMAND_BUFFER = 6,

    /// <summary>
    /// See <see cref="VkFence"/>
    /// </summary>
    VK_OBJECT_TYPE_FENCE = 7,

    /// <summary>
    /// See <see cref="VkDeviceMemory"/>
    /// </summary>
    VK_OBJECT_TYPE_DEVICE_MEMORY = 8,

    /// <summary>
    /// See <see cref="VkBuffer"/>
    /// </summary>
    VK_OBJECT_TYPE_BUFFER = 9,

    /// <summary>
    /// See <see cref="VkImage"/>
    /// </summary>
    VK_OBJECT_TYPE_IMAGE = 10,

    /// <summary>
    /// See <see cref="VkEvent"/>
    /// </summary>
    VK_OBJECT_TYPE_EVENT = 11,

    /// <summary>
    /// See <see cref="VkQueryPool"/>
    /// </summary>
    VK_OBJECT_TYPE_QUERY_POOL = 12,

    /// <summary>
    /// See <see cref="VkBufferView"/>
    /// </summary>
    VK_OBJECT_TYPE_BUFFER_VIEW = 13,

    /// <summary>
    /// See <see cref="VkImageView"/>
    /// </summary>
    VK_OBJECT_TYPE_IMAGE_VIEW = 14,

    /// <summary>
    /// See <see cref="VkShaderModule"/>
    /// </summary>
    VK_OBJECT_TYPE_SHADER_MODULE = 15,

    /// <summary>
    /// See <see cref="VkPipelineCache"/>
    /// </summary>
    VK_OBJECT_TYPE_PIPELINE_CACHE = 16,

    /// <summary>
    /// See <see cref="VkPipelineLayout"/>
    /// </summary>
    VK_OBJECT_TYPE_PIPELINE_LAYOUT = 17,

    /// <summary>
    /// See <see cref="VkRenderPass"/>
    /// </summary>
    VK_OBJECT_TYPE_RENDER_PASS = 18,

    /// <summary>
    /// See <see cref="VkPipeline"/>
    /// </summary>
    VK_OBJECT_TYPE_PIPELINE = 19,

    /// <summary>
    /// See <see cref="VkDescriptorSetLayout"/>
    /// </summary>
    VK_OBJECT_TYPE_DESCRIPTOR_SET_LAYOUT = 20,

    /// <summary>
    /// See <see cref="VkSampler"/>
    /// </summary>
    VK_OBJECT_TYPE_SAMPLER = 21,

    /// <summary>
    /// See <see cref="VkDescriptorPool"/>
    /// </summary>
    VK_OBJECT_TYPE_DESCRIPTOR_POOL = 22,

    /// <summary>
    /// See <see cref="VkDescriptorSet"/>
    /// </summary>
    VK_OBJECT_TYPE_DESCRIPTOR_SET = 23,

    /// <summary>
    /// See <see cref="VkFramebuffer"/>
    /// </summary>
    VK_OBJECT_TYPE_FRAMEBUFFER = 24,

    /// <summary>
    /// See <see cref="VkCommandPool"/>
    /// </summary>
    VK_OBJECT_TYPE_COMMAND_POOL = 25,

    /// <summary>
    /// See <see cref="VkSamplerYcbcrConversion"/>
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_OBJECT_TYPE_SAMPLER_YCBCR_CONVERSION = 1000156000,

    /// <summary>
    /// See <see cref="VkDescriptorUpdateTemplate"/>
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_OBJECT_TYPE_DESCRIPTOR_UPDATE_TEMPLATE = 1000085000,

    /// <summary>
    /// See <see cref="VkPrivateDataSlot"/>
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_OBJECT_TYPE_PRIVATE_DATA_SLOT = 1000295000,

    /// <summary>
    /// See <see cref="VkSurfaceKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_surface</remarks>
    VK_OBJECT_TYPE_SURFACE_KHR = 1000000000,

    /// <summary>
    /// See <see cref="VkSwapchainKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_OBJECT_TYPE_SWAPCHAIN_KHR = 1000001000,

    /// <summary>
    /// See <see cref="VkDisplayKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_display</remarks>
    VK_OBJECT_TYPE_DISPLAY_KHR = 1000002000,

    /// <summary>
    /// See <see cref="VkDisplayModeKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_display</remarks>
    VK_OBJECT_TYPE_DISPLAY_MODE_KHR = 1000002001,

    /// <summary>
    /// See <see cref="VkDebugReportCallbackExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_debug_report</remarks>
    VK_OBJECT_TYPE_DEBUG_REPORT_CALLBACK_EXT = 1000011000,

    /// <summary>
    /// See <see cref="VkVideoSessionKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_OBJECT_TYPE_VIDEO_SESSION_KHR = 1000023000,

    /// <summary>
    /// See <see cref="VkVideoSessionParametersKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_OBJECT_TYPE_VIDEO_SESSION_PARAMETERS_KHR = 1000023001,

    /// <summary>
    /// See <see cref="VkCuModuleNvx"/>
    /// </summary>
    /// <remarks>Provided by VK_NVX_binary_import</remarks>
    VK_OBJECT_TYPE_CU_MODULE_NVX = 1000029000,

    /// <summary>
    /// See <see cref="VkCuFunctionNvx"/>
    /// </summary>
    /// <remarks>Provided by VK_NVX_binary_import</remarks>
    VK_OBJECT_TYPE_CU_FUNCTION_NVX = 1000029001,

    /// <summary>
    /// See <see cref="VkDebugUtilsMessengerExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_OBJECT_TYPE_DEBUG_UTILS_MESSENGER_EXT = 1000128000,

    /// <summary>
    /// See <see cref="VkAccelerationStructureKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_OBJECT_TYPE_ACCELERATION_STRUCTURE_KHR = 1000150000,

    /// <summary>
    /// See <see cref="VkValidationCacheExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_validation_cache</remarks>
    VK_OBJECT_TYPE_VALIDATION_CACHE_EXT = 1000160000,

    /// <summary>
    /// See <see cref="VkAccelerationStructureNv"/>
    /// </summary>
    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_OBJECT_TYPE_ACCELERATION_STRUCTURE_NV = 1000165000,

    /// <summary>
    /// See <see cref="VkPerformanceConfigurationIntel"/>
    /// </summary>
    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_OBJECT_TYPE_PERFORMANCE_CONFIGURATION_INTEL = 1000210000,

    /// <summary>
    /// See <see cref="VkDeferredOperationKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_OBJECT_TYPE_DEFERRED_OPERATION_KHR = 1000268000,

    /// <summary>
    /// See <see cref="VkIndirectCommandsLayoutNv"/>
    /// </summary>
    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_OBJECT_TYPE_INDIRECT_COMMANDS_LAYOUT_NV = 1000277000,

    /// <summary>
    /// See <see cref="VkBufferCollectionFuchsia"/>
    /// </summary>
    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_OBJECT_TYPE_BUFFER_COLLECTION_FUCHSIA = 1000366000,

    /// <summary>
    /// See <see cref="VkMicromapExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_OBJECT_TYPE_MICROMAP_EXT = 1000396000,

    /// <summary>
    /// See <see cref="VkOpticalFlowSessionNv"/>
    /// </summary>
    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_OBJECT_TYPE_OPTICAL_FLOW_SESSION_NV = 1000464000,

    /// <summary>
    /// See <see cref="VkShaderExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_OBJECT_TYPE_SHADER_EXT = 1000482000,

    /// <summary>
    /// See <see cref="VkDescriptorUpdateTemplateKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_descriptor_update_template</remarks>
    VK_OBJECT_TYPE_DESCRIPTOR_UPDATE_TEMPLATE_KHR = VK_OBJECT_TYPE_DESCRIPTOR_UPDATE_TEMPLATE,

    /// <summary>
    /// See <see cref="VkSamplerYcbcrConversionKhr"/>
    /// </summary>
    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_OBJECT_TYPE_SAMPLER_YCBCR_CONVERSION_KHR = VK_OBJECT_TYPE_SAMPLER_YCBCR_CONVERSION,

    /// <summary>
    /// See <see cref="VkPrivateDataSlotExt"/>
    /// </summary>
    /// <remarks>Provided by VK_EXT_private_data</remarks>
    VK_OBJECT_TYPE_PRIVATE_DATA_SLOT_EXT = VK_OBJECT_TYPE_PRIVATE_DATA_SLOT,
}
