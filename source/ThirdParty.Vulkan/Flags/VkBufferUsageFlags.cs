namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferUsageFlagBits.html">VkBufferUsageFlagBits</see>
/// </summary>
[Flags]
public enum VkBufferUsageFlags
{
    TransferSrc                                = 0x00000001,
    TransferDst                                = 0x00000002,
    UniformTexelBuffer                         = 0x00000004,
    StorageTexelBuffer                         = 0x00000008,
    UniformBuffer                              = 0x00000010,
    StorageBuffer                              = 0x00000020,
    IndexBuffer                                = 0x00000040,
    VertexBuffer                               = 0x00000080,
    IndirectBuffer                             = 0x00000100,
    ShaderDeviceAddress                        = 0x00020000,
    VideoDecodeSrcKHR                          = 0x00002000,
    VideoDecodeDstKHR                          = 0x00004000,
    TransformFeedbackBufferEXT                 = 0x00000800,
    TransformFeedbackCounterBufferEXT          = 0x00001000,
    ConditionalRenderingEXT                    = 0x00000200,
#if VK_ENABLE_BETA_EXTENSIONS
    ExecutionGraphScratchAmdx                  = 0x02000000,
#endif
    AccelerationStructureBuildInputReadOnlyKHR = 0x00080000,
    AccelerationStructureStorageKHR            = 0x00100000,
    ShaderBindingTableKHR                      = 0x00000400,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKHR                          = 0x00008000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKHR                          = 0x00010000,
#endif
    SamplerDescriptorBufferEXT                 = 0x00200000,
    ResourceDescriptorBufferEXT                = 0x00400000,
    PushDescriptorsDescriptorBufferEXT         = 0x04000000,
    MicromapBuildInputReadOnlyEXT              = 0x00800000,
    MicromapStorageEXT                         = 0x01000000,
    RayTracingNV                               = ShaderBindingTableKHR,
    ShaderDeviceAddressEXT                     = ShaderDeviceAddress,
    ShaderDeviceAddressKHR                     = ShaderDeviceAddress,
}
