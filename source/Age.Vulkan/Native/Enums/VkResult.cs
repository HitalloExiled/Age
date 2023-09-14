namespace Age.Vulkan.Native.Enums;

/// <summary>
/// Vulkan command return codes.</para>
/// <para>While the core Vulkan API is not designed to capture incorrect usage, some circumstances still require return codes. Commands in Vulkan return their status via return codes that are in one of two categories:</para>
/// <list type="bullet">
/// <item>Successful completion codes are returned when a command needs to communicate success or status information. All successful completion codes are non-negative values.</item>
/// <item>Run time error codes are returned when a command needs to communicate a failure that could only be detected at runtime. All runtime error codes are negative values.</item>
/// </list>
/// <para>If a command returns a runtime error, unless otherwise specified any output parameters will have undefined contents, except that if the output parameter is a structure with sType and pNext fields, those fields will be unmodified. Any structures chained from pNext will also have undefined contents, except that sType and pNext will be unmodified.</para>
/// <para>VK_ERROR_OUT_OF_*_MEMORY errors do not modify any currently existing Vulkan objects. Objects that have already been successfully created can still be used by the application.</para>
/// <remarks>As a general rule, Free, Release, and Reset commands do not return <see cref="VK_ERROR_OUT_OF_HOST_MEMORY"/>, while any other command with a return code may return it. Any exceptions from this rule are described for those commands.</remarks>
/// <para><see cref="VK_ERROR_UNKNOWN"/> will be returned by an implementation when an unexpected error occurs that cannot be attributed to valid behavior of the application and implementation. Under these conditions, it may be returned from any command returning a VkResult.</para>
/// <remarks><see cref="VK_ERROR_UNKNOWN"/> is not expected to ever be returned if the application behavior is valid, and if the implementation is bug-free. If <see cref="VK_ERROR_UNKNOWN"/> is received, the application should be checked against the latest validation layers to verify correct behavior as much as possible. If no issues are identified it could be an implementation issue, and the implementor should be contacted for support.</remarks>
/// <para>Any command returning a VkResult may return <see cref="VK_ERROR_VALIDATION_FAILED_EXT"/> if a violation of valid usage is detected, even though commands do not explicitly list this as a possible return code.</para>
/// <para>Performance-critical commands generally do not have return codes. If a runtime error occurs in such commands, the implementation will defer reporting the error until a specified point. For commands that record into command buffers (vkCmd*) runtime errors are reported by vkEndCommandBuffer.</para>
/// </summary>
public enum VkResult
{
    /// <summary>
    /// Command successfully completed.
    /// </summary>
    VK_SUCCESS = 0,

    /// <summary>
    /// A fence or query has not yet completed.
    /// </summary>
    VK_NOT_READY = 1,

    /// <summary>
    /// A wait operation has not completed in the specified time.
    /// </summary>
    VK_TIMEOUT = 2,

    /// <summary>
    /// An event is signaled.
    /// </summary>
    VK_EVENT_SET = 3,

    /// <summary>
    /// An event is unsignaled.
    /// </summary>
    VK_EVENT_RESET = 4,

    /// <summary>
    /// A return array was too small for the result.
    /// </summary>
    VK_INCOMPLETE = 5,

    /// <summary>
    /// A host memory allocation has failed.
    /// </summary>
    VK_ERROR_OUT_OF_HOST_MEMORY = -1,

    /// <summary>
    /// A device memory allocation has failed.
    /// </summary>
    VK_ERROR_OUT_OF_DEVICE_MEMORY = -2,

    /// <summary>
    /// Initialization of an object could not be completed for implementation-specific reasons.
    /// </summary>
    VK_ERROR_INITIALIZATION_FAILED = -3,

    /// <summary>
    /// The logical or physical device has been lost. See Lost Device.
    /// </summary>
    VK_ERROR_DEVICE_LOST = -4,

    /// <summary>
    /// Mapping of a memory object has failed.
    /// </summary>
    VK_ERROR_MEMORY_MAP_FAILED = -5,

    /// <summary>
    /// A requested layer is not present or could not be loaded.
    /// </summary>
    VK_ERROR_LAYER_NOT_PRESENT = -6,

    /// <summary>
    /// A requested extension is not supported.
    /// </summary>
    VK_ERROR_EXTENSION_NOT_PRESENT = -7,

    /// <summary>
    /// A requested feature is not supported.
    /// </summary>
    VK_ERROR_FEATURE_NOT_PRESENT = -8,

    /// <summary>
    /// The requested version of Vulkan is not supported by the driver or is otherwise incompatible for implementation-specific reasons.
    /// </summary>
    VK_ERROR_INCOMPATIBLE_DRIVER = -9,

