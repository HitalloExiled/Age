namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkBufferUsageFlagBits.html">VkBufferUsageFlagBits</see>
/// </summary>
[Flags]
public enum BufferUsageFlags
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
    VideoDecodeSrcKhr                          = 0x00002000,
    VideoDecodeDstKhr                          = 0x00004000,
    TransformFeedbackBufferExt                 = 0x00000800,
    TransformFeedbackCounterBufferExt          = 0x00001000,
    ConditionalRenderingExt                    = 0x00000200,
#if VK_ENABLE_BETA_EXTENSIONS
    ExecutionGraphScratchAmdx                  = 0x02000000,
#endif
    AccelerationStructureBuildInputReadOnlyKhr = 0x00080000,
    AccelerationStructureStorageKhr            = 0x00100000,
    ShaderBindingTableKhr                      = 0x00000400,
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeDstKhr                          = 0x00008000,
#endif
#if VK_ENABLE_BETA_EXTENSIONS
    VideoEncodeSrcKhr                          = 0x00010000,
#endif
    SamplerDescriptorBufferExt                 = 0x00200000,
    ResourceDescriptorBufferExt                = 0x00400000,
    PushDescriptorsDescriptorBufferExt         = 0x04000000,
    MicromapBuildInputReadOnlyExt              = 0x00800000,
    MicromapStorageExt                         = 0x01000000,
    RayTracingNv                               = ShaderBindingTableKhr,
    ShaderDeviceAddressExt                     = ShaderDeviceAddress,
    ShaderDeviceAddressKhr                     = ShaderDeviceAddress,
}
