namespace Age.Vulkan;

public enum VkResult
{
    VK_SUCCESS                                            = 0,
    VK_NOT_READY                                          = 1,
    VK_TIMEOUT                                            = 2,
    VK_EVENT_SET                                          = 3,
    VK_EVENT_RESET                                        = 4,
    VK_INCOMPLETE                                         = 5,
    VK_ERROR_OUT_OF_HOST_MEMORY                           = -1,
    VK_ERROR_OUT_OF_DEVICE_MEMORY                         = -2,
    VK_ERROR_INITIALIZATION_FAILED                        = -3,
    VK_ERROR_DEVICE_LOST                                  = -4,
    VK_ERROR_MEMORY_MAP_FAILED                            = -5,
    VK_ERROR_LAYER_NOT_PRESENT                            = -6,
    VK_ERROR_EXTENSION_NOT_PRESENT                        = -7,
    VK_ERROR_FEATURE_NOT_PRESENT                          = -8,
    VK_ERROR_INCOMPATIBLE_DRIVER                          = -9,
    VK_ERROR_TOO_MANY_OBJECTS                             = -10,
    VK_ERROR_FORMAT_NOT_SUPPORTED                         = -11,
    VK_ERROR_FRAGMENTED_POOL                              = -12,
    VK_ERROR_UNKNOWN                                      = -13,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_ERROR_OUT_OF_POOL_MEMORY                           = -1000069000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_ERROR_INVALID_EXTERNAL_HANDLE                      = -1000072003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_ERROR_FRAGMENTATION                                = -1000161000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS               = -1000257000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_COMPILE_REQUIRED                          = 1000297000,

    /// <remarks>Provided by VK_KHR_surface</remarks>
    VK_ERROR_SURFACE_LOST_KHR                             = -1000000000,

    /// <remarks>Provided by VK_KHR_surface</remarks>
    VK_ERROR_NATIVE_WINDOW_IN_USE_KHR                     = -1000000001,

    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_SUBOPTIMAL_KHR                                     = 1000001003,

    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_ERROR_OUT_OF_DATE_KHR                              = -1000001004,

    /// <remarks>Provided by VK_KHR_display_swapchain</remarks>
    VK_ERROR_INCOMPATIBLE_DISPLAY_KHR                     = -1000003001,

    /// <remarks>Provided by VK_EXT_debug_report</remarks>
    VK_ERROR_VALIDATION_FAILED_EXT                        = -1000011001,

    /// <remarks>Provided by VK_NV_glsl_shader</remarks>
    VK_ERROR_INVALID_SHADER_NV                            = -1000012000,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_IMAGE_USAGE_NOT_SUPPORTED_KHR                = -1000023000,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PICTURE_LAYOUT_NOT_SUPPORTED_KHR       = -1000023001,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_OPERATION_NOT_SUPPORTED_KHR    = -1000023002,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_FORMAT_NOT_SUPPORTED_KHR       = -1000023003,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_CODEC_NOT_SUPPORTED_KHR        = -1000023004,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_STD_VERSION_NOT_SUPPORTED_KHR          = -1000023005,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_ERROR_INVALID_DRM_FORMAT_MODIFIER_PLANE_LAYOUT_EXT = -1000158000,

    /// <remarks>Provided by VK_KHR_global_priority</remarks>
    VK_ERROR_NOT_PERMITTED_KHR                            = -1000174001,

    /// <remarks>Provided by VK_EXT_full_screen_exclusive</remarks>
    VK_ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT          = -1000255000,

    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_THREAD_IDLE_KHR                                    = 1000268000,

    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_THREAD_DONE_KHR                                    = 1000268001,


    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_OPERATION_DEFERRED_KHR                             = 1000268002,

    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_OPERATION_NOT_DEFERRED_KHR                         = 1000268003,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR             = -1000299000,
#endif

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_ERROR_COMPRESSION_EXHAUSTED_EXT                    = -1000338000,

    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_ERROR_INCOMPATIBLE_SHADER_BINARY_EXT               = 1000482000,

    /// <remarks>Provided by VK_KHR_maintenance1</remarks>
    VK_ERROR_OUT_OF_POOL_MEMORY_KHR                       = VK_ERROR_OUT_OF_POOL_MEMORY,

    /// <remarks>Provided by VK_KHR_external_memory</remarks>
    VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR                  = VK_ERROR_INVALID_EXTERNAL_HANDLE,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_ERROR_FRAGMENTATION_EXT                            = VK_ERROR_FRAGMENTATION,

    /// <remarks>Provided by VK_EXT_global_priority</remarks>
    VK_ERROR_NOT_PERMITTED_EXT                            = VK_ERROR_NOT_PERMITTED_KHR,

    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_ERROR_INVALID_DEVICE_ADDRESS_EXT                   = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS_KHR           = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_PIPELINE_COMPILE_REQUIRED_EXT                      = VK_PIPELINE_COMPILE_REQUIRED,


    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_ERROR_PIPELINE_COMPILE_REQUIRED_EXT                = VK_PIPELINE_COMPILE_REQUIRED,
}