    /// <summary>
    /// Too many objects of the type have already been created.
    /// </summary>
    VK_ERROR_TOO_MANY_OBJECTS = -10,

    /// <summary>
    /// A requested format is not supported on this device.
    /// </summary>
    VK_ERROR_FORMAT_NOT_SUPPORTED = -11,

    /// <summary>
    /// A pool allocation has failed due to fragmentation of the pool’s memory. This must only be returned if no attempt to allocate host or device memory was made to accommodate the new allocation. This should be returned in preference to <see cref="VK_ERROR_OUT_OF_POOL_MEMORY"/>, but only if the implementation is certain that the pool allocation failure was due to fragmentation.
    /// </summary>
    VK_ERROR_FRAGMENTED_POOL = -12,

    /// <summary>
    /// An unknown error has occurred; either the application has provided invalid input, or an implementation failure has occurred.
    /// </summary>
    VK_ERROR_UNKNOWN = -13,

    /// <summary>
    /// A pool memory allocation has failed. This must only be returned if no attempt to allocate host or device memory was made to accommodate the new allocation. If the failure was definitely due to fragmentation of the pool, <see cref="VK_ERROR_FRAGMENTED_POOL"/> should be returned instead.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_ERROR_OUT_OF_POOL_MEMORY = -1000069000,

    /// <summary>
    /// An external handle is not a valid handle of the specified type.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_ERROR_INVALID_EXTERNAL_HANDLE = -1000072003,

    /// <summary>
    /// A descriptor pool creation has failed due to fragmentation.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_ERROR_FRAGMENTATION = -1000161000,

    /// <summary>
    /// A buffer creation or memory allocation failed because the requested address is not available. A shader group handle assignment failed because the requested shader group handle information is no longer valid.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS = -1000257000,

    /// <summary>
    /// A requested pipeline creation would have required compilation, but the application requested compilation to not be performed.
    /// </summary>
    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_PIPELINE_COMPILE_REQUIRED = 1000297000,

    /// <summary>
    /// A surface is no longer available.
    /// </summary>
    /// <remarks>Provided by VK_KHR_surface</remarks>
    VK_ERROR_SURFACE_LOST_KHR = -1000000000,

    /// <summary>
    /// The requested window is already in use by Vulkan or another API in a manner which prevents it from being used again.
    /// </summary>
    /// <remarks>Provided by VK_KHR_surface</remarks>
    VK_ERROR_NATIVE_WINDOW_IN_USE_KHR = -1000000001,

    /// <summary>
    /// A swapchain no longer matches the surface properties exactly, but can still be used to present to the surface successfully.
    /// </summary>
    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_SUBOPTIMAL_KHR = 1000001003,

    /// <summary>
    /// A surface has changed in such a way that it is no longer compatible with the swapchain, and further presentation requests using the swapchain will fail. Applications must query the new surface properties and recreate their swapchain if they wish to continue presenting to the surface.
    /// </summary>
    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_ERROR_OUT_OF_DATE_KHR = -1000001004,

    /// <summary>
    /// The display used by a swapchain does not use the same presentable image layout, or is incompatible in a way that prevents sharing an image.
    /// </summary>
    /// <remarks>Provided by VK_KHR_display_swapchain</remarks>
    VK_ERROR_INCOMPATIBLE_DISPLAY_KHR = -1000003001,

    /// <summary>
    /// A command failed because invalid usage was detected by the implementation or a validation-layer.
    /// </summary>
    /// <remarks>Provided by VK_EXT_debug_report</remarks>
    VK_ERROR_VALIDATION_FAILED_EXT = -1000011001,

    /// <summary>
    /// One or more shaders failed to compile or link. More details are reported back to the application via VK_EXT_debug_report if enabled.
    /// </summary>
    /// <remarks>Provided by VK_NV_glsl_shader</remarks>
    VK_ERROR_INVALID_SHADER_NV = -1000012000,

    /// <summary>
    /// The requested VkImageUsageFlags are not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_IMAGE_USAGE_NOT_SUPPORTED_KHR = -1000023000,

    /// <summary>
    /// The requested video picture layout is not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PICTURE_LAYOUT_NOT_SUPPORTED_KHR = -1000023001,

    /// <summary>
    /// A video profile operation specified via <see cref="VkVideoProfileInfoKHR.videoCodecOperation"/> is not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_OPERATION_NOT_SUPPORTED_KHR = -1000023002,

    /// <summary>
    /// Format parameters in a requested <see cref="VkVideoProfileInfoKHR"/> chain are not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_FORMAT_NOT_SUPPORTED_KHR = -1000023003,

