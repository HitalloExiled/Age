namespace Age.Vulkan.Native.Enums;

/// <summary>
/// <para>Each value corresponds to a particular structure with a sType member with a matching name. As a general rule, the name of each VkStructureType value is obtained by taking the name of the structure, stripping the leading Vk, prefixing each capital letter with _, converting the entire resulting string to upper case, and prefixing it with VK_STRUCTURE_TYPE_. For example, structures of type <see cref="VkImageCreateInfo"/> correspond to a <see cref="VkStructureType"/> value of <see cref="VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO"/>, and thus a structure of this type must have its sType member set to this value before it is passed to the API.</para>
/// <para>The values <see cref="VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO"/> and <see cref="VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO"/> are reserved for internal use by the loader, and do not have corresponding Vulkan structures in this Specification.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkStructureType
{
    VK_STRUCTURE_TYPE_APPLICATION_INFO                                                    = 0,
    VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO                                                = 1,
    VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO                                            = 2,
    VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO                                                  = 3,
    VK_STRUCTURE_TYPE_SUBMIT_INFO                                                         = 4,
    VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO                                                = 5,
    VK_STRUCTURE_TYPE_MAPPED_MEMORY_RANGE                                                 = 6,
    VK_STRUCTURE_TYPE_BIND_SPARSE_INFO                                                    = 7,
    VK_STRUCTURE_TYPE_FENCE_CREATE_INFO                                                   = 8,
    VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO                                               = 9,
    VK_STRUCTURE_TYPE_EVENT_CREATE_INFO                                                   = 10,
    VK_STRUCTURE_TYPE_QUERY_POOL_CREATE_INFO                                              = 11,
    VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO                                                  = 12,
    VK_STRUCTURE_TYPE_BUFFER_VIEW_CREATE_INFO                                             = 13,
    VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO                                                   = 14,
    VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO                                              = 15,
    VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO                                           = 16,
    VK_STRUCTURE_TYPE_PIPELINE_CACHE_CREATE_INFO                                          = 17,
    VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO                                   = 18,
    VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO                             = 19,
    VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO                           = 20,
    VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_STATE_CREATE_INFO                             = 21,
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO                                 = 22,
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO                            = 23,
    VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO                              = 24,
    VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO                            = 25,
    VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO                              = 26,
    VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO                                  = 27,
    VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO                                       = 28,
    VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_CREATE_INFO                                        = 29,
    VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO                                         = 30,
    VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO                                                 = 31,
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO                                   = 32,
    VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO                                         = 33,
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO                                        = 34,
    VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET                                                = 35,
    VK_STRUCTURE_TYPE_COPY_DESCRIPTOR_SET                                                 = 36,
    VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO                                             = 37,
    VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO                                             = 38,
    VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO                                            = 39,
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO                                        = 40,
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO                                     = 41,
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO                                           = 42,
    VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO                                              = 43,
    VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER                                               = 44,
    VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER                                                = 45,
    VK_STRUCTURE_TYPE_MEMORY_BARRIER                                                      = 46,
    VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO                                         = 47,
    VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO                                           = 48,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_PROPERTIES                                 = 1000094000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_INFO                                             = 1000157000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_INFO                                              = 1000157001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES                              = 1000083000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_MEMORY_DEDICATED_REQUIREMENTS                                       = 1000127000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_MEMORY_DEDICATED_ALLOCATE_INFO                                      = 1000127001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO                                          = 1000060000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_RENDER_PASS_BEGIN_INFO                                 = 1000060003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_COMMAND_BUFFER_BEGIN_INFO                              = 1000060004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_SUBMIT_INFO                                            = 1000060005,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_BIND_SPARSE_INFO                                       = 1000060006,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_DEVICE_GROUP_INFO                                = 1000060013,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_DEVICE_GROUP_INFO                                 = 1000060014,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GROUP_PROPERTIES                                    = 1000070000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_DEVICE_CREATE_INFO                                     = 1000070001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BUFFER_MEMORY_REQUIREMENTS_INFO_2                                   = 1000146000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_IMAGE_MEMORY_REQUIREMENTS_INFO_2                                    = 1000146001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_IMAGE_SPARSE_MEMORY_REQUIREMENTS_INFO_2                             = 1000146002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_MEMORY_REQUIREMENTS_2                                               = 1000146003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_SPARSE_IMAGE_MEMORY_REQUIREMENTS_2                                  = 1000146004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2                                          = 1000059000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROPERTIES_2                                        = 1000059001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_2                                                 = 1000059002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_IMAGE_FORMAT_PROPERTIES_2                                           = 1000059003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_FORMAT_INFO_2                                 = 1000059004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_PROPERTIES_2                                           = 1000059005,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_PROPERTIES_2                                 = 1000059006,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_SPARSE_IMAGE_FORMAT_PROPERTIES_2                                    = 1000059007,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SPARSE_IMAGE_FORMAT_INFO_2                          = 1000059008,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_POINT_CLIPPING_PROPERTIES                           = 1000117000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_INPUT_ATTACHMENT_ASPECT_CREATE_INFO                     = 1000117001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_USAGE_CREATE_INFO                                        = 1000117002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_DOMAIN_ORIGIN_STATE_CREATE_INFO               = 1000117003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_MULTIVIEW_CREATE_INFO                                   = 1000053000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_FEATURES                                  = 1000053001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PROPERTIES                                = 1000053002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTERS_FEATURES                          = 1000120000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PROTECTED_SUBMIT_INFO                                               = 1000145000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROTECTED_MEMORY_FEATURES                           = 1000145001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROTECTED_MEMORY_PROPERTIES                         = 1000145002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DEVICE_QUEUE_INFO_2                                                 = 1000145003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_CREATE_INFO                                = 1000156000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_INFO                                       = 1000156001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_PLANE_MEMORY_INFO                                        = 1000156002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_IMAGE_PLANE_MEMORY_REQUIREMENTS_INFO                                = 1000156003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_YCBCR_CONVERSION_FEATURES                   = 1000156004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_IMAGE_FORMAT_PROPERTIES                    = 1000156005,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_UPDATE_TEMPLATE_CREATE_INFO                              = 1000085000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_IMAGE_FORMAT_INFO                          = 1000071000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_IMAGE_FORMAT_PROPERTIES                                    = 1000071001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_BUFFER_INFO                                = 1000071002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_BUFFER_PROPERTIES                                          = 1000071003,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ID_PROPERTIES                                       = 1000071004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_BUFFER_CREATE_INFO                                  = 1000072000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_IMAGE_CREATE_INFO                                   = 1000072001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXPORT_MEMORY_ALLOCATE_INFO                                         = 1000072002,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_FENCE_INFO                                 = 1000112000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_FENCE_PROPERTIES                                           = 1000112001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXPORT_FENCE_CREATE_INFO                                            = 1000113000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXPORT_SEMAPHORE_CREATE_INFO                                        = 1000077000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_SEMAPHORE_INFO                             = 1000076000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_SEMAPHORE_PROPERTIES                                       = 1000076001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_3_PROPERTIES                            = 1000168000,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_SUPPORT                                       = 1000168001,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DRAW_PARAMETERS_FEATURES                     = 1000063000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_1_FEATURES                                 = 49,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_1_PROPERTIES                               = 50,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_2_FEATURES                                 = 51,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_2_PROPERTIES                               = 52,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_FORMAT_LIST_CREATE_INFO                                       = 1000147000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_2                                            = 1000109000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_2                                              = 1000109001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_2                                               = 1000109002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DEPENDENCY_2                                                = 1000109003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO_2                                           = 1000109004,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_BEGIN_INFO                                                  = 1000109005,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_END_INFO                                                    = 1000109006,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_8BIT_STORAGE_FEATURES                               = 1000177000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DRIVER_PROPERTIES                                   = 1000196000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ATOMIC_INT64_FEATURES                        = 1000180000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_FLOAT16_INT8_FEATURES                        = 1000082000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FLOAT_CONTROLS_PROPERTIES                           = 1000197000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_BINDING_FLAGS_CREATE_INFO                     = 1000161000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_FEATURES                        = 1000161001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_PROPERTIES                      = 1000161002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_ALLOCATE_INFO              = 1000161003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_LAYOUT_SUPPORT             = 1000161004,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_STENCIL_RESOLVE_PROPERTIES                    = 1000199000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_DEPTH_STENCIL_RESOLVE                           = 1000199001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SCALAR_BLOCK_LAYOUT_FEATURES                        = 1000221000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_STENCIL_USAGE_CREATE_INFO                                     = 1000246000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_FILTER_MINMAX_PROPERTIES                    = 1000130000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_REDUCTION_MODE_CREATE_INFO                                  = 1000130001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_MEMORY_MODEL_FEATURES                        = 1000211000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGELESS_FRAMEBUFFER_FEATURES                      = 1000108000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENTS_CREATE_INFO                                 = 1000108001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENT_IMAGE_INFO                                   = 1000108002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_ATTACHMENT_BEGIN_INFO                                   = 1000108003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_UNIFORM_BUFFER_STANDARD_LAYOUT_FEATURES             = 1000253000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SUBGROUP_EXTENDED_TYPES_FEATURES             = 1000175000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SEPARATE_DEPTH_STENCIL_LAYOUTS_FEATURES             = 1000241000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_STENCIL_LAYOUT                                 = 1000241001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_STENCIL_LAYOUT                               = 1000241002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_HOST_QUERY_RESET_FEATURES                           = 1000261000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_FEATURES                         = 1000207000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_PROPERTIES                       = 1000207001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_TYPE_CREATE_INFO                                          = 1000207002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_TIMELINE_SEMAPHORE_SUBMIT_INFO                                      = 1000207003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_WAIT_INFO                                                 = 1000207004,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_SIGNAL_INFO                                               = 1000207005,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_DEVICE_ADDRESS_FEATURES                      = 1000257000,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_INFO                                          = 1000244001,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_OPAQUE_CAPTURE_ADDRESS_CREATE_INFO                           = 1000257002,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_MEMORY_OPAQUE_CAPTURE_ADDRESS_ALLOCATE_INFO                         = 1000257003,

    /// <remarks>Provided by VK_VERSION_1_2</remarks>
    VK_STRUCTURE_TYPE_DEVICE_MEMORY_OPAQUE_CAPTURE_ADDRESS_INFO                           = 1000257004,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_3_FEATURES                                 = 53,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_1_3_PROPERTIES                               = 54,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_CREATION_FEEDBACK_CREATE_INFO                              = 1000192000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_TERMINATE_INVOCATION_FEATURES                = 1000215000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TOOL_PROPERTIES                                     = 1000245000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DEMOTE_TO_HELPER_INVOCATION_FEATURES         = 1000276000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRIVATE_DATA_FEATURES                               = 1000295000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_DEVICE_PRIVATE_DATA_CREATE_INFO                                     = 1000295001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PRIVATE_DATA_SLOT_CREATE_INFO                                       = 1000295002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_CREATION_CACHE_CONTROL_FEATURES            = 1000297000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_MEMORY_BARRIER_2                                                    = 1000314000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER_2                                             = 1000314001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER_2                                              = 1000314002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_DEPENDENCY_INFO                                                     = 1000314003,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_SUBMIT_INFO_2                                                       = 1000314004,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_SUBMIT_INFO                                               = 1000314005,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_SUBMIT_INFO                                          = 1000314006,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SYNCHRONIZATION_2_FEATURES                          = 1000314007,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ZERO_INITIALIZE_WORKGROUP_MEMORY_FEATURES           = 1000325000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_ROBUSTNESS_FEATURES                           = 1000335000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COPY_BUFFER_INFO_2                                                  = 1000337000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_INFO_2                                                   = 1000337001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COPY_BUFFER_TO_IMAGE_INFO_2                                         = 1000337002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_TO_BUFFER_INFO_2                                         = 1000337003,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_BLIT_IMAGE_INFO_2                                                   = 1000337004,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_RESOLVE_IMAGE_INFO_2                                                = 1000337005,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COPY_2                                                       = 1000337006,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_IMAGE_COPY_2                                                        = 1000337007,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_IMAGE_BLIT_2                                                        = 1000337008,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_BUFFER_IMAGE_COPY_2                                                 = 1000337009,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_IMAGE_RESOLVE_2                                                     = 1000337010,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_PROPERTIES                    = 1000225000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_REQUIRED_SUBGROUP_SIZE_CREATE_INFO            = 1000225001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_FEATURES                      = 1000225002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_FEATURES                       = 1000138000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_PROPERTIES                     = 1000138001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET_INLINE_UNIFORM_BLOCK                           = 1000138002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_INLINE_UNIFORM_BLOCK_CREATE_INFO                    = 1000138003,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXTURE_COMPRESSION_ASTC_HDR_FEATURES               = 1000066000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_RENDERING_INFO                                                      = 1000044000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_RENDERING_ATTACHMENT_INFO                                           = 1000044001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RENDERING_CREATE_INFO                                      = 1000044002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DYNAMIC_RENDERING_FEATURES                          = 1000044003,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_RENDERING_INFO                           = 1000044004,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_FEATURES                 = 1000280000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_PROPERTIES               = 1000280001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXEL_BUFFER_ALIGNMENT_PROPERTIES                   = 1000281001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_3                                                 = 1000360000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_FEATURES                              = 1000413000,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_PROPERTIES                            = 1000413001,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_DEVICE_BUFFER_MEMORY_REQUIREMENTS                                   = 1000413002,

    /// <remarks>Provided by VK_VERSION_1_3</remarks>
    VK_STRUCTURE_TYPE_DEVICE_IMAGE_MEMORY_REQUIREMENTS                                    = 1000413003,

    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR                                           = 1000001000,

    /// <remarks>Provided by VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_PRESENT_INFO_KHR                                                    = 1000001001,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_surface</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_PRESENT_CAPABILITIES_KHR                               = 1000060007,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_IMAGE_SWAPCHAIN_CREATE_INFO_KHR                                     = 1000060008,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_SWAPCHAIN_INFO_KHR                                = 1000060009,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_ACQUIRE_NEXT_IMAGE_INFO_KHR                                         = 1000060010,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_PRESENT_INFO_KHR                                       = 1000060011,

    /// <remarks>Provided by VK_VERSION_1_1 with VK_KHR_swapchain, VK_KHR_device_group with VK_KHR_swapchain</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_SWAPCHAIN_CREATE_INFO_KHR                              = 1000060012,

    /// <remarks>Provided by VK_KHR_display</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_MODE_CREATE_INFO_KHR                                        = 1000002000,

    /// <remarks>Provided by VK_KHR_display</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_SURFACE_CREATE_INFO_KHR                                     = 1000002001,

    /// <remarks>Provided by VK_KHR_display_swapchain</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_PRESENT_INFO_KHR                                            = 1000003000,

    /// <remarks>Provided by VK_KHR_xlib_surface</remarks>
    VK_STRUCTURE_TYPE_XLIB_SURFACE_CREATE_INFO_KHR                                        = 1000004000,

    /// <remarks>Provided by VK_KHR_xcb_surface</remarks>
    VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR                                         = 1000005000,

    /// <remarks>Provided by VK_KHR_wayland_surface</remarks>
    VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR                                     = 1000006000,

    /// <remarks>Provided by VK_KHR_android_surface</remarks>
    VK_STRUCTURE_TYPE_ANDROID_SURFACE_CREATE_INFO_KHR                                     = 1000008000,

    /// <remarks>Provided by VK_KHR_win32_surface</remarks>
    VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR                                       = 1000009000,

    /// <remarks>Provided by VK_EXT_debug_report</remarks>
    VK_STRUCTURE_TYPE_DEBUG_REPORT_CALLBACK_CREATE_INFO_EXT                               = 1000011000,

    /// <remarks>Provided by VK_AMD_rasterization_order</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_RASTERIZATION_ORDER_AMD                = 1000018000,

    /// <remarks>Provided by VK_EXT_debug_marker</remarks>
    VK_STRUCTURE_TYPE_DEBUG_MARKER_OBJECT_NAME_INFO_EXT                                   = 1000022000,

    /// <remarks>Provided by VK_EXT_debug_marker</remarks>
    VK_STRUCTURE_TYPE_DEBUG_MARKER_OBJECT_TAG_INFO_EXT                                    = 1000022001,

    /// <remarks>Provided by VK_EXT_debug_marker</remarks>
    VK_STRUCTURE_TYPE_DEBUG_MARKER_MARKER_INFO_EXT                                        = 1000022002,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_PROFILE_INFO_KHR                                              = 1000023000,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_CAPABILITIES_KHR                                              = 1000023001,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_PICTURE_RESOURCE_INFO_KHR                                     = 1000023002,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_SESSION_MEMORY_REQUIREMENTS_KHR                               = 1000023003,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_BIND_VIDEO_SESSION_MEMORY_INFO_KHR                                  = 1000023004,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_SESSION_CREATE_INFO_KHR                                       = 1000023005,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_SESSION_PARAMETERS_CREATE_INFO_KHR                            = 1000023006,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_SESSION_PARAMETERS_UPDATE_INFO_KHR                            = 1000023007,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_BEGIN_CODING_INFO_KHR                                         = 1000023008,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_END_CODING_INFO_KHR                                           = 1000023009,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_CODING_CONTROL_INFO_KHR                                       = 1000023010,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_REFERENCE_SLOT_INFO_KHR                                       = 1000023011,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_VIDEO_PROPERTIES_KHR                                   = 1000023012,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_PROFILE_LIST_INFO_KHR                                         = 1000023013,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VIDEO_FORMAT_INFO_KHR                               = 1000023014,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_FORMAT_PROPERTIES_KHR                                         = 1000023015,

    /// <remarks>Provided by VK_KHR_video_queue</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_QUERY_RESULT_STATUS_PROPERTIES_KHR                     = 1000023016,

    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_INFO_KHR                                               = 1000024000,

    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_CAPABILITIES_KHR                                       = 1000024001,

    /// <remarks>Provided by VK_KHR_video_decode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_USAGE_INFO_KHR                                         = 1000024002,

    /// <remarks>Provided by VK_NV_dedicated_allocation</remarks>
    VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_IMAGE_CREATE_INFO_NV                           = 1000026000,

    /// <remarks>Provided by VK_NV_dedicated_allocation</remarks>
    VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_BUFFER_CREATE_INFO_NV                          = 1000026001,

    /// <remarks>Provided by VK_NV_dedicated_allocation</remarks>
    VK_STRUCTURE_TYPE_DEDICATED_ALLOCATION_MEMORY_ALLOCATE_INFO_NV                        = 1000026002,

    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TRANSFORM_FEEDBACK_FEATURES_EXT                     = 1000028000,

    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TRANSFORM_FEEDBACK_PROPERTIES_EXT                   = 1000028001,

    /// <remarks>Provided by VK_EXT_transform_feedback</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_STREAM_CREATE_INFO_EXT                 = 1000028002,

    /// <remarks>Provided by VK_NVX_binary_import</remarks>
    VK_STRUCTURE_TYPE_CU_MODULE_CREATE_INFO_NVX                                           = 1000029000,

    /// <remarks>Provided by VK_NVX_binary_import</remarks>
    VK_STRUCTURE_TYPE_CU_FUNCTION_CREATE_INFO_NVX                                         = 1000029001,

    /// <remarks>Provided by VK_NVX_binary_import</remarks>
    VK_STRUCTURE_TYPE_CU_LAUNCH_INFO_NVX                                                  = 1000029002,

    /// <remarks>Provided by VK_NVX_image_view_handle</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_HANDLE_INFO_NVX                                          = 1000030000,

    /// <remarks>Provided by VK_NVX_image_view_handle</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_ADDRESS_PROPERTIES_NVX                                   = 1000030001,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_CAPABILITIES_EXT                                  = 1000038000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_SESSION_PARAMETERS_CREATE_INFO_EXT                = 1000038001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_SESSION_PARAMETERS_ADD_INFO_EXT                   = 1000038002,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_PICTURE_INFO_EXT                                  = 1000038003,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_DPB_SLOT_INFO_EXT                                 = 1000038004,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_NALU_SLICE_INFO_EXT                               = 1000038005,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_GOP_REMAINING_FRAME_INFO_EXT                      = 1000038006,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_PROFILE_INFO_EXT                                  = 1000038007,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_RATE_CONTROL_INFO_EXT                             = 1000038008,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_RATE_CONTROL_LAYER_INFO_EXT                       = 1000038009,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_SESSION_CREATE_INFO_EXT                           = 1000038010,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_QUALITY_LEVEL_PROPERTIES_EXT                      = 1000038011,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_SESSION_PARAMETERS_GET_INFO_EXT                   = 1000038012,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H264_SESSION_PARAMETERS_FEEDBACK_INFO_EXT              = 1000038013,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_CAPABILITIES_EXT                                  = 1000039000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_SESSION_PARAMETERS_CREATE_INFO_EXT                = 1000039001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_SESSION_PARAMETERS_ADD_INFO_EXT                   = 1000039002,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_PICTURE_INFO_EXT                                  = 1000039003,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_DPB_SLOT_INFO_EXT                                 = 1000039004,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_NALU_SLICE_SEGMENT_INFO_EXT                       = 1000039005,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_GOP_REMAINING_FRAME_INFO_EXT                      = 1000039006,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_PROFILE_INFO_EXT                                  = 1000039007,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_RATE_CONTROL_INFO_EXT                             = 1000039009,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_RATE_CONTROL_LAYER_INFO_EXT                       = 1000039010,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_SESSION_CREATE_INFO_EXT                           = 1000039011,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_QUALITY_LEVEL_PROPERTIES_EXT                      = 1000039012,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_SESSION_PARAMETERS_GET_INFO_EXT                   = 1000039013,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_EXT_video_encode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_H265_SESSION_PARAMETERS_FEEDBACK_INFO_EXT              = 1000039014,
#endif

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_CAPABILITIES_KHR                                  = 1000040000,

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_PICTURE_INFO_KHR                                  = 1000040001,

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_PROFILE_INFO_KHR                                  = 1000040003,

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_SESSION_PARAMETERS_CREATE_INFO_KHR                = 1000040004,

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_SESSION_PARAMETERS_ADD_INFO_KHR                   = 1000040005,

    /// <remarks>Provided by VK_KHR_video_decode_h264</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H264_DPB_SLOT_INFO_KHR                                 = 1000040006,

    /// <remarks>Provided by VK_AMD_texture_gather_bias_lod</remarks>
    VK_STRUCTURE_TYPE_TEXTURE_LOD_GATHER_FORMAT_PROPERTIES_AMD                            = 1000041000,

    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_RENDERING_FRAGMENT_SHADING_RATE_ATTACHMENT_INFO_KHR                 = 1000044006,

    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_EXT_fragment_density_map</remarks>
    VK_STRUCTURE_TYPE_RENDERING_FRAGMENT_DENSITY_MAP_ATTACHMENT_INFO_EXT                  = 1000044007,

    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_AMD_mixed_attachment_samples</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_SAMPLE_COUNT_INFO_AMD                                    = 1000044008,

    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_NVX_multiview_per_view_attributes</remarks>
    VK_STRUCTURE_TYPE_MULTIVIEW_PER_VIEW_ATTRIBUTES_INFO_NVX                              = 1000044009,

    /// <remarks>Provided by VK_GGP_stream_descriptor_surface</remarks>
    VK_STRUCTURE_TYPE_STREAM_DESCRIPTOR_SURFACE_CREATE_INFO_GGP                           = 1000049000,

    /// <remarks>Provided by VK_NV_corner_sampled_image</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CORNER_SAMPLED_IMAGE_FEATURES_NV                    = 1000050000,

    /// <remarks>Provided by VK_NV_external_memory</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_IMAGE_CREATE_INFO_NV                                = 1000056000,

    /// <remarks>Provided by VK_NV_external_memory</remarks>
    VK_STRUCTURE_TYPE_EXPORT_MEMORY_ALLOCATE_INFO_NV                                      = 1000056001,

    /// <remarks>Provided by VK_NV_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_WIN32_HANDLE_INFO_NV                                  = 1000057000,

    /// <remarks>Provided by VK_NV_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_EXPORT_MEMORY_WIN32_HANDLE_INFO_NV                                  = 1000057001,

    /// <remarks>Provided by VK_NV_win32_keyed_mutex</remarks>
    VK_STRUCTURE_TYPE_WIN32_KEYED_MUTEX_ACQUIRE_RELEASE_INFO_NV                           = 1000058000,

    /// <remarks>Provided by VK_EXT_validation_flags</remarks>
    VK_STRUCTURE_TYPE_VALIDATION_FLAGS_EXT                                                = 1000061000,

    /// <remarks>Provided by VK_NN_vi_surface</remarks>
    VK_STRUCTURE_TYPE_VI_SURFACE_CREATE_INFO_NN                                           = 1000062000,

    /// <remarks>Provided by VK_EXT_astc_decode_mode</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_ASTC_DECODE_MODE_EXT                                     = 1000067000,

    /// <remarks>Provided by VK_EXT_astc_decode_mode</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ASTC_DECODE_FEATURES_EXT                            = 1000067001,

    /// <remarks>Provided by VK_EXT_pipeline_robustness</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_ROBUSTNESS_CREATE_INFO_EXT                                 = 1000068000,

    /// <remarks>Provided by VK_EXT_pipeline_robustness</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_ROBUSTNESS_FEATURES_EXT                    = 1000068001,

    /// <remarks>Provided by VK_EXT_pipeline_robustness</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_ROBUSTNESS_PROPERTIES_EXT                  = 1000068002,

    /// <remarks>Provided by VK_KHR_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_WIN32_HANDLE_INFO_KHR                                 = 1000073000,

    /// <remarks>Provided by VK_KHR_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_EXPORT_MEMORY_WIN32_HANDLE_INFO_KHR                                 = 1000073001,

    /// <remarks>Provided by VK_KHR_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_MEMORY_WIN32_HANDLE_PROPERTIES_KHR                                  = 1000073002,

    /// <remarks>Provided by VK_KHR_external_memory_win32</remarks>
    VK_STRUCTURE_TYPE_MEMORY_GET_WIN32_HANDLE_INFO_KHR                                    = 1000073003,

    /// <remarks>Provided by VK_KHR_external_memory_fd</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_FD_INFO_KHR                                           = 1000074000,

    /// <remarks>Provided by VK_KHR_external_memory_fd</remarks>
    VK_STRUCTURE_TYPE_MEMORY_FD_PROPERTIES_KHR                                            = 1000074001,

    /// <remarks>Provided by VK_KHR_external_memory_fd</remarks>
    VK_STRUCTURE_TYPE_MEMORY_GET_FD_INFO_KHR                                              = 1000074002,

    /// <remarks>Provided by VK_KHR_win32_keyed_mutex</remarks>
    VK_STRUCTURE_TYPE_WIN32_KEYED_MUTEX_ACQUIRE_RELEASE_INFO_KHR                          = 1000075000,

    /// <remarks>Provided by VK_KHR_external_semaphore_win32</remarks>
    VK_STRUCTURE_TYPE_IMPORT_SEMAPHORE_WIN32_HANDLE_INFO_KHR                              = 1000078000,

    /// <remarks>Provided by VK_KHR_external_semaphore_win32</remarks>
    VK_STRUCTURE_TYPE_EXPORT_SEMAPHORE_WIN32_HANDLE_INFO_KHR                              = 1000078001,

    /// <remarks>Provided by VK_KHR_external_semaphore_win32</remarks>
    VK_STRUCTURE_TYPE_D3D12_FENCE_SUBMIT_INFO_KHR                                         = 1000078002,

    /// <remarks>Provided by VK_KHR_external_semaphore_win32</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_GET_WIN32_HANDLE_INFO_KHR                                 = 1000078003,

    /// <remarks>Provided by VK_KHR_external_semaphore_fd</remarks>
    VK_STRUCTURE_TYPE_IMPORT_SEMAPHORE_FD_INFO_KHR                                        = 1000079000,

    /// <remarks>Provided by VK_KHR_external_semaphore_fd</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_GET_FD_INFO_KHR                                           = 1000079001,

    /// <remarks>Provided by VK_KHR_push_descriptor</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PUSH_DESCRIPTOR_PROPERTIES_KHR                      = 1000080000,

    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_CONDITIONAL_RENDERING_INFO_EXT           = 1000081000,

    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CONDITIONAL_RENDERING_FEATURES_EXT                  = 1000081001,

    /// <remarks>Provided by VK_EXT_conditional_rendering</remarks>
    VK_STRUCTURE_TYPE_CONDITIONAL_RENDERING_BEGIN_INFO_EXT                                = 1000081002,

    /// <remarks>Provided by VK_KHR_incremental_present</remarks>
    VK_STRUCTURE_TYPE_PRESENT_REGIONS_KHR                                                 = 1000084000,

    /// <remarks>Provided by VK_NV_clip_space_w_scaling</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_W_SCALING_STATE_CREATE_INFO_NV                    = 1000087000,

    /// <remarks>Provided by VK_EXT_display_surface_counter</remarks>
    VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_2_EXT                                          = 1000090000,

    /// <remarks>Provided by VK_EXT_display_control</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_POWER_INFO_EXT                                              = 1000091000,

    /// <remarks>Provided by VK_EXT_display_control</remarks>
    VK_STRUCTURE_TYPE_DEVICE_EVENT_INFO_EXT                                               = 1000091001,

    /// <remarks>Provided by VK_EXT_display_control</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_EVENT_INFO_EXT                                              = 1000091002,

    /// <remarks>Provided by VK_EXT_display_control</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_COUNTER_CREATE_INFO_EXT                                   = 1000091003,

    /// <remarks>Provided by VK_GOOGLE_display_timing</remarks>
    VK_STRUCTURE_TYPE_PRESENT_TIMES_INFO_GOOGLE                                           = 1000092000,

    /// <remarks>Provided by VK_NVX_multiview_per_view_attributes</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PER_VIEW_ATTRIBUTES_PROPERTIES_NVX        = 1000097000,

    /// <remarks>Provided by VK_NV_viewport_swizzle</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_SWIZZLE_STATE_CREATE_INFO_NV                      = 1000098000,

    /// <remarks>Provided by VK_EXT_discard_rectangles</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DISCARD_RECTANGLE_PROPERTIES_EXT                    = 1000099000,

    /// <remarks>Provided by VK_EXT_discard_rectangles</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_DISCARD_RECTANGLE_STATE_CREATE_INFO_EXT                    = 1000099001,

    /// <remarks>Provided by VK_EXT_conservative_rasterization</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CONSERVATIVE_RASTERIZATION_PROPERTIES_EXT           = 1000101000,

    /// <remarks>Provided by VK_EXT_conservative_rasterization</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_CONSERVATIVE_STATE_CREATE_INFO_EXT           = 1000101001,

    /// <remarks>Provided by VK_EXT_depth_clip_enable</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_CLIP_ENABLE_FEATURES_EXT                      = 1000102000,

    /// <remarks>Provided by VK_EXT_depth_clip_enable</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_DEPTH_CLIP_STATE_CREATE_INFO_EXT             = 1000102001,

    /// <remarks>Provided by VK_EXT_hdr_metadata</remarks>
    VK_STRUCTURE_TYPE_HDR_METADATA_EXT                                                    = 1000105000,

    /// <remarks>Provided by VK_KHR_shared_presentable_image</remarks>
    VK_STRUCTURE_TYPE_SHARED_PRESENT_SURFACE_CAPABILITIES_KHR                             = 1000111000,

    /// <remarks>Provided by VK_KHR_external_fence_win32</remarks>
    VK_STRUCTURE_TYPE_IMPORT_FENCE_WIN32_HANDLE_INFO_KHR                                  = 1000114000,

    /// <remarks>Provided by VK_KHR_external_fence_win32</remarks>
    VK_STRUCTURE_TYPE_EXPORT_FENCE_WIN32_HANDLE_INFO_KHR                                  = 1000114001,

    /// <remarks>Provided by VK_KHR_external_fence_win32</remarks>
    VK_STRUCTURE_TYPE_FENCE_GET_WIN32_HANDLE_INFO_KHR                                     = 1000114002,

    /// <remarks>Provided by VK_KHR_external_fence_fd</remarks>
    VK_STRUCTURE_TYPE_IMPORT_FENCE_FD_INFO_KHR                                            = 1000115000,

    /// <remarks>Provided by VK_KHR_external_fence_fd</remarks>
    VK_STRUCTURE_TYPE_FENCE_GET_FD_INFO_KHR                                               = 1000115001,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PERFORMANCE_QUERY_FEATURES_KHR                      = 1000116000,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PERFORMANCE_QUERY_PROPERTIES_KHR                    = 1000116001,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_QUERY_POOL_PERFORMANCE_CREATE_INFO_KHR                              = 1000116002,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_QUERY_SUBMIT_INFO_KHR                                   = 1000116003,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_ACQUIRE_PROFILING_LOCK_INFO_KHR                                     = 1000116004,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_COUNTER_KHR                                             = 1000116005,

    /// <remarks>Provided by VK_KHR_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_COUNTER_DESCRIPTION_KHR                                 = 1000116006,

    /// <remarks>Provided by VK_KHR_get_surface_capabilities2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SURFACE_INFO_2_KHR                                  = 1000119000,

    /// <remarks>Provided by VK_KHR_get_surface_capabilities2</remarks>
    VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_2_KHR                                          = 1000119001,

    /// <remarks>Provided by VK_KHR_get_surface_capabilities2</remarks>
    VK_STRUCTURE_TYPE_SURFACE_FORMAT_2_KHR                                                = 1000119002,

    /// <remarks>Provided by VK_KHR_get_display_properties2</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_PROPERTIES_2_KHR                                            = 1000121000,

    /// <remarks>Provided by VK_KHR_get_display_properties2</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_PLANE_PROPERTIES_2_KHR                                      = 1000121001,

    /// <remarks>Provided by VK_KHR_get_display_properties2</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_MODE_PROPERTIES_2_KHR                                       = 1000121002,

    /// <remarks>Provided by VK_KHR_get_display_properties2</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_PLANE_INFO_2_KHR                                            = 1000121003,

    /// <remarks>Provided by VK_KHR_get_display_properties2</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_PLANE_CAPABILITIES_2_KHR                                    = 1000121004,

    /// <remarks>Provided by VK_MVK_ios_surface</remarks>
    VK_STRUCTURE_TYPE_IOS_SURFACE_CREATE_INFO_MVK                                         = 1000122000,

    /// <remarks>Provided by VK_MVK_macos_surface</remarks>
    VK_STRUCTURE_TYPE_MACOS_SURFACE_CREATE_INFO_MVK                                       = 1000123000,

    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_STRUCTURE_TYPE_DEBUG_UTILS_OBJECT_NAME_INFO_EXT                                    = 1000128000,

    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_STRUCTURE_TYPE_DEBUG_UTILS_OBJECT_TAG_INFO_EXT                                     = 1000128001,

    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_STRUCTURE_TYPE_DEBUG_UTILS_LABEL_EXT                                               = 1000128002,

    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CALLBACK_DATA_EXT                             = 1000128003,

    /// <remarks>Provided by VK_EXT_debug_utils</remarks>
    VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT                               = 1000128004,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_ANDROID_HARDWARE_BUFFER_USAGE_ANDROID                               = 1000129000,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_ANDROID_HARDWARE_BUFFER_PROPERTIES_ANDROID                          = 1000129001,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_ANDROID_HARDWARE_BUFFER_FORMAT_PROPERTIES_ANDROID                   = 1000129002,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_IMPORT_ANDROID_HARDWARE_BUFFER_INFO_ANDROID                         = 1000129003,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_MEMORY_GET_ANDROID_HARDWARE_BUFFER_INFO_ANDROID                     = 1000129004,

    /// <remarks>Provided by VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_FORMAT_ANDROID                                             = 1000129005,

    /// <remarks>Provided by VK_KHR_format_feature_flags2 with VK_ANDROID_external_memory_android_hardware_buffer</remarks>
    VK_STRUCTURE_TYPE_ANDROID_HARDWARE_BUFFER_FORMAT_PROPERTIES_2_ANDROID                 = 1000129006,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ENQUEUE_FEATURES_AMDX                        = 1000134000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ENQUEUE_PROPERTIES_AMDX                      = 1000134001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_STRUCTURE_TYPE_EXECUTION_GRAPH_PIPELINE_SCRATCH_SIZE_AMDX                          = 1000134002,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_STRUCTURE_TYPE_EXECUTION_GRAPH_PIPELINE_CREATE_INFO_AMDX                           = 1000134003,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_AMDX_shader_enqueue</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_NODE_CREATE_INFO_AMDX                         = 1000134004,
#endif

    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_STRUCTURE_TYPE_SAMPLE_LOCATIONS_INFO_EXT                                           = 1000143000,

    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_SAMPLE_LOCATIONS_BEGIN_INFO_EXT                         = 1000143001,

    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_SAMPLE_LOCATIONS_STATE_CREATE_INFO_EXT                     = 1000143002,

    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLE_LOCATIONS_PROPERTIES_EXT                     = 1000143003,

    /// <remarks>Provided by VK_EXT_sample_locations</remarks>
    VK_STRUCTURE_TYPE_MULTISAMPLE_PROPERTIES_EXT                                          = 1000143004,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BLEND_OPERATION_ADVANCED_FEATURES_EXT               = 1000148000,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BLEND_OPERATION_ADVANCED_PROPERTIES_EXT             = 1000148001,

    /// <remarks>Provided by VK_EXT_blend_operation_advanced</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_ADVANCED_STATE_CREATE_INFO_EXT                 = 1000148002,

    /// <remarks>Provided by VK_NV_fragment_coverage_to_color</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COVERAGE_TO_COLOR_STATE_CREATE_INFO_NV                     = 1000149000,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET_ACCELERATION_STRUCTURE_KHR                     = 1000150007,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_BUILD_GEOMETRY_INFO_KHR                      = 1000150000,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_DEVICE_ADDRESS_INFO_KHR                      = 1000150002,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_GEOMETRY_AABBS_DATA_KHR                      = 1000150003,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_GEOMETRY_INSTANCES_DATA_KHR                  = 1000150004,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_GEOMETRY_TRIANGLES_DATA_KHR                  = 1000150005,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_GEOMETRY_KHR                                 = 1000150006,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_VERSION_INFO_KHR                             = 1000150009,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_COPY_ACCELERATION_STRUCTURE_INFO_KHR                                = 1000150010,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_COPY_ACCELERATION_STRUCTURE_TO_MEMORY_INFO_KHR                      = 1000150011,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_COPY_MEMORY_TO_ACCELERATION_STRUCTURE_INFO_KHR                      = 1000150012,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ACCELERATION_STRUCTURE_FEATURES_KHR                 = 1000150013,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ACCELERATION_STRUCTURE_PROPERTIES_KHR               = 1000150014,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_CREATE_INFO_KHR                              = 1000150017,

    /// <remarks>Provided by VK_KHR_acceleration_structure</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_BUILD_SIZES_INFO_KHR                         = 1000150020,

    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_PIPELINE_FEATURES_KHR                   = 1000347000,

    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_PIPELINE_PROPERTIES_KHR                 = 1000347001,

    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_STRUCTURE_TYPE_RAY_TRACING_PIPELINE_CREATE_INFO_KHR                                = 1000150015,

    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_STRUCTURE_TYPE_RAY_TRACING_SHADER_GROUP_CREATE_INFO_KHR                            = 1000150016,

    /// <remarks>Provided by VK_KHR_ray_tracing_pipeline</remarks>
    VK_STRUCTURE_TYPE_RAY_TRACING_PIPELINE_INTERFACE_CREATE_INFO_KHR                      = 1000150018,

    /// <remarks>Provided by VK_KHR_ray_query</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_QUERY_FEATURES_KHR                              = 1000348013,

    /// <remarks>Provided by VK_NV_framebuffer_mixed_samples</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COVERAGE_MODULATION_STATE_CREATE_INFO_NV                   = 1000152000,

    /// <remarks>Provided by VK_NV_shader_sm_builtins</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SM_BUILTINS_FEATURES_NV                      = 1000154000,

    /// <remarks>Provided by VK_NV_shader_sm_builtins</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SM_BUILTINS_PROPERTIES_NV                    = 1000154001,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_DRM_FORMAT_MODIFIER_PROPERTIES_LIST_EXT                             = 1000158000,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_DRM_FORMAT_MODIFIER_INFO_EXT                  = 1000158002,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_IMAGE_DRM_FORMAT_MODIFIER_LIST_CREATE_INFO_EXT                      = 1000158003,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_IMAGE_DRM_FORMAT_MODIFIER_EXPLICIT_CREATE_INFO_EXT                  = 1000158004,

    /// <remarks>Provided by VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_IMAGE_DRM_FORMAT_MODIFIER_PROPERTIES_EXT                            = 1000158005,

    /// <remarks>Provided by VK_KHR_format_feature_flags2 with VK_EXT_image_drm_format_modifier</remarks>
    VK_STRUCTURE_TYPE_DRM_FORMAT_MODIFIER_PROPERTIES_LIST_2_EXT                           = 1000158006,

    /// <remarks>Provided by VK_EXT_validation_cache</remarks>
    VK_STRUCTURE_TYPE_VALIDATION_CACHE_CREATE_INFO_EXT                                    = 1000160000,

    /// <remarks>Provided by VK_EXT_validation_cache</remarks>
    VK_STRUCTURE_TYPE_SHADER_MODULE_VALIDATION_CACHE_CREATE_INFO_EXT                      = 1000160001,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_portability_subset</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PORTABILITY_SUBSET_FEATURES_KHR                     = 1000163000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_portability_subset</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PORTABILITY_SUBSET_PROPERTIES_KHR                   = 1000163001,
#endif

    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_SHADING_RATE_IMAGE_STATE_CREATE_INFO_NV           = 1000164000,

    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADING_RATE_IMAGE_FEATURES_NV                      = 1000164001,

    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADING_RATE_IMAGE_PROPERTIES_NV                    = 1000164002,

    /// <remarks>Provided by VK_NV_shading_rate_image</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_COARSE_SAMPLE_ORDER_STATE_CREATE_INFO_NV          = 1000164005,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_RAY_TRACING_PIPELINE_CREATE_INFO_NV                                 = 1000165000,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_CREATE_INFO_NV                               = 1000165001,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_GEOMETRY_NV                                                         = 1000165003,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_GEOMETRY_TRIANGLES_NV                                               = 1000165004,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_GEOMETRY_AABB_NV                                                    = 1000165005,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_BIND_ACCELERATION_STRUCTURE_MEMORY_INFO_NV                          = 1000165006,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET_ACCELERATION_STRUCTURE_NV                      = 1000165007,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_INFO_NV                  = 1000165008,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_PROPERTIES_NV                           = 1000165009,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_RAY_TRACING_SHADER_GROUP_CREATE_INFO_NV                             = 1000165011,

    /// <remarks>Provided by VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_INFO_NV                                      = 1000165012,

    /// <remarks>Provided by VK_NV_representative_fragment_test</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_REPRESENTATIVE_FRAGMENT_TEST_FEATURES_NV            = 1000166000,

    /// <remarks>Provided by VK_NV_representative_fragment_test</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_REPRESENTATIVE_FRAGMENT_TEST_STATE_CREATE_INFO_NV          = 1000166001,

    /// <remarks>Provided by VK_EXT_filter_cubic</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_VIEW_IMAGE_FORMAT_INFO_EXT                    = 1000170000,

    /// <remarks>Provided by VK_EXT_filter_cubic</remarks>
    VK_STRUCTURE_TYPE_FILTER_CUBIC_IMAGE_VIEW_IMAGE_FORMAT_PROPERTIES_EXT                 = 1000170001,

    /// <remarks>Provided by VK_EXT_external_memory_host</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_HOST_POINTER_INFO_EXT                                 = 1000178000,

    /// <remarks>Provided by VK_EXT_external_memory_host</remarks>
    VK_STRUCTURE_TYPE_MEMORY_HOST_POINTER_PROPERTIES_EXT                                  = 1000178001,

    /// <remarks>Provided by VK_EXT_external_memory_host</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_MEMORY_HOST_PROPERTIES_EXT                 = 1000178002,

    /// <remarks>Provided by VK_KHR_shader_clock</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CLOCK_FEATURES_KHR                           = 1000181000,

    /// <remarks>Provided by VK_AMD_pipeline_compiler_control</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COMPILER_CONTROL_CREATE_INFO_AMD                           = 1000183000,

    /// <remarks>Provided by VK_EXT_calibrated_timestamps</remarks>
    VK_STRUCTURE_TYPE_CALIBRATED_TIMESTAMP_INFO_EXT                                       = 1000184000,

    /// <remarks>Provided by VK_AMD_shader_core_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CORE_PROPERTIES_AMD                          = 1000185000,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_CAPABILITIES_KHR                                  = 1000187000,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_SESSION_PARAMETERS_CREATE_INFO_KHR                = 1000187001,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_SESSION_PARAMETERS_ADD_INFO_KHR                   = 1000187002,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_PROFILE_INFO_KHR                                  = 1000187003,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_PICTURE_INFO_KHR                                  = 1000187004,

    /// <remarks>Provided by VK_KHR_video_decode_h265</remarks>
    VK_STRUCTURE_TYPE_VIDEO_DECODE_H265_DPB_SLOT_INFO_KHR                                 = 1000187005,

    /// <remarks>Provided by VK_KHR_global_priority</remarks>
    VK_STRUCTURE_TYPE_DEVICE_QUEUE_GLOBAL_PRIORITY_CREATE_INFO_KHR                        = 1000174000,

    /// <remarks>Provided by VK_KHR_global_priority</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GLOBAL_PRIORITY_QUERY_FEATURES_KHR                  = 1000388000,

    /// <remarks>Provided by VK_KHR_global_priority</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_GLOBAL_PRIORITY_PROPERTIES_KHR                         = 1000388001,

    /// <remarks>Provided by VK_AMD_memory_overallocation_behavior</remarks>
    VK_STRUCTURE_TYPE_DEVICE_MEMORY_OVERALLOCATION_CREATE_INFO_AMD                        = 1000189000,

    /// <remarks>Provided by VK_EXT_vertex_attribute_divisor</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VERTEX_ATTRIBUTE_DIVISOR_PROPERTIES_EXT             = 1000190000,

    /// <remarks>Provided by VK_EXT_vertex_attribute_divisor</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_DIVISOR_STATE_CREATE_INFO_EXT                 = 1000190001,

    /// <remarks>Provided by VK_EXT_vertex_attribute_divisor</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VERTEX_ATTRIBUTE_DIVISOR_FEATURES_EXT               = 1000190002,

    /// <remarks>Provided by VK_GGP_frame_token</remarks>
    VK_STRUCTURE_TYPE_PRESENT_FRAME_TOKEN_GGP                                             = 1000191000,

    /// <remarks>Provided by VK_NV_compute_shader_derivatives</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COMPUTE_SHADER_DERIVATIVES_FEATURES_NV              = 1000201000,

    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MESH_SHADER_FEATURES_NV                             = 1000202000,

    /// <remarks>Provided by VK_NV_mesh_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MESH_SHADER_PROPERTIES_NV                           = 1000202001,

    /// <remarks>Provided by VK_NV_shader_image_footprint</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_IMAGE_FOOTPRINT_FEATURES_NV                  = 1000204000,

    /// <remarks>Provided by VK_NV_scissor_exclusive</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_EXCLUSIVE_SCISSOR_STATE_CREATE_INFO_NV            = 1000205000,

    /// <remarks>Provided by VK_NV_scissor_exclusive</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXCLUSIVE_SCISSOR_FEATURES_NV                       = 1000205002,

    /// <remarks>Provided by VK_NV_device_diagnostic_checkpoints</remarks>
    VK_STRUCTURE_TYPE_CHECKPOINT_DATA_NV                                                  = 1000206000,

    /// <remarks>Provided by VK_NV_device_diagnostic_checkpoints</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_CHECKPOINT_PROPERTIES_NV                               = 1000206001,

    /// <remarks>Provided by VK_INTEL_shader_integer_functions2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_FUNCTIONS_2_FEATURES_INTEL           = 1000209000,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_QUERY_POOL_PERFORMANCE_QUERY_CREATE_INFO_INTEL                      = 1000210000,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_INITIALIZE_PERFORMANCE_API_INFO_INTEL                               = 1000210001,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_MARKER_INFO_INTEL                                       = 1000210002,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_STREAM_MARKER_INFO_INTEL                                = 1000210003,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_OVERRIDE_INFO_INTEL                                     = 1000210004,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_PERFORMANCE_CONFIGURATION_ACQUIRE_INFO_INTEL                        = 1000210005,

    /// <remarks>Provided by VK_EXT_pci_bus_info</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PCI_BUS_INFO_PROPERTIES_EXT                         = 1000212000,

    /// <remarks>Provided by VK_AMD_display_native_hdr</remarks>
    VK_STRUCTURE_TYPE_DISPLAY_NATIVE_HDR_SURFACE_CAPABILITIES_AMD                         = 1000213000,

    /// <remarks>Provided by VK_AMD_display_native_hdr</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_DISPLAY_NATIVE_HDR_CREATE_INFO_AMD                        = 1000213001,

    /// <remarks>Provided by VK_FUCHSIA_imagepipe_surface</remarks>
    VK_STRUCTURE_TYPE_IMAGEPIPE_SURFACE_CREATE_INFO_FUCHSIA                               = 1000214000,

    /// <remarks>Provided by VK_EXT_metal_surface</remarks>
    VK_STRUCTURE_TYPE_METAL_SURFACE_CREATE_INFO_EXT                                       = 1000217000,

    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_FEATURES_EXT                   = 1000218000,

    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_PROPERTIES_EXT                 = 1000218001,

    /// <remarks>Provided by VK_EXT_fragment_density_map</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_FRAGMENT_DENSITY_MAP_CREATE_INFO_EXT                    = 1000218002,

    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_FRAGMENT_SHADING_RATE_ATTACHMENT_INFO_KHR                           = 1000226000,

    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_FRAGMENT_SHADING_RATE_STATE_CREATE_INFO_KHR                = 1000226001,

    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADING_RATE_PROPERTIES_KHR                = 1000226002,

    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADING_RATE_FEATURES_KHR                  = 1000226003,

    /// <remarks>Provided by VK_KHR_fragment_shading_rate</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADING_RATE_KHR                           = 1000226004,

    /// <remarks>Provided by VK_AMD_shader_core_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CORE_PROPERTIES_2_AMD                        = 1000227000,

    /// <remarks>Provided by VK_AMD_device_coherent_memory</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COHERENT_MEMORY_FEATURES_AMD                        = 1000229000,

    /// <remarks>Provided by VK_EXT_shader_image_atomic_int64</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_IMAGE_ATOMIC_INT64_FEATURES_EXT              = 1000234000,

    /// <remarks>Provided by VK_EXT_memory_budget</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_BUDGET_PROPERTIES_EXT                        = 1000237000,

    /// <remarks>Provided by VK_EXT_memory_priority</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_PRIORITY_FEATURES_EXT                        = 1000238000,

    /// <remarks>Provided by VK_EXT_memory_priority</remarks>
    VK_STRUCTURE_TYPE_MEMORY_PRIORITY_ALLOCATE_INFO_EXT                                   = 1000238001,

    /// <remarks>Provided by VK_KHR_surface_protected_capabilities</remarks>
    VK_STRUCTURE_TYPE_SURFACE_PROTECTED_CAPABILITIES_KHR                                  = 1000239000,

    /// <remarks>Provided by VK_NV_dedicated_allocation_image_aliasing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEDICATED_ALLOCATION_IMAGE_ALIASING_FEATURES_NV     = 1000240000,

    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_DEVICE_ADDRESS_FEATURES_EXT                  = 1000244000,

    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_CREATE_INFO_EXT                               = 1000244002,

    /// <remarks>Provided by VK_EXT_validation_features</remarks>
    VK_STRUCTURE_TYPE_VALIDATION_FEATURES_EXT                                             = 1000247000,

    /// <remarks>Provided by VK_KHR_present_wait</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRESENT_WAIT_FEATURES_KHR                           = 1000248000,

    /// <remarks>Provided by VK_NV_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COOPERATIVE_MATRIX_FEATURES_NV                      = 1000249000,

    /// <remarks>Provided by VK_NV_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_COOPERATIVE_MATRIX_PROPERTIES_NV                                    = 1000249001,

    /// <remarks>Provided by VK_NV_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COOPERATIVE_MATRIX_PROPERTIES_NV                    = 1000249002,

    /// <remarks>Provided by VK_NV_coverage_reduction_mode</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COVERAGE_REDUCTION_MODE_FEATURES_NV                 = 1000250000,

    /// <remarks>Provided by VK_NV_coverage_reduction_mode</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COVERAGE_REDUCTION_STATE_CREATE_INFO_NV                    = 1000250001,

    /// <remarks>Provided by VK_NV_coverage_reduction_mode</remarks>
    VK_STRUCTURE_TYPE_FRAMEBUFFER_MIXED_SAMPLES_COMBINATION_NV                            = 1000250002,

    /// <remarks>Provided by VK_EXT_fragment_shader_interlock</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADER_INTERLOCK_FEATURES_EXT              = 1000251000,

    /// <remarks>Provided by VK_EXT_ycbcr_image_arrays</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_YCBCR_IMAGE_ARRAYS_FEATURES_EXT                     = 1000252000,

    /// <remarks>Provided by VK_EXT_provoking_vertex</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROVOKING_VERTEX_FEATURES_EXT                       = 1000254000,

    /// <remarks>Provided by VK_EXT_provoking_vertex</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_PROVOKING_VERTEX_STATE_CREATE_INFO_EXT       = 1000254001,

    /// <remarks>Provided by VK_EXT_provoking_vertex</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROVOKING_VERTEX_PROPERTIES_EXT                     = 1000254002,

    /// <remarks>Provided by VK_EXT_full_screen_exclusive</remarks>
    VK_STRUCTURE_TYPE_SURFACE_FULL_SCREEN_EXCLUSIVE_INFO_EXT                              = 1000255000,

    /// <remarks>Provided by VK_EXT_full_screen_exclusive</remarks>
    VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_FULL_SCREEN_EXCLUSIVE_EXT                      = 1000255002,

    /// <remarks>Provided by VK_KHR_win32_surface with VK_EXT_full_screen_exclusive</remarks>
    VK_STRUCTURE_TYPE_SURFACE_FULL_SCREEN_EXCLUSIVE_WIN32_INFO_EXT                        = 1000255001,

    /// <remarks>Provided by VK_EXT_headless_surface</remarks>
    VK_STRUCTURE_TYPE_HEADLESS_SURFACE_CREATE_INFO_EXT                                    = 1000256000,

    /// <remarks>Provided by VK_EXT_line_rasterization</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_LINE_RASTERIZATION_FEATURES_EXT                     = 1000259000,

    /// <remarks>Provided by VK_EXT_line_rasterization</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_LINE_STATE_CREATE_INFO_EXT                   = 1000259001,

    /// <remarks>Provided by VK_EXT_line_rasterization</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_LINE_RASTERIZATION_PROPERTIES_EXT                   = 1000259002,

    /// <remarks>Provided by VK_EXT_shader_atomic_float</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ATOMIC_FLOAT_FEATURES_EXT                    = 1000260000,

    /// <remarks>Provided by VK_EXT_index_type_uint8</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INDEX_TYPE_UINT8_FEATURES_EXT                       = 1000265000,

    /// <remarks>Provided by VK_EXT_extended_dynamic_state</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTENDED_DYNAMIC_STATE_FEATURES_EXT                 = 1000267000,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_EXECUTABLE_PROPERTIES_FEATURES_KHR         = 1000269000,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_INFO_KHR                                                   = 1000269001,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_EXECUTABLE_PROPERTIES_KHR                                  = 1000269002,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_EXECUTABLE_INFO_KHR                                        = 1000269003,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_EXECUTABLE_STATISTIC_KHR                                   = 1000269004,

    /// <remarks>Provided by VK_KHR_pipeline_executable_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_EXECUTABLE_INTERNAL_REPRESENTATION_KHR                     = 1000269005,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_HOST_IMAGE_COPY_FEATURES_EXT                        = 1000270000,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_HOST_IMAGE_COPY_PROPERTIES_EXT                      = 1000270001,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_MEMORY_TO_IMAGE_COPY_EXT                                            = 1000270002,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_IMAGE_TO_MEMORY_COPY_EXT                                            = 1000270003,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_TO_MEMORY_INFO_EXT                                       = 1000270004,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_COPY_MEMORY_TO_IMAGE_INFO_EXT                                       = 1000270005,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_HOST_IMAGE_LAYOUT_TRANSITION_INFO_EXT                               = 1000270006,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_TO_IMAGE_INFO_EXT                                        = 1000270007,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_SUBRESOURCE_HOST_MEMCPY_SIZE_EXT                                    = 1000270008,

    /// <remarks>Provided by VK_EXT_host_image_copy</remarks>
    VK_STRUCTURE_TYPE_HOST_IMAGE_COPY_DEVICE_PERFORMANCE_QUERY_EXT                        = 1000270009,

    /// <remarks>Provided by VK_KHR_map_memory2</remarks>
    VK_STRUCTURE_TYPE_MEMORY_MAP_INFO_KHR                                                 = 1000271000,

    /// <remarks>Provided by VK_KHR_map_memory2</remarks>
    VK_STRUCTURE_TYPE_MEMORY_UNMAP_INFO_KHR                                               = 1000271001,

    /// <remarks>Provided by VK_EXT_shader_atomic_float2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ATOMIC_FLOAT_2_FEATURES_EXT                  = 1000273000,

    /// <remarks>Provided by VK_EXT_surface_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SURFACE_PRESENT_MODE_EXT                                            = 1000274000,

    /// <remarks>Provided by VK_EXT_surface_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SURFACE_PRESENT_SCALING_CAPABILITIES_EXT                            = 1000274001,

    /// <remarks>Provided by VK_EXT_surface_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SURFACE_PRESENT_MODE_COMPATIBILITY_EXT                              = 1000274002,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SWAPCHAIN_MAINTENANCE_1_FEATURES_EXT                = 1000275000,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_PRESENT_FENCE_INFO_EXT                                    = 1000275001,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_PRESENT_MODES_CREATE_INFO_EXT                             = 1000275002,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_PRESENT_MODE_INFO_EXT                                     = 1000275003,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_PRESENT_SCALING_CREATE_INFO_EXT                           = 1000275004,

    /// <remarks>Provided by VK_EXT_swapchain_maintenance1</remarks>
    VK_STRUCTURE_TYPE_RELEASE_SWAPCHAIN_IMAGES_INFO_EXT                                   = 1000275005,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEVICE_GENERATED_COMMANDS_PROPERTIES_NV             = 1000277000,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_GRAPHICS_SHADER_GROUP_CREATE_INFO_NV                                = 1000277001,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_SHADER_GROUPS_CREATE_INFO_NV                      = 1000277002,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_INDIRECT_COMMANDS_LAYOUT_TOKEN_NV                                   = 1000277003,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_INDIRECT_COMMANDS_LAYOUT_CREATE_INFO_NV                             = 1000277004,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_GENERATED_COMMANDS_INFO_NV                                          = 1000277005,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_GENERATED_COMMANDS_MEMORY_REQUIREMENTS_INFO_NV                      = 1000277006,

    /// <remarks>Provided by VK_NV_device_generated_commands</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEVICE_GENERATED_COMMANDS_FEATURES_NV               = 1000277007,

    /// <remarks>Provided by VK_NV_inherited_viewport_scissor</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INHERITED_VIEWPORT_SCISSOR_FEATURES_NV              = 1000278000,

    /// <remarks>Provided by VK_NV_inherited_viewport_scissor</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_VIEWPORT_SCISSOR_INFO_NV                 = 1000278001,

    /// <remarks>Provided by VK_EXT_texel_buffer_alignment</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXEL_BUFFER_ALIGNMENT_FEATURES_EXT                 = 1000281000,

    /// <remarks>Provided by VK_QCOM_render_pass_transform</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_RENDER_PASS_TRANSFORM_INFO_QCOM          = 1000282000,

    /// <remarks>Provided by VK_QCOM_render_pass_transform</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_TRANSFORM_BEGIN_INFO_QCOM                               = 1000282001,

    /// <remarks>Provided by VK_EXT_depth_bias_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_BIAS_CONTROL_FEATURES_EXT                     = 1000283000,

    /// <remarks>Provided by VK_EXT_depth_bias_control</remarks>
    VK_STRUCTURE_TYPE_DEPTH_BIAS_INFO_EXT                                                 = 1000283001,

    /// <remarks>Provided by VK_EXT_depth_bias_control</remarks>
    VK_STRUCTURE_TYPE_DEPTH_BIAS_REPRESENTATION_INFO_EXT                                  = 1000283002,

    /// <remarks>Provided by VK_EXT_device_memory_report</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEVICE_MEMORY_REPORT_FEATURES_EXT                   = 1000284000,

    /// <remarks>Provided by VK_EXT_device_memory_report</remarks>
    VK_STRUCTURE_TYPE_DEVICE_DEVICE_MEMORY_REPORT_CREATE_INFO_EXT                         = 1000284001,

    /// <remarks>Provided by VK_EXT_device_memory_report</remarks>
    VK_STRUCTURE_TYPE_DEVICE_MEMORY_REPORT_CALLBACK_DATA_EXT                              = 1000284002,

    /// <remarks>Provided by VK_EXT_robustness2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ROBUSTNESS_2_FEATURES_EXT                           = 1000286000,

    /// <remarks>Provided by VK_EXT_robustness2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ROBUSTNESS_2_PROPERTIES_EXT                         = 1000286001,

    /// <remarks>Provided by VK_EXT_custom_border_color</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_CUSTOM_BORDER_COLOR_CREATE_INFO_EXT                         = 1000287000,

    /// <remarks>Provided by VK_EXT_custom_border_color</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CUSTOM_BORDER_COLOR_PROPERTIES_EXT                  = 1000287001,

    /// <remarks>Provided by VK_EXT_custom_border_color</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CUSTOM_BORDER_COLOR_FEATURES_EXT                    = 1000287002,

    /// <remarks>Provided by VK_KHR_pipeline_library</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_LIBRARY_CREATE_INFO_KHR                                    = 1000290000,

    /// <remarks>Provided by VK_NV_present_barrier</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRESENT_BARRIER_FEATURES_NV                         = 1000292000,

    /// <remarks>Provided by VK_NV_present_barrier</remarks>
    VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_PRESENT_BARRIER_NV                             = 1000292001,

    /// <remarks>Provided by VK_NV_present_barrier</remarks>
    VK_STRUCTURE_TYPE_SWAPCHAIN_PRESENT_BARRIER_CREATE_INFO_NV                            = 1000292002,

    /// <remarks>Provided by VK_KHR_present_id</remarks>
    VK_STRUCTURE_TYPE_PRESENT_ID_KHR                                                      = 1000294000,

    /// <remarks>Provided by VK_KHR_present_id</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRESENT_ID_FEATURES_KHR                             = 1000294001,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_INFO_KHR                                               = 1000299000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_RATE_CONTROL_INFO_KHR                                  = 1000299001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_RATE_CONTROL_LAYER_INFO_KHR                            = 1000299002,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_CAPABILITIES_KHR                                       = 1000299003,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_USAGE_INFO_KHR                                         = 1000299004,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_QUERY_POOL_VIDEO_ENCODE_FEEDBACK_CREATE_INFO_KHR                    = 1000299005,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VIDEO_ENCODE_QUALITY_LEVEL_INFO_KHR                 = 1000299006,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_QUALITY_LEVEL_PROPERTIES_KHR                           = 1000299007,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_QUALITY_LEVEL_INFO_KHR                                 = 1000299008,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_SESSION_PARAMETERS_GET_INFO_KHR                        = 1000299009,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_KHR_video_encode_queue</remarks>
    VK_STRUCTURE_TYPE_VIDEO_ENCODE_SESSION_PARAMETERS_FEEDBACK_INFO_KHR                   = 1000299010,
#endif

    /// <remarks>Provided by VK_NV_device_diagnostics_config</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DIAGNOSTICS_CONFIG_FEATURES_NV                      = 1000300000,

    /// <remarks>Provided by VK_NV_device_diagnostics_config</remarks>
    VK_STRUCTURE_TYPE_DEVICE_DIAGNOSTICS_CONFIG_CREATE_INFO_NV                            = 1000300001,

    /// <remarks>Provided by VK_NV_low_latency</remarks>
    VK_STRUCTURE_TYPE_QUERY_LOW_LATENCY_SUPPORT_NV                                        = 1000310000,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_OBJECT_CREATE_INFO_EXT                                 = 1000311000,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_OBJECTS_INFO_EXT                                       = 1000311001,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_DEVICE_INFO_EXT                                        = 1000311002,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_COMMAND_QUEUE_INFO_EXT                                 = 1000311003,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_BUFFER_INFO_EXT                                        = 1000311004,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_IMPORT_METAL_BUFFER_INFO_EXT                                        = 1000311005,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_TEXTURE_INFO_EXT                                       = 1000311006,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_IMPORT_METAL_TEXTURE_INFO_EXT                                       = 1000311007,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_IO_SURFACE_INFO_EXT                                    = 1000311008,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_IMPORT_METAL_IO_SURFACE_INFO_EXT                                    = 1000311009,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_EXPORT_METAL_SHARED_EVENT_INFO_EXT                                  = 1000311010,

    /// <remarks>Provided by VK_EXT_metal_objects</remarks>
    VK_STRUCTURE_TYPE_IMPORT_METAL_SHARED_EVENT_INFO_EXT                                  = 1000311011,

    /// <remarks>Provided by VK_KHR_synchronization2 with VK_NV_device_diagnostic_checkpoints</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_CHECKPOINT_PROPERTIES_2_NV                             = 1000314008,

    /// <remarks>Provided by VK_KHR_synchronization2 with VK_NV_device_diagnostic_checkpoints</remarks>
    VK_STRUCTURE_TYPE_CHECKPOINT_DATA_2_NV                                                = 1000314009,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_BUFFER_PROPERTIES_EXT                    = 1000316000,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_BUFFER_DENSITY_MAP_PROPERTIES_EXT        = 1000316001,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_BUFFER_FEATURES_EXT                      = 1000316002,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_ADDRESS_INFO_EXT                                         = 1000316003,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_GET_INFO_EXT                                             = 1000316004,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_BUFFER_CAPTURE_DESCRIPTOR_DATA_INFO_EXT                             = 1000316005,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_IMAGE_CAPTURE_DESCRIPTOR_DATA_INFO_EXT                              = 1000316006,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_CAPTURE_DESCRIPTOR_DATA_INFO_EXT                         = 1000316007,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_CAPTURE_DESCRIPTOR_DATA_INFO_EXT                            = 1000316008,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_OPAQUE_CAPTURE_DESCRIPTOR_DATA_CREATE_INFO_EXT                      = 1000316010,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_BUFFER_BINDING_INFO_EXT                                  = 1000316011,

    /// <remarks>Provided by VK_EXT_descriptor_buffer</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_BUFFER_BINDING_PUSH_DESCRIPTOR_BUFFER_HANDLE_EXT         = 1000316012,

    /// <remarks>Provided by VK_EXT_descriptor_buffer with VK_KHR_acceleration_structure or VK_NV_ray_tracing</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_CAPTURE_DESCRIPTOR_DATA_INFO_EXT             = 1000316009,

    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GRAPHICS_PIPELINE_LIBRARY_FEATURES_EXT              = 1000320000,

    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GRAPHICS_PIPELINE_LIBRARY_PROPERTIES_EXT            = 1000320001,

    /// <remarks>Provided by VK_EXT_graphics_pipeline_library</remarks>
    VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_LIBRARY_CREATE_INFO_EXT                           = 1000320002,

    /// <remarks>Provided by VK_AMD_shader_early_and_late_fragment_tests</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_EARLY_AND_LATE_FRAGMENT_TESTS_FEATURES_AMD   = 1000321000,

    /// <remarks>Provided by VK_KHR_fragment_shader_barycentric</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADER_BARYCENTRIC_FEATURES_KHR            = 1000203000,

    /// <remarks>Provided by VK_KHR_fragment_shader_barycentric</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADER_BARYCENTRIC_PROPERTIES_KHR          = 1000322000,

    /// <remarks>Provided by VK_KHR_shader_subgroup_uniform_control_flow</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SUBGROUP_UNIFORM_CONTROL_FLOW_FEATURES_KHR   = 1000323000,

    /// <remarks>Provided by VK_NV_fragment_shading_rate_enums</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADING_RATE_ENUMS_PROPERTIES_NV           = 1000326000,

    /// <remarks>Provided by VK_NV_fragment_shading_rate_enums</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADING_RATE_ENUMS_FEATURES_NV             = 1000326001,

    /// <remarks>Provided by VK_NV_fragment_shading_rate_enums</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_FRAGMENT_SHADING_RATE_ENUM_STATE_CREATE_INFO_NV            = 1000326002,

    /// <remarks>Provided by VK_NV_ray_tracing_motion_blur</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_GEOMETRY_MOTION_TRIANGLES_DATA_NV            = 1000327000,

    /// <remarks>Provided by VK_NV_ray_tracing_motion_blur</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_MOTION_BLUR_FEATURES_NV                 = 1000327001,

    /// <remarks>Provided by VK_NV_ray_tracing_motion_blur</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_MOTION_INFO_NV                               = 1000327002,

    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MESH_SHADER_FEATURES_EXT                            = 1000328000,

    /// <remarks>Provided by VK_EXT_mesh_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MESH_SHADER_PROPERTIES_EXT                          = 1000328001,

    /// <remarks>Provided by VK_EXT_ycbcr_2plane_444_formats</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_YCBCR_2_PLANE_444_FORMATS_FEATURES_EXT              = 1000330000,

    /// <remarks>Provided by VK_EXT_fragment_density_map2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_2_FEATURES_EXT                 = 1000332000,

    /// <remarks>Provided by VK_EXT_fragment_density_map2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_2_PROPERTIES_EXT               = 1000332001,

    /// <remarks>Provided by VK_QCOM_rotated_copy_commands</remarks>
    VK_STRUCTURE_TYPE_COPY_COMMAND_TRANSFORM_INFO_QCOM                                    = 1000333000,

    /// <remarks>Provided by VK_KHR_workgroup_memory_explicit_layout</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_WORKGROUP_MEMORY_EXPLICIT_LAYOUT_FEATURES_KHR       = 1000336000,

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_COMPRESSION_CONTROL_FEATURES_EXT              = 1000338000,

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_STRUCTURE_TYPE_IMAGE_COMPRESSION_CONTROL_EXT                                       = 1000338001,

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_STRUCTURE_TYPE_IMAGE_COMPRESSION_PROPERTIES_EXT                                    = 1000338004,

    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_layout</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ATTACHMENT_FEEDBACK_LOOP_LAYOUT_FEATURES_EXT        = 1000339000,

    /// <remarks>Provided by VK_EXT_4444_formats</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_4444_FORMATS_FEATURES_EXT                           = 1000340000,

    /// <remarks>Provided by VK_EXT_device_fault</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FAULT_FEATURES_EXT                                  = 1000341000,

    /// <remarks>Provided by VK_EXT_device_fault</remarks>
    VK_STRUCTURE_TYPE_DEVICE_FAULT_COUNTS_EXT                                             = 1000341001,

    /// <remarks>Provided by VK_EXT_device_fault</remarks>
    VK_STRUCTURE_TYPE_DEVICE_FAULT_INFO_EXT                                               = 1000341002,

    /// <remarks>Provided by VK_EXT_rgba10x6_formats</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RGBA10X6_FORMATS_FEATURES_EXT                       = 1000344000,

    /// <remarks>Provided by VK_EXT_directfb_surface</remarks>
    VK_STRUCTURE_TYPE_DIRECTFB_SURFACE_CREATE_INFO_EXT                                    = 1000346000,

    /// <remarks>Provided by VK_EXT_vertex_input_dynamic_state</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VERTEX_INPUT_DYNAMIC_STATE_FEATURES_EXT             = 1000352000,

    /// <remarks>Provided by VK_EXT_shader_object, VK_EXT_vertex_input_dynamic_state</remarks>
    VK_STRUCTURE_TYPE_VERTEX_INPUT_BINDING_DESCRIPTION_2_EXT                              = 1000352001,

    /// <remarks>Provided by VK_EXT_shader_object, VK_EXT_vertex_input_dynamic_state</remarks>
    VK_STRUCTURE_TYPE_VERTEX_INPUT_ATTRIBUTE_DESCRIPTION_2_EXT                            = 1000352002,

    /// <remarks>Provided by VK_EXT_physical_device_drm</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DRM_PROPERTIES_EXT                                  = 1000353000,

    /// <remarks>Provided by VK_EXT_device_address_binding_report</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ADDRESS_BINDING_REPORT_FEATURES_EXT                 = 1000354000,

    /// <remarks>Provided by VK_EXT_device_address_binding_report</remarks>
    VK_STRUCTURE_TYPE_DEVICE_ADDRESS_BINDING_CALLBACK_DATA_EXT                            = 1000354001,

    /// <remarks>Provided by VK_EXT_depth_clip_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_CLIP_CONTROL_FEATURES_EXT                     = 1000355000,

    /// <remarks>Provided by VK_EXT_depth_clip_control</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_DEPTH_CLIP_CONTROL_CREATE_INFO_EXT                = 1000355001,

    /// <remarks>Provided by VK_EXT_primitive_topology_list_restart</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRIMITIVE_TOPOLOGY_LIST_RESTART_FEATURES_EXT        = 1000356000,

    /// <remarks>Provided by VK_FUCHSIA_external_memory</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_ZIRCON_HANDLE_INFO_FUCHSIA                            = 1000364000,

    /// <remarks>Provided by VK_FUCHSIA_external_memory</remarks>
    VK_STRUCTURE_TYPE_MEMORY_ZIRCON_HANDLE_PROPERTIES_FUCHSIA                             = 1000364001,

    /// <remarks>Provided by VK_FUCHSIA_external_memory</remarks>
    VK_STRUCTURE_TYPE_MEMORY_GET_ZIRCON_HANDLE_INFO_FUCHSIA                               = 1000364002,

    /// <remarks>Provided by VK_FUCHSIA_external_semaphore</remarks>
    VK_STRUCTURE_TYPE_IMPORT_SEMAPHORE_ZIRCON_HANDLE_INFO_FUCHSIA                         = 1000365000,

    /// <remarks>Provided by VK_FUCHSIA_external_semaphore</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_GET_ZIRCON_HANDLE_INFO_FUCHSIA                            = 1000365001,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COLLECTION_CREATE_INFO_FUCHSIA                               = 1000366000,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_IMPORT_MEMORY_BUFFER_COLLECTION_FUCHSIA                             = 1000366001,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COLLECTION_IMAGE_CREATE_INFO_FUCHSIA                         = 1000366002,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COLLECTION_PROPERTIES_FUCHSIA                                = 1000366003,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_CONSTRAINTS_INFO_FUCHSIA                                     = 1000366004,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COLLECTION_BUFFER_CREATE_INFO_FUCHSIA                        = 1000366005,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_IMAGE_CONSTRAINTS_INFO_FUCHSIA                                      = 1000366006,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_IMAGE_FORMAT_CONSTRAINTS_INFO_FUCHSIA                               = 1000366007,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_SYSMEM_COLOR_SPACE_FUCHSIA                                          = 1000366008,

    /// <remarks>Provided by VK_FUCHSIA_buffer_collection</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COLLECTION_CONSTRAINTS_INFO_FUCHSIA                          = 1000366009,

    /// <remarks>Provided by VK_HUAWEI_subpass_shading</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_SHADING_PIPELINE_CREATE_INFO_HUAWEI                         = 1000369000,

    /// <remarks>Provided by VK_HUAWEI_subpass_shading</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBPASS_SHADING_FEATURES_HUAWEI                     = 1000369001,

    /// <remarks>Provided by VK_HUAWEI_subpass_shading</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBPASS_SHADING_PROPERTIES_HUAWEI                   = 1000369002,

    /// <remarks>Provided by VK_HUAWEI_invocation_mask</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INVOCATION_MASK_FEATURES_HUAWEI                     = 1000370000,

    /// <remarks>Provided by VK_NV_external_memory_rdma</remarks>
    VK_STRUCTURE_TYPE_MEMORY_GET_REMOTE_ADDRESS_INFO_NV                                   = 1000371000,

    /// <remarks>Provided by VK_NV_external_memory_rdma</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_MEMORY_RDMA_FEATURES_NV                    = 1000371001,

    /// <remarks>Provided by VK_EXT_pipeline_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_PROPERTIES_IDENTIFIER_EXT                                  = 1000372000,

    /// <remarks>Provided by VK_EXT_pipeline_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_PROPERTIES_FEATURES_EXT                    = 1000372001,

    /// <remarks>Provided by VK_EXT_multisampled_render_to_single_sampled</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTISAMPLED_RENDER_TO_SINGLE_SAMPLED_FEATURES_EXT  = 1000376000,

    /// <remarks>Provided by VK_EXT_multisampled_render_to_single_sampled</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_RESOLVE_PERFORMANCE_QUERY_EXT                               = 1000376001,

    /// <remarks>Provided by VK_EXT_multisampled_render_to_single_sampled</remarks>
    VK_STRUCTURE_TYPE_MULTISAMPLED_RENDER_TO_SINGLE_SAMPLED_INFO_EXT                      = 1000376002,

    /// <remarks>Provided by VK_EXT_extended_dynamic_state2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTENDED_DYNAMIC_STATE_2_FEATURES_EXT               = 1000377000,

    /// <remarks>Provided by VK_QNX_screen_surface</remarks>
    VK_STRUCTURE_TYPE_SCREEN_SURFACE_CREATE_INFO_QNX                                      = 1000378000,

    /// <remarks>Provided by VK_EXT_color_write_enable</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COLOR_WRITE_ENABLE_FEATURES_EXT                     = 1000381000,

    /// <remarks>Provided by VK_EXT_color_write_enable</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_COLOR_WRITE_CREATE_INFO_EXT                                = 1000381001,

    /// <remarks>Provided by VK_EXT_primitives_generated_query</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRIMITIVES_GENERATED_QUERY_FEATURES_EXT             = 1000382000,

    /// <remarks>Provided by VK_KHR_ray_tracing_maintenance1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_MAINTENANCE_1_FEATURES_KHR              = 1000386000,

    /// <remarks>Provided by VK_EXT_image_view_min_lod</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_VIEW_MIN_LOD_FEATURES_EXT                     = 1000391000,

    /// <remarks>Provided by VK_EXT_image_view_min_lod</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_MIN_LOD_CREATE_INFO_EXT                                  = 1000391001,

    /// <remarks>Provided by VK_EXT_multi_draw</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTI_DRAW_FEATURES_EXT                             = 1000392000,

    /// <remarks>Provided by VK_EXT_multi_draw</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTI_DRAW_PROPERTIES_EXT                           = 1000392001,

    /// <remarks>Provided by VK_EXT_image_2d_view_of_3d</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_2D_VIEW_OF_3D_FEATURES_EXT                    = 1000393000,

    /// <remarks>Provided by VK_EXT_shader_tile_image</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_TILE_IMAGE_FEATURES_EXT                      = 1000395000,

    /// <remarks>Provided by VK_EXT_shader_tile_image</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_TILE_IMAGE_PROPERTIES_EXT                    = 1000395001,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_MICROMAP_BUILD_INFO_EXT                                             = 1000396000,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_MICROMAP_VERSION_INFO_EXT                                           = 1000396001,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_COPY_MICROMAP_INFO_EXT                                              = 1000396002,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_COPY_MICROMAP_TO_MEMORY_INFO_EXT                                    = 1000396003,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_COPY_MEMORY_TO_MICROMAP_INFO_EXT                                    = 1000396004,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_OPACITY_MICROMAP_FEATURES_EXT                       = 1000396005,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_OPACITY_MICROMAP_PROPERTIES_EXT                     = 1000396006,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_MICROMAP_CREATE_INFO_EXT                                            = 1000396007,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_MICROMAP_BUILD_SIZES_INFO_EXT                                       = 1000396008,

    /// <remarks>Provided by VK_EXT_opacity_micromap</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_TRIANGLES_OPACITY_MICROMAP_EXT               = 1000396009,
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_NV_displacement_micromap</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DISPLACEMENT_MICROMAP_FEATURES_NV                   = 1000397000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_NV_displacement_micromap</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DISPLACEMENT_MICROMAP_PROPERTIES_NV                 = 1000397001,
#endif
#if VK_ENABLE_BETA_EXTENSIONS

    /// <remarks>Provided by VK_NV_displacement_micromap</remarks>
    VK_STRUCTURE_TYPE_ACCELERATION_STRUCTURE_TRIANGLES_DISPLACEMENT_MICROMAP_NV           = 1000397002,
#endif

    /// <remarks>Provided by VK_HUAWEI_cluster_culling_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CLUSTER_CULLING_SHADER_FEATURES_HUAWEI              = 1000404000,

    /// <remarks>Provided by VK_HUAWEI_cluster_culling_shader</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CLUSTER_CULLING_SHADER_PROPERTIES_HUAWEI            = 1000404001,

    /// <remarks>Provided by VK_EXT_border_color_swizzle</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BORDER_COLOR_SWIZZLE_FEATURES_EXT                   = 1000411000,

    /// <remarks>Provided by VK_EXT_border_color_swizzle</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_BORDER_COLOR_COMPONENT_MAPPING_CREATE_INFO_EXT              = 1000411001,

    /// <remarks>Provided by VK_EXT_pageable_device_local_memory</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PAGEABLE_DEVICE_LOCAL_MEMORY_FEATURES_EXT           = 1000412000,

    /// <remarks>Provided by VK_ARM_shader_core_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CORE_PROPERTIES_ARM                          = 1000415000,

    /// <remarks>Provided by VK_EXT_image_sliced_view_of_3d</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_SLICED_VIEW_OF_3D_FEATURES_EXT                = 1000418000,

    /// <remarks>Provided by VK_EXT_image_sliced_view_of_3d</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_SLICED_CREATE_INFO_EXT                                   = 1000418001,

    /// <remarks>Provided by VK_VALVE_descriptor_set_host_mapping</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_SET_HOST_MAPPING_FEATURES_VALVE          = 1000420000,

    /// <remarks>Provided by VK_VALVE_descriptor_set_host_mapping</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_BINDING_REFERENCE_VALVE                              = 1000420001,

    /// <remarks>Provided by VK_VALVE_descriptor_set_host_mapping</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_HOST_MAPPING_INFO_VALVE                       = 1000420002,

    /// <remarks>Provided by VK_EXT_depth_clamp_zero_one</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_CLAMP_ZERO_ONE_FEATURES_EXT                   = 1000421000,

    /// <remarks>Provided by VK_EXT_non_seamless_cube_map</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_NON_SEAMLESS_CUBE_MAP_FEATURES_EXT                  = 1000422000,

    /// <remarks>Provided by VK_QCOM_fragment_density_map_offset</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_OFFSET_FEATURES_QCOM           = 1000425000,

    /// <remarks>Provided by VK_QCOM_fragment_density_map_offset</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_DENSITY_MAP_OFFSET_PROPERTIES_QCOM         = 1000425001,

    /// <remarks>Provided by VK_QCOM_fragment_density_map_offset</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_FRAGMENT_DENSITY_MAP_OFFSET_END_INFO_QCOM                   = 1000425002,

    /// <remarks>Provided by VK_NV_copy_memory_indirect</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COPY_MEMORY_INDIRECT_FEATURES_NV                    = 1000426000,

    /// <remarks>Provided by VK_NV_copy_memory_indirect</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COPY_MEMORY_INDIRECT_PROPERTIES_NV                  = 1000426001,

    /// <remarks>Provided by VK_NV_memory_decompression</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_DECOMPRESSION_FEATURES_NV                    = 1000427000,

    /// <remarks>Provided by VK_NV_memory_decompression</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_DECOMPRESSION_PROPERTIES_NV                  = 1000427001,

    /// <remarks>Provided by VK_NV_device_generated_commands_compute</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEVICE_GENERATED_COMMANDS_COMPUTE_FEATURES_NV       = 1000428000,

    /// <remarks>Provided by VK_NV_device_generated_commands_compute</remarks>
    VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_INDIRECT_BUFFER_INFO_NV                            = 1000428001,

    /// <remarks>Provided by VK_NV_device_generated_commands_compute</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_INDIRECT_DEVICE_ADDRESS_INFO_NV                            = 1000428002,

    /// <remarks>Provided by VK_NV_linear_color_attachment</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_LINEAR_COLOR_ATTACHMENT_FEATURES_NV                 = 1000430000,

    /// <remarks>Provided by VK_EXT_image_compression_control_swapchain</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_COMPRESSION_CONTROL_SWAPCHAIN_FEATURES_EXT    = 1000437000,

    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_PROCESSING_FEATURES_QCOM                      = 1000440000,

    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_PROCESSING_PROPERTIES_QCOM                    = 1000440001,

    /// <remarks>Provided by VK_QCOM_image_processing</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_SAMPLE_WEIGHT_CREATE_INFO_QCOM                           = 1000440002,

    /// <remarks>Provided by VK_EXT_external_memory_acquire_unmodified</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_ACQUIRE_UNMODIFIED_EXT                              = 1000453000,

    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTENDED_DYNAMIC_STATE_3_FEATURES_EXT               = 1000455000,

    /// <remarks>Provided by VK_EXT_extended_dynamic_state3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTENDED_DYNAMIC_STATE_3_PROPERTIES_EXT             = 1000455001,

    /// <remarks>Provided by VK_EXT_subpass_merge_feedback</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBPASS_MERGE_FEEDBACK_FEATURES_EXT                 = 1000458000,

    /// <remarks>Provided by VK_EXT_subpass_merge_feedback</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_CREATION_CONTROL_EXT                                    = 1000458001,

    /// <remarks>Provided by VK_EXT_subpass_merge_feedback</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_CREATION_FEEDBACK_CREATE_INFO_EXT                       = 1000458002,

    /// <remarks>Provided by VK_EXT_subpass_merge_feedback</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_SUBPASS_FEEDBACK_CREATE_INFO_EXT                        = 1000458003,

    /// <remarks>Provided by VK_LUNARG_direct_driver_loading</remarks>
    VK_STRUCTURE_TYPE_DIRECT_DRIVER_LOADING_INFO_LUNARG                                   = 1000459000,

    /// <remarks>Provided by VK_LUNARG_direct_driver_loading</remarks>
    VK_STRUCTURE_TYPE_DIRECT_DRIVER_LOADING_LIST_LUNARG                                   = 1000459001,

    /// <remarks>Provided by VK_EXT_shader_module_identifier</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_MODULE_IDENTIFIER_FEATURES_EXT               = 1000462000,

    /// <remarks>Provided by VK_EXT_shader_module_identifier</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_MODULE_IDENTIFIER_PROPERTIES_EXT             = 1000462001,

    /// <remarks>Provided by VK_EXT_shader_module_identifier</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_MODULE_IDENTIFIER_CREATE_INFO_EXT             = 1000462002,

    /// <remarks>Provided by VK_EXT_shader_module_identifier</remarks>
    VK_STRUCTURE_TYPE_SHADER_MODULE_IDENTIFIER_EXT                                        = 1000462003,

    /// <remarks>Provided by VK_EXT_rasterization_order_attachment_access</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RASTERIZATION_ORDER_ATTACHMENT_ACCESS_FEATURES_EXT  = 1000342000,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_OPTICAL_FLOW_FEATURES_NV                            = 1000464000,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_OPTICAL_FLOW_PROPERTIES_NV                          = 1000464001,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_OPTICAL_FLOW_IMAGE_FORMAT_INFO_NV                                   = 1000464002,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_OPTICAL_FLOW_IMAGE_FORMAT_PROPERTIES_NV                             = 1000464003,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_OPTICAL_FLOW_SESSION_CREATE_INFO_NV                                 = 1000464004,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_OPTICAL_FLOW_EXECUTE_INFO_NV                                        = 1000464005,

    /// <remarks>Provided by VK_NV_optical_flow</remarks>
    VK_STRUCTURE_TYPE_OPTICAL_FLOW_SESSION_CREATE_PRIVATE_DATA_INFO_NV                    = 1000464010,

    /// <remarks>Provided by VK_EXT_legacy_dithering</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_LEGACY_DITHERING_FEATURES_EXT                       = 1000465000,

    /// <remarks>Provided by VK_EXT_pipeline_protected_access</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_PROTECTED_ACCESS_FEATURES_EXT              = 1000466000,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_5_FEATURES_KHR                          = 1000470000,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_5_PROPERTIES_KHR                        = 1000470001,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_RENDERING_AREA_INFO_KHR                                             = 1000470003,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_DEVICE_IMAGE_SUBRESOURCE_INFO_KHR                                   = 1000470004,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_SUBRESOURCE_LAYOUT_2_KHR                                            = 1000338002,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_IMAGE_SUBRESOURCE_2_KHR                                             = 1000338003,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_CREATE_FLAGS_2_CREATE_INFO_KHR                             = 1000470005,

    /// <remarks>Provided by VK_KHR_maintenance5</remarks>
    VK_STRUCTURE_TYPE_BUFFER_USAGE_FLAGS_2_CREATE_INFO_KHR                                = 1000470006,

    /// <remarks>Provided by VK_KHR_ray_tracing_position_fetch</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_POSITION_FETCH_FEATURES_KHR             = 1000481000,

    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_OBJECT_FEATURES_EXT                          = 1000482000,

    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_OBJECT_PROPERTIES_EXT                        = 1000482001,

    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_STRUCTURE_TYPE_SHADER_CREATE_INFO_EXT                                              = 1000482002,

    /// <remarks>Provided by VK_QCOM_tile_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TILE_PROPERTIES_FEATURES_QCOM                       = 1000484000,

    /// <remarks>Provided by VK_QCOM_tile_properties</remarks>
    VK_STRUCTURE_TYPE_TILE_PROPERTIES_QCOM                                                = 1000484001,

    /// <remarks>Provided by VK_SEC_amigo_profiling</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_AMIGO_PROFILING_FEATURES_SEC                        = 1000485000,

    /// <remarks>Provided by VK_SEC_amigo_profiling</remarks>
    VK_STRUCTURE_TYPE_AMIGO_PROFILING_SUBMIT_INFO_SEC                                     = 1000485001,

    /// <remarks>Provided by VK_QCOM_multiview_per_view_viewports</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PER_VIEW_VIEWPORTS_FEATURES_QCOM          = 1000488000,

    /// <remarks>Provided by VK_NV_ray_tracing_invocation_reorder</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_INVOCATION_REORDER_FEATURES_NV          = 1000490000,

    /// <remarks>Provided by VK_NV_ray_tracing_invocation_reorder</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RAY_TRACING_INVOCATION_REORDER_PROPERTIES_NV        = 1000490001,

    /// <remarks>Provided by VK_EXT_mutable_descriptor_type</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MUTABLE_DESCRIPTOR_TYPE_FEATURES_EXT                = 1000351000,

    /// <remarks>Provided by VK_EXT_mutable_descriptor_type</remarks>
    VK_STRUCTURE_TYPE_MUTABLE_DESCRIPTOR_TYPE_CREATE_INFO_EXT                             = 1000351002,

    /// <remarks>Provided by VK_ARM_shader_core_builtins</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CORE_BUILTINS_FEATURES_ARM                   = 1000497000,

    /// <remarks>Provided by VK_ARM_shader_core_builtins</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_CORE_BUILTINS_PROPERTIES_ARM                 = 1000497001,

    /// <remarks>Provided by VK_EXT_pipeline_library_group_handles</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_LIBRARY_GROUP_HANDLES_FEATURES_EXT         = 1000498000,

    /// <remarks>Provided by VK_EXT_dynamic_rendering_unused_attachments</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DYNAMIC_RENDERING_UNUSED_ATTACHMENTS_FEATURES_EXT   = 1000499000,

    /// <remarks>Provided by VK_KHR_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COOPERATIVE_MATRIX_FEATURES_KHR                     = 1000506000,

    /// <remarks>Provided by VK_KHR_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_COOPERATIVE_MATRIX_PROPERTIES_KHR                                   = 1000506001,

    /// <remarks>Provided by VK_KHR_cooperative_matrix</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_COOPERATIVE_MATRIX_PROPERTIES_KHR                   = 1000506002,

    /// <remarks>Provided by VK_QCOM_multiview_per_view_render_areas</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PER_VIEW_RENDER_AREAS_FEATURES_QCOM       = 1000510000,

    /// <remarks>Provided by VK_QCOM_multiview_per_view_render_areas</remarks>
    VK_STRUCTURE_TYPE_MULTIVIEW_PER_VIEW_RENDER_AREAS_RENDER_PASS_BEGIN_INFO_QCOM         = 1000510001,

    /// <remarks>Provided by VK_QCOM_image_processing2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_PROCESSING_2_FEATURES_QCOM                    = 1000518000,

    /// <remarks>Provided by VK_QCOM_image_processing2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_PROCESSING_2_PROPERTIES_QCOM                  = 1000518001,

    /// <remarks>Provided by VK_QCOM_image_processing2</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_BLOCK_MATCH_WINDOW_CREATE_INFO_QCOM                         = 1000518002,

    /// <remarks>Provided by VK_QCOM_filter_cubic_weights</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_CUBIC_WEIGHTS_CREATE_INFO_QCOM                              = 1000519000,

    /// <remarks>Provided by VK_QCOM_filter_cubic_weights</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CUBIC_WEIGHTS_FEATURES_QCOM                         = 1000519001,

    /// <remarks>Provided by VK_QCOM_filter_cubic_weights</remarks>
    VK_STRUCTURE_TYPE_BLIT_IMAGE_CUBIC_WEIGHTS_INFO_QCOM                                  = 1000519002,

    /// <remarks>Provided by VK_QCOM_ycbcr_degamma</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_YCBCR_DEGAMMA_FEATURES_QCOM                         = 1000520000,

    /// <remarks>Provided by VK_QCOM_ycbcr_degamma</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_YCBCR_DEGAMMA_CREATE_INFO_QCOM             = 1000520001,

    /// <remarks>Provided by VK_QCOM_filter_cubic_clamp</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_CUBIC_CLAMP_FEATURES_QCOM                           = 1000521000,

    /// <remarks>Provided by VK_EXT_attachment_feedback_loop_dynamic_state</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ATTACHMENT_FEEDBACK_LOOP_DYNAMIC_STATE_FEATURES_EXT = 1000524000,

    /// <remarks>Provided by VK_QNX_external_memory_screen_buffer</remarks>
    VK_STRUCTURE_TYPE_SCREEN_BUFFER_PROPERTIES_QNX                                        = 1000529000,

    /// <remarks>Provided by VK_QNX_external_memory_screen_buffer</remarks>
    VK_STRUCTURE_TYPE_SCREEN_BUFFER_FORMAT_PROPERTIES_QNX                                 = 1000529001,

    /// <remarks>Provided by VK_QNX_external_memory_screen_buffer</remarks>
    VK_STRUCTURE_TYPE_IMPORT_SCREEN_BUFFER_INFO_QNX                                       = 1000529002,

    /// <remarks>Provided by VK_QNX_external_memory_screen_buffer</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_FORMAT_QNX                                                 = 1000529003,

    /// <remarks>Provided by VK_QNX_external_memory_screen_buffer</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_MEMORY_SCREEN_BUFFER_FEATURES_QNX          = 1000529004,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTER_FEATURES                           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTERS_FEATURES,

    /// <remarks>Provided by VK_VERSION_1_1</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DRAW_PARAMETER_FEATURES                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DRAW_PARAMETERS_FEATURES,

    /// <remarks>Provided by VK_EXT_debug_report</remarks>
    VK_STRUCTURE_TYPE_DEBUG_REPORT_CREATE_INFO_EXT                                        = VK_STRUCTURE_TYPE_DEBUG_REPORT_CALLBACK_CREATE_INFO_EXT,

    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_STRUCTURE_TYPE_RENDERING_INFO_KHR                                                  = VK_STRUCTURE_TYPE_RENDERING_INFO,

    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_STRUCTURE_TYPE_RENDERING_ATTACHMENT_INFO_KHR                                       = VK_STRUCTURE_TYPE_RENDERING_ATTACHMENT_INFO,

    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_RENDERING_CREATE_INFO_KHR                                  = VK_STRUCTURE_TYPE_PIPELINE_RENDERING_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DYNAMIC_RENDERING_FEATURES_KHR                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DYNAMIC_RENDERING_FEATURES,

    /// <remarks>Provided by VK_KHR_dynamic_rendering</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_RENDERING_INFO_KHR                       = VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_RENDERING_INFO,

    /// <remarks>Provided by VK_KHR_dynamic_rendering with VK_NV_framebuffer_mixed_samples</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_SAMPLE_COUNT_INFO_NV                                     = VK_STRUCTURE_TYPE_ATTACHMENT_SAMPLE_COUNT_INFO_AMD,

    /// <remarks>Provided by VK_KHR_multiview</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_MULTIVIEW_CREATE_INFO_KHR                               = VK_STRUCTURE_TYPE_RENDER_PASS_MULTIVIEW_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_multiview</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_FEATURES_KHR                              = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_FEATURES,

    /// <remarks>Provided by VK_KHR_multiview</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PROPERTIES_KHR                            = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MULTIVIEW_PROPERTIES,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2_KHR                                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FEATURES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROPERTIES_2_KHR                                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_2_KHR                                             = VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_FORMAT_PROPERTIES_2_KHR                                       = VK_STRUCTURE_TYPE_IMAGE_FORMAT_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_FORMAT_INFO_2_KHR                             = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_FORMAT_INFO_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_PROPERTIES_2_KHR                                       = VK_STRUCTURE_TYPE_QUEUE_FAMILY_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_PROPERTIES_2_KHR                             = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MEMORY_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_SPARSE_IMAGE_FORMAT_PROPERTIES_2_KHR                                = VK_STRUCTURE_TYPE_SPARSE_IMAGE_FORMAT_PROPERTIES_2,

    /// <remarks>Provided by VK_KHR_get_physical_device_properties2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SPARSE_IMAGE_FORMAT_INFO_2_KHR                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SPARSE_IMAGE_FORMAT_INFO_2,

    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO_KHR                                      = VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_FLAGS_INFO,

    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_RENDER_PASS_BEGIN_INFO_KHR                             = VK_STRUCTURE_TYPE_DEVICE_GROUP_RENDER_PASS_BEGIN_INFO,

    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_COMMAND_BUFFER_BEGIN_INFO_KHR                          = VK_STRUCTURE_TYPE_DEVICE_GROUP_COMMAND_BUFFER_BEGIN_INFO,

    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_SUBMIT_INFO_KHR                                        = VK_STRUCTURE_TYPE_DEVICE_GROUP_SUBMIT_INFO,

    /// <remarks>Provided by VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_BIND_SPARSE_INFO_KHR                                   = VK_STRUCTURE_TYPE_DEVICE_GROUP_BIND_SPARSE_INFO,

    /// <remarks>Provided by VK_KHR_bind_memory2 with VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_DEVICE_GROUP_INFO_KHR                            = VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_DEVICE_GROUP_INFO,

    /// <remarks>Provided by VK_KHR_bind_memory2 with VK_KHR_device_group</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_DEVICE_GROUP_INFO_KHR                             = VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_DEVICE_GROUP_INFO,

    /// <remarks>Provided by VK_EXT_texture_compression_astc_hdr</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXTURE_COMPRESSION_ASTC_HDR_FEATURES_EXT           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXTURE_COMPRESSION_ASTC_HDR_FEATURES,

    /// <remarks>Provided by VK_KHR_device_group_creation</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GROUP_PROPERTIES_KHR                                = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GROUP_PROPERTIES,

    /// <remarks>Provided by VK_KHR_device_group_creation</remarks>
    VK_STRUCTURE_TYPE_DEVICE_GROUP_DEVICE_CREATE_INFO_KHR                                 = VK_STRUCTURE_TYPE_DEVICE_GROUP_DEVICE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_external_memory_capabilities</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_IMAGE_FORMAT_INFO_KHR                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_IMAGE_FORMAT_INFO,

    /// <remarks>Provided by VK_KHR_external_memory_capabilities</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_IMAGE_FORMAT_PROPERTIES_KHR                                = VK_STRUCTURE_TYPE_EXTERNAL_IMAGE_FORMAT_PROPERTIES,

    /// <remarks>Provided by VK_KHR_external_memory_capabilities</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_BUFFER_INFO_KHR                            = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_BUFFER_INFO,

    /// <remarks>Provided by VK_KHR_external_memory_capabilities</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_BUFFER_PROPERTIES_KHR                                      = VK_STRUCTURE_TYPE_EXTERNAL_BUFFER_PROPERTIES,

    /// <remarks>Provided by VK_KHR_external_fence_capabilities, VK_KHR_external_memory_capabilities, VK_KHR_external_semaphore_capabilities</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ID_PROPERTIES_KHR                                   = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ID_PROPERTIES,

    /// <remarks>Provided by VK_KHR_external_memory</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_BUFFER_CREATE_INFO_KHR                              = VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_BUFFER_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_external_memory</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_IMAGE_CREATE_INFO_KHR                               = VK_STRUCTURE_TYPE_EXTERNAL_MEMORY_IMAGE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_external_memory</remarks>
    VK_STRUCTURE_TYPE_EXPORT_MEMORY_ALLOCATE_INFO_KHR                                     = VK_STRUCTURE_TYPE_EXPORT_MEMORY_ALLOCATE_INFO,

    /// <remarks>Provided by VK_KHR_external_semaphore_capabilities</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_SEMAPHORE_INFO_KHR                         = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_SEMAPHORE_INFO,

    /// <remarks>Provided by VK_KHR_external_semaphore_capabilities</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_SEMAPHORE_PROPERTIES_KHR                                   = VK_STRUCTURE_TYPE_EXTERNAL_SEMAPHORE_PROPERTIES,

    /// <remarks>Provided by VK_KHR_external_semaphore</remarks>
    VK_STRUCTURE_TYPE_EXPORT_SEMAPHORE_CREATE_INFO_KHR                                    = VK_STRUCTURE_TYPE_EXPORT_SEMAPHORE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_shader_float16_int8</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_FLOAT16_INT8_FEATURES_KHR                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_FLOAT16_INT8_FEATURES,

    /// <remarks>Provided by VK_KHR_shader_float16_int8</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FLOAT16_INT8_FEATURES_KHR                           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_FLOAT16_INT8_FEATURES,

    /// <remarks>Provided by VK_KHR_16bit_storage</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES_KHR                          = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_16BIT_STORAGE_FEATURES,

    /// <remarks>Provided by VK_KHR_descriptor_update_template</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_UPDATE_TEMPLATE_CREATE_INFO_KHR                          = VK_STRUCTURE_TYPE_DESCRIPTOR_UPDATE_TEMPLATE_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_display_surface_counter</remarks>
    VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES2_EXT                                           = VK_STRUCTURE_TYPE_SURFACE_CAPABILITIES_2_EXT,

    /// <remarks>Provided by VK_KHR_imageless_framebuffer</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGELESS_FRAMEBUFFER_FEATURES_KHR                  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGELESS_FRAMEBUFFER_FEATURES,

    /// <remarks>Provided by VK_KHR_imageless_framebuffer</remarks>
    VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENTS_CREATE_INFO_KHR                             = VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENTS_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_imageless_framebuffer</remarks>
    VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENT_IMAGE_INFO_KHR                               = VK_STRUCTURE_TYPE_FRAMEBUFFER_ATTACHMENT_IMAGE_INFO,

    /// <remarks>Provided by VK_KHR_imageless_framebuffer</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_ATTACHMENT_BEGIN_INFO_KHR                               = VK_STRUCTURE_TYPE_RENDER_PASS_ATTACHMENT_BEGIN_INFO,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_2_KHR                                        = VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_2,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_2_KHR                                          = VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_2,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_2_KHR                                           = VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_2,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DEPENDENCY_2_KHR                                            = VK_STRUCTURE_TYPE_SUBPASS_DEPENDENCY_2,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO_2_KHR                                       = VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO_2,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_BEGIN_INFO_KHR                                              = VK_STRUCTURE_TYPE_SUBPASS_BEGIN_INFO,

    /// <remarks>Provided by VK_KHR_create_renderpass2</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_END_INFO_KHR                                                = VK_STRUCTURE_TYPE_SUBPASS_END_INFO,

    /// <remarks>Provided by VK_KHR_external_fence_capabilities</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_FENCE_INFO_KHR                             = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_EXTERNAL_FENCE_INFO,

    /// <remarks>Provided by VK_KHR_external_fence_capabilities</remarks>
    VK_STRUCTURE_TYPE_EXTERNAL_FENCE_PROPERTIES_KHR                                       = VK_STRUCTURE_TYPE_EXTERNAL_FENCE_PROPERTIES,

    /// <remarks>Provided by VK_KHR_external_fence</remarks>
    VK_STRUCTURE_TYPE_EXPORT_FENCE_CREATE_INFO_KHR                                        = VK_STRUCTURE_TYPE_EXPORT_FENCE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_POINT_CLIPPING_PROPERTIES_KHR                       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_POINT_CLIPPING_PROPERTIES,

    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_STRUCTURE_TYPE_RENDER_PASS_INPUT_ATTACHMENT_ASPECT_CREATE_INFO_KHR                 = VK_STRUCTURE_TYPE_RENDER_PASS_INPUT_ATTACHMENT_ASPECT_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_VIEW_USAGE_CREATE_INFO_KHR                                    = VK_STRUCTURE_TYPE_IMAGE_VIEW_USAGE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_maintenance2</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_DOMAIN_ORIGIN_STATE_CREATE_INFO_KHR           = VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_DOMAIN_ORIGIN_STATE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_variable_pointers</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTERS_FEATURES_KHR                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTERS_FEATURES,

    /// <remarks>Provided by VK_KHR_variable_pointers</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTER_FEATURES_KHR                       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VARIABLE_POINTERS_FEATURES,

    /// <remarks>Provided by VK_KHR_dedicated_allocation</remarks>
    VK_STRUCTURE_TYPE_MEMORY_DEDICATED_REQUIREMENTS_KHR                                   = VK_STRUCTURE_TYPE_MEMORY_DEDICATED_REQUIREMENTS,

    /// <remarks>Provided by VK_KHR_dedicated_allocation</remarks>
    VK_STRUCTURE_TYPE_MEMORY_DEDICATED_ALLOCATE_INFO_KHR                                  = VK_STRUCTURE_TYPE_MEMORY_DEDICATED_ALLOCATE_INFO,

    /// <remarks>Provided by VK_EXT_sampler_filter_minmax</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_FILTER_MINMAX_PROPERTIES_EXT                = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_FILTER_MINMAX_PROPERTIES,

    /// <remarks>Provided by VK_EXT_sampler_filter_minmax</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_REDUCTION_MODE_CREATE_INFO_EXT                              = VK_STRUCTURE_TYPE_SAMPLER_REDUCTION_MODE_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_inline_uniform_block</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_FEATURES_EXT                   = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_FEATURES,

    /// <remarks>Provided by VK_EXT_inline_uniform_block</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_PROPERTIES_EXT                 = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_INLINE_UNIFORM_BLOCK_PROPERTIES,

    /// <remarks>Provided by VK_EXT_inline_uniform_block</remarks>
    VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET_INLINE_UNIFORM_BLOCK_EXT                       = VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET_INLINE_UNIFORM_BLOCK,

    /// <remarks>Provided by VK_EXT_inline_uniform_block</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_INLINE_UNIFORM_BLOCK_CREATE_INFO_EXT                = VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_INLINE_UNIFORM_BLOCK_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_get_memory_requirements2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_MEMORY_REQUIREMENTS_INFO_2_KHR                               = VK_STRUCTURE_TYPE_BUFFER_MEMORY_REQUIREMENTS_INFO_2,

    /// <remarks>Provided by VK_KHR_get_memory_requirements2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_MEMORY_REQUIREMENTS_INFO_2_KHR                                = VK_STRUCTURE_TYPE_IMAGE_MEMORY_REQUIREMENTS_INFO_2,

    /// <remarks>Provided by VK_KHR_get_memory_requirements2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_SPARSE_MEMORY_REQUIREMENTS_INFO_2_KHR                         = VK_STRUCTURE_TYPE_IMAGE_SPARSE_MEMORY_REQUIREMENTS_INFO_2,

    /// <remarks>Provided by VK_KHR_get_memory_requirements2</remarks>
    VK_STRUCTURE_TYPE_MEMORY_REQUIREMENTS_2_KHR                                           = VK_STRUCTURE_TYPE_MEMORY_REQUIREMENTS_2,

    /// <remarks>Provided by VK_KHR_get_memory_requirements2</remarks>
    VK_STRUCTURE_TYPE_SPARSE_IMAGE_MEMORY_REQUIREMENTS_2_KHR                              = VK_STRUCTURE_TYPE_SPARSE_IMAGE_MEMORY_REQUIREMENTS_2,

    /// <remarks>Provided by VK_KHR_image_format_list</remarks>
    VK_STRUCTURE_TYPE_IMAGE_FORMAT_LIST_CREATE_INFO_KHR                                   = VK_STRUCTURE_TYPE_IMAGE_FORMAT_LIST_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_CREATE_INFO_KHR                            = VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_INFO_KHR                                   = VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_INFO,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_PLANE_MEMORY_INFO_KHR                                    = VK_STRUCTURE_TYPE_BIND_IMAGE_PLANE_MEMORY_INFO,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_IMAGE_PLANE_MEMORY_REQUIREMENTS_INFO_KHR                            = VK_STRUCTURE_TYPE_IMAGE_PLANE_MEMORY_REQUIREMENTS_INFO,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_YCBCR_CONVERSION_FEATURES_KHR               = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SAMPLER_YCBCR_CONVERSION_FEATURES,

    /// <remarks>Provided by VK_KHR_sampler_ycbcr_conversion</remarks>
    VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_IMAGE_FORMAT_PROPERTIES_KHR                = VK_STRUCTURE_TYPE_SAMPLER_YCBCR_CONVERSION_IMAGE_FORMAT_PROPERTIES,

    /// <remarks>Provided by VK_KHR_bind_memory2</remarks>
    VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_INFO_KHR                                         = VK_STRUCTURE_TYPE_BIND_BUFFER_MEMORY_INFO,

    /// <remarks>Provided by VK_KHR_bind_memory2</remarks>
    VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_INFO_KHR                                          = VK_STRUCTURE_TYPE_BIND_IMAGE_MEMORY_INFO,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_BINDING_FLAGS_CREATE_INFO_EXT                 = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_BINDING_FLAGS_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_FEATURES_EXT                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_FEATURES,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_PROPERTIES_EXT                  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DESCRIPTOR_INDEXING_PROPERTIES,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_ALLOCATE_INFO_EXT          = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_ALLOCATE_INFO,

    /// <remarks>Provided by VK_EXT_descriptor_indexing</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_LAYOUT_SUPPORT_EXT         = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_VARIABLE_DESCRIPTOR_COUNT_LAYOUT_SUPPORT,

    /// <remarks>Provided by VK_KHR_maintenance3</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_3_PROPERTIES_KHR                        = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_3_PROPERTIES,

    /// <remarks>Provided by VK_KHR_maintenance3</remarks>
    VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_SUPPORT_KHR                                   = VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_SUPPORT,

    /// <remarks>Provided by VK_EXT_global_priority</remarks>
    VK_STRUCTURE_TYPE_DEVICE_QUEUE_GLOBAL_PRIORITY_CREATE_INFO_EXT                        = VK_STRUCTURE_TYPE_DEVICE_QUEUE_GLOBAL_PRIORITY_CREATE_INFO_KHR,

    /// <remarks>Provided by VK_KHR_shader_subgroup_extended_types</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SUBGROUP_EXTENDED_TYPES_FEATURES_KHR         = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_SUBGROUP_EXTENDED_TYPES_FEATURES,

    /// <remarks>Provided by VK_KHR_8bit_storage</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_8BIT_STORAGE_FEATURES_KHR                           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_8BIT_STORAGE_FEATURES,

    /// <remarks>Provided by VK_KHR_shader_atomic_int64</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ATOMIC_INT64_FEATURES_KHR                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_ATOMIC_INT64_FEATURES,

    /// <remarks>Provided by VK_EXT_pipeline_creation_feedback</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_CREATION_FEEDBACK_CREATE_INFO_EXT                          = VK_STRUCTURE_TYPE_PIPELINE_CREATION_FEEDBACK_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_driver_properties</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DRIVER_PROPERTIES_KHR                               = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DRIVER_PROPERTIES,

    /// <remarks>Provided by VK_KHR_shader_float_controls</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FLOAT_CONTROLS_PROPERTIES_KHR                       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FLOAT_CONTROLS_PROPERTIES,

    /// <remarks>Provided by VK_KHR_depth_stencil_resolve</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_STENCIL_RESOLVE_PROPERTIES_KHR                = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_DEPTH_STENCIL_RESOLVE_PROPERTIES,

    /// <remarks>Provided by VK_KHR_depth_stencil_resolve</remarks>
    VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_DEPTH_STENCIL_RESOLVE_KHR                       = VK_STRUCTURE_TYPE_SUBPASS_DESCRIPTION_DEPTH_STENCIL_RESOLVE,

    /// <remarks>Provided by VK_NV_fragment_shader_barycentric</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADER_BARYCENTRIC_FEATURES_NV             = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FRAGMENT_SHADER_BARYCENTRIC_FEATURES_KHR,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_FEATURES_KHR                     = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_FEATURES,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_PROPERTIES_KHR                   = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TIMELINE_SEMAPHORE_PROPERTIES,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_TYPE_CREATE_INFO_KHR                                      = VK_STRUCTURE_TYPE_SEMAPHORE_TYPE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_TIMELINE_SEMAPHORE_SUBMIT_INFO_KHR                                  = VK_STRUCTURE_TYPE_TIMELINE_SEMAPHORE_SUBMIT_INFO,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_WAIT_INFO_KHR                                             = VK_STRUCTURE_TYPE_SEMAPHORE_WAIT_INFO,

    /// <remarks>Provided by VK_KHR_timeline_semaphore</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_SIGNAL_INFO_KHR                                           = VK_STRUCTURE_TYPE_SEMAPHORE_SIGNAL_INFO,

    /// <remarks>Provided by VK_INTEL_performance_query</remarks>
    VK_STRUCTURE_TYPE_QUERY_POOL_CREATE_INFO_INTEL                                        = VK_STRUCTURE_TYPE_QUERY_POOL_PERFORMANCE_QUERY_CREATE_INFO_INTEL,

    /// <remarks>Provided by VK_KHR_vulkan_memory_model</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_MEMORY_MODEL_FEATURES_KHR                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_VULKAN_MEMORY_MODEL_FEATURES,

    /// <remarks>Provided by VK_KHR_shader_terminate_invocation</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_TERMINATE_INVOCATION_FEATURES_KHR            = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_TERMINATE_INVOCATION_FEATURES,

    /// <remarks>Provided by VK_EXT_scalar_block_layout</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SCALAR_BLOCK_LAYOUT_FEATURES_EXT                    = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SCALAR_BLOCK_LAYOUT_FEATURES,

    /// <remarks>Provided by VK_EXT_subgroup_size_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_PROPERTIES_EXT                = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_PROPERTIES,

    /// <remarks>Provided by VK_EXT_subgroup_size_control</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_REQUIRED_SUBGROUP_SIZE_CREATE_INFO_EXT        = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_REQUIRED_SUBGROUP_SIZE_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_subgroup_size_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_FEATURES_EXT                  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SUBGROUP_SIZE_CONTROL_FEATURES,

    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SEPARATE_DEPTH_STENCIL_LAYOUTS_FEATURES_KHR         = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SEPARATE_DEPTH_STENCIL_LAYOUTS_FEATURES,

    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_STENCIL_LAYOUT_KHR                             = VK_STRUCTURE_TYPE_ATTACHMENT_REFERENCE_STENCIL_LAYOUT,

    /// <remarks>Provided by VK_KHR_separate_depth_stencil_layouts</remarks>
    VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_STENCIL_LAYOUT_KHR                           = VK_STRUCTURE_TYPE_ATTACHMENT_DESCRIPTION_STENCIL_LAYOUT,

    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_ADDRESS_FEATURES_EXT                         = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_DEVICE_ADDRESS_FEATURES_EXT,

    /// <remarks>Provided by VK_EXT_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_INFO_EXT                                      = VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_INFO,

    /// <remarks>Provided by VK_EXT_tooling_info</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TOOL_PROPERTIES_EXT                                 = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TOOL_PROPERTIES,

    /// <remarks>Provided by VK_EXT_separate_stencil_usage</remarks>
    VK_STRUCTURE_TYPE_IMAGE_STENCIL_USAGE_CREATE_INFO_EXT                                 = VK_STRUCTURE_TYPE_IMAGE_STENCIL_USAGE_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_uniform_buffer_standard_layout</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_UNIFORM_BUFFER_STANDARD_LAYOUT_FEATURES_KHR         = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_UNIFORM_BUFFER_STANDARD_LAYOUT_FEATURES,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_DEVICE_ADDRESS_FEATURES_KHR                  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_BUFFER_DEVICE_ADDRESS_FEATURES,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_INFO_KHR                                      = VK_STRUCTURE_TYPE_BUFFER_DEVICE_ADDRESS_INFO,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_BUFFER_OPAQUE_CAPTURE_ADDRESS_CREATE_INFO_KHR                       = VK_STRUCTURE_TYPE_BUFFER_OPAQUE_CAPTURE_ADDRESS_CREATE_INFO,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_MEMORY_OPAQUE_CAPTURE_ADDRESS_ALLOCATE_INFO_KHR                     = VK_STRUCTURE_TYPE_MEMORY_OPAQUE_CAPTURE_ADDRESS_ALLOCATE_INFO,

    /// <remarks>Provided by VK_KHR_buffer_device_address</remarks>
    VK_STRUCTURE_TYPE_DEVICE_MEMORY_OPAQUE_CAPTURE_ADDRESS_INFO_KHR                       = VK_STRUCTURE_TYPE_DEVICE_MEMORY_OPAQUE_CAPTURE_ADDRESS_INFO,

    /// <remarks>Provided by VK_EXT_host_query_reset</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_HOST_QUERY_RESET_FEATURES_EXT                       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_HOST_QUERY_RESET_FEATURES,

    /// <remarks>Provided by VK_EXT_shader_demote_to_helper_invocation</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DEMOTE_TO_HELPER_INVOCATION_FEATURES_EXT     = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_DEMOTE_TO_HELPER_INVOCATION_FEATURES,

    /// <remarks>Provided by VK_KHR_shader_integer_dot_product</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_FEATURES_KHR             = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_FEATURES,

    /// <remarks>Provided by VK_KHR_shader_integer_dot_product</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_PROPERTIES_KHR           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SHADER_INTEGER_DOT_PRODUCT_PROPERTIES,

    /// <remarks>Provided by VK_EXT_texel_buffer_alignment</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXEL_BUFFER_ALIGNMENT_PROPERTIES_EXT               = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_TEXEL_BUFFER_ALIGNMENT_PROPERTIES,

    /// <remarks>Provided by VK_EXT_private_data</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRIVATE_DATA_FEATURES_EXT                           = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PRIVATE_DATA_FEATURES,

    /// <remarks>Provided by VK_EXT_private_data</remarks>
    VK_STRUCTURE_TYPE_DEVICE_PRIVATE_DATA_CREATE_INFO_EXT                                 = VK_STRUCTURE_TYPE_DEVICE_PRIVATE_DATA_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_private_data</remarks>
    VK_STRUCTURE_TYPE_PRIVATE_DATA_SLOT_CREATE_INFO_EXT                                   = VK_STRUCTURE_TYPE_PRIVATE_DATA_SLOT_CREATE_INFO,

    /// <remarks>Provided by VK_EXT_pipeline_creation_cache_control</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_CREATION_CACHE_CONTROL_FEATURES_EXT        = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_PIPELINE_CREATION_CACHE_CONTROL_FEATURES,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_MEMORY_BARRIER_2_KHR                                                = VK_STRUCTURE_TYPE_MEMORY_BARRIER_2,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER_2_KHR                                         = VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER_2,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER_2_KHR                                          = VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER_2,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_DEPENDENCY_INFO_KHR                                                 = VK_STRUCTURE_TYPE_DEPENDENCY_INFO,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_SUBMIT_INFO_2_KHR                                                   = VK_STRUCTURE_TYPE_SUBMIT_INFO_2,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_SEMAPHORE_SUBMIT_INFO_KHR                                           = VK_STRUCTURE_TYPE_SEMAPHORE_SUBMIT_INFO,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_COMMAND_BUFFER_SUBMIT_INFO_KHR                                      = VK_STRUCTURE_TYPE_COMMAND_BUFFER_SUBMIT_INFO,

    /// <remarks>Provided by VK_KHR_synchronization2</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SYNCHRONIZATION_2_FEATURES_KHR                      = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_SYNCHRONIZATION_2_FEATURES,

    /// <remarks>Provided by VK_KHR_zero_initialize_workgroup_memory</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ZERO_INITIALIZE_WORKGROUP_MEMORY_FEATURES_KHR       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_ZERO_INITIALIZE_WORKGROUP_MEMORY_FEATURES,

    /// <remarks>Provided by VK_EXT_image_robustness</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_ROBUSTNESS_FEATURES_EXT                       = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_IMAGE_ROBUSTNESS_FEATURES,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_COPY_BUFFER_INFO_2_KHR                                              = VK_STRUCTURE_TYPE_COPY_BUFFER_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_INFO_2_KHR                                               = VK_STRUCTURE_TYPE_COPY_IMAGE_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_COPY_BUFFER_TO_IMAGE_INFO_2_KHR                                     = VK_STRUCTURE_TYPE_COPY_BUFFER_TO_IMAGE_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_COPY_IMAGE_TO_BUFFER_INFO_2_KHR                                     = VK_STRUCTURE_TYPE_COPY_IMAGE_TO_BUFFER_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_BLIT_IMAGE_INFO_2_KHR                                               = VK_STRUCTURE_TYPE_BLIT_IMAGE_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_RESOLVE_IMAGE_INFO_2_KHR                                            = VK_STRUCTURE_TYPE_RESOLVE_IMAGE_INFO_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_COPY_2_KHR                                                   = VK_STRUCTURE_TYPE_BUFFER_COPY_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_COPY_2_KHR                                                    = VK_STRUCTURE_TYPE_IMAGE_COPY_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_BLIT_2_KHR                                                    = VK_STRUCTURE_TYPE_IMAGE_BLIT_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_BUFFER_IMAGE_COPY_2_KHR                                             = VK_STRUCTURE_TYPE_BUFFER_IMAGE_COPY_2,

    /// <remarks>Provided by VK_KHR_copy_commands2</remarks>
    VK_STRUCTURE_TYPE_IMAGE_RESOLVE_2_KHR                                                 = VK_STRUCTURE_TYPE_IMAGE_RESOLVE_2,

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_STRUCTURE_TYPE_SUBRESOURCE_LAYOUT_2_EXT                                            = VK_STRUCTURE_TYPE_SUBRESOURCE_LAYOUT_2_KHR,

    /// <remarks>Provided by VK_EXT_image_compression_control</remarks>
    VK_STRUCTURE_TYPE_IMAGE_SUBRESOURCE_2_EXT                                             = VK_STRUCTURE_TYPE_IMAGE_SUBRESOURCE_2_KHR,

    /// <remarks>Provided by VK_ARM_rasterization_order_attachment_access</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RASTERIZATION_ORDER_ATTACHMENT_ACCESS_FEATURES_ARM  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_RASTERIZATION_ORDER_ATTACHMENT_ACCESS_FEATURES_EXT,

    /// <remarks>Provided by VK_VALVE_mutable_descriptor_type</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MUTABLE_DESCRIPTOR_TYPE_FEATURES_VALVE              = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MUTABLE_DESCRIPTOR_TYPE_FEATURES_EXT,

    /// <remarks>Provided by VK_VALVE_mutable_descriptor_type</remarks>
    VK_STRUCTURE_TYPE_MUTABLE_DESCRIPTOR_TYPE_CREATE_INFO_VALVE                           = VK_STRUCTURE_TYPE_MUTABLE_DESCRIPTOR_TYPE_CREATE_INFO_EXT,

    /// <remarks>Provided by VK_KHR_format_feature_flags2</remarks>
    VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_3_KHR                                             = VK_STRUCTURE_TYPE_FORMAT_PROPERTIES_3,

    /// <remarks>Provided by VK_EXT_pipeline_properties</remarks>
    VK_STRUCTURE_TYPE_PIPELINE_INFO_EXT                                                   = VK_STRUCTURE_TYPE_PIPELINE_INFO_KHR,

    /// <remarks>Provided by VK_EXT_global_priority_query</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GLOBAL_PRIORITY_QUERY_FEATURES_EXT                  = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_GLOBAL_PRIORITY_QUERY_FEATURES_KHR,

    /// <remarks>Provided by VK_EXT_global_priority_query</remarks>
    VK_STRUCTURE_TYPE_QUEUE_FAMILY_GLOBAL_PRIORITY_PROPERTIES_EXT                         = VK_STRUCTURE_TYPE_QUEUE_FAMILY_GLOBAL_PRIORITY_PROPERTIES_KHR,

    /// <remarks>Provided by VK_KHR_maintenance4</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_FEATURES_KHR                          = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_FEATURES,

    /// <remarks>Provided by VK_KHR_maintenance4</remarks>
    VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_PROPERTIES_KHR                        = VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_MAINTENANCE_4_PROPERTIES,

    /// <remarks>Provided by VK_KHR_maintenance4</remarks>
    VK_STRUCTURE_TYPE_DEVICE_BUFFER_MEMORY_REQUIREMENTS_KHR                               = VK_STRUCTURE_TYPE_DEVICE_BUFFER_MEMORY_REQUIREMENTS,

    /// <remarks>Provided by VK_KHR_maintenance4</remarks>
    VK_STRUCTURE_TYPE_DEVICE_IMAGE_MEMORY_REQUIREMENTS_KHR                                = VK_STRUCTURE_TYPE_DEVICE_IMAGE_MEMORY_REQUIREMENTS,

    /// <remarks>Provided by VK_EXT_shader_object</remarks>
    VK_STRUCTURE_TYPE_SHADER_REQUIRED_SUBGROUP_SIZE_CREATE_INFO_EXT                       = VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_REQUIRED_SUBGROUP_SIZE_CREATE_INFO,
}
