namespace ThirdParty.Vulkan.Flags;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkImageCreateFlagBits.html">VkImageCreateFlagBits</see>
/// </summary>
[Flags]
public enum ImageCreateFlags
{
    SparseBinding                        = 0x00000001,
    SparseResidency                      = 0x00000002,
    SparseAliased                        = 0x00000004,
    MutableFormat                        = 0x00000008,
    CubeCompatible                       = 0x00000010,
    Alias                                = 0x00000400,
    SplitInstanceBindRegions             = 0x00000040,
    N2DArrayCompatible                   = 0x00000020,
    BlockTexelViewCompatible             = 0x00000080,
    ExtendedUsage                        = 0x00000100,
    Protected                            = 0x00000800,
    Disjoint                             = 0x00000200,
    CornerSampledBitNv                   = 0x00002000,
    SampleLocationsCompatibleDepthExt    = 0x00001000,
    SubsampledExt                        = 0x00004000,
    DescriptorBufferCaptureReplayExt     = 0x00010000,
    MultisampledRenderToSingleSampledExt = 0x00040000,
    N2DViewCompatibleExt                 = 0x00020000,
    FragmentDensityMapOffsetBitQcom      = 0x00008000,
    SplitInstanceBindRegionsBitKhr       = SplitInstanceBindRegions,
    N2DArrayCompatibleBitKhr             = N2DArrayCompatible,
    BlockTexelViewCompatibleBitKhr       = BlockTexelViewCompatible,
    ExtendedUsageBitKhr                  = ExtendedUsage,
    DisjointBitKhr                       = Disjoint,
    AliasBitKhr                          = Alias,
}