    /// <summary>
    /// Codec-specific parameters in a requested <see cref="VkVideoProfileInfoKHR"/> chain are not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_PROFILE_CODEC_NOT_SUPPORTED_KHR = -1000023004,

    /// <summary>
    /// The specified video Std header version is not supported.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_ERROR_VIDEO_STD_VERSION_NOT_SUPPORTED_KHR = -1000023005,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_ERROR_INVALID_DRM_FORMAT_MODIFIER_PLANE_LAYOUT_EXT = -1000158000,

    /// <remarks>Provided by VK_KHR_global_priority</remarks>
    VK_ERROR_NOT_PERMITTED_KHR = -1000174001,

    /// <summary>
    /// An operation on a swapchain created with <see cref="VK_FULL_SCREEN_EXCLUSIVE_APPLICATION_CONTROLLED_EXT"/> failed as it did not have exclusive full-screen access. This may occur due to implementation-dependent reasons, outside of the application’s control.
    /// </summary>
    /// <remarks>Provided by VK_EXT_full_screen_exclusive</remarks>
    VK_ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT = -1000255000,

    /// <summary>
    /// A deferred operation is not complete but there is currently no work for this thread to do at the time of this call.
    /// </summary>
    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_THREAD_IDLE_KHR = 1000268000,

    /// <summary>
    /// A deferred operation is not complete but there is no work remaining to assign to additional threads.
    /// </summary>
    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_THREAD_DONE_KHR = 1000268001,

    /// <summary>
    /// A deferred operation was requested and at least some of the work was deferred.
    /// </summary>
    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_OPERATION_DEFERRED_KHR = 1000268002,

    /// <summary>
    /// A deferred operation was requested and no operations were deferred.
    /// </summary>
    /// <remarks>Provided by VK_KHR_deferred_host_operations</remarks>
    VK_OPERATION_NOT_DEFERRED_KHR = 1000268003,

#if VK_ENABLE_BETA_EXTENSIONS
    /// <summary>
    /// The specified Video Std parameters do not adhere to the syntactic or semantic requirements of the used video compression standard, or values derived from parameters according to the rules defined by the used video compression standard do not adhere to the capabilities of the video compression standard or the implementation.
    /// </summary>
    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_ERROR_INVALID_VIDEO_STD_PARAMETERS_KHR = -1000299000,

#endif
    /// <summary>
    /// An image creation failed because internal resources required for compression are exhausted. This must only be returned when fixed-rate compression is requested.
    /// </summary>
    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_ERROR_COMPRESSION_EXHAUSTED_EXT = -1000338000,

    /// <summary>
    /// The provided binary shader code is not compatible with this device.
    /// </summary>
    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_ERROR_INCOMPATIBLE_SHADER_BINARY_EXT = 1000482000,

    /// <inheritdoc cref="VK_ERROR_OUT_OF_POOL_MEMORY" />
    /// <remarks>Provided by VK_KHR_maintenance1</remarks>
    VK_ERROR_OUT_OF_POOL_MEMORY_KHR = VK_ERROR_OUT_OF_POOL_MEMORY,

    /// <inheritdoc cref="VK_ERROR_INVALID_EXTERNAL_HANDLE" />
    /// <remarks>Provided by VK_KHR_external_memory</remarks>
    VK_ERROR_INVALID_EXTERNAL_HANDLE_KHR = VK_ERROR_INVALID_EXTERNAL_HANDLE,

    /// <inheritdoc cref="VK_ERROR_FRAGMENTATION" />
    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_ERROR_FRAGMENTATION_EXT = VK_ERROR_FRAGMENTATION,

    /// <inheritdoc cref="VK_ERROR_NOT_PERMITTED_KHR" />
    /// <remarks>Provided by VK_EXT_global_priority</remarks>
    VK_ERROR_NOT_PERMITTED_EXT = VK_ERROR_NOT_PERMITTED_KHR,

    /// <summary>
    /// A buffer creation failed because the requested address is not available.
    /// </summary>
    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_ERROR_INVALID_DEVICE_ADDRESS_EXT = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

    /// <inheritdoc cref="VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS" />
    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS_KHR = VK_ERROR_INVALID_OPAQUE_CAPTURE_ADDRESS,

    /// <inheritdoc cref="VK_PIPELINE_COMPILE_REQUIRED" />
    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_PIPELINE_COMPILE_REQUIRED_EXT = VK_PIPELINE_COMPILE_REQUIRED,

    /// <inheritdoc cref="VK_PIPELINE_COMPILE_REQUIRED" />
    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_ERROR_PIPELINE_COMPILE_REQUIRED_EXT = VK_PIPELINE_COMPILE_REQUIRED,
}

