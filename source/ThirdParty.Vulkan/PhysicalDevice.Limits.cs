using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class PhysicalDevice
{
    /// <inheritdoc cref="VkPhysicalDeviceLimits" />
    public record Limits : NativeReference<VkPhysicalDeviceLimits>
    {
        private float[] lineWidthRange           = [];
        private uint[]  maxComputeWorkGroupCount = [];
        private uint[]  maxComputeWorkGroupSize  = [];
        private uint[]  maxViewportDimensions    = [];
        private float[] pointSizeRange           = [];
        private float[] viewportBoundsRange      = [];

        public uint               MaxImageDimension1D                             => this.PNative->maxImageDimension1D;
        public uint               MaxImageDimension2D                             => this.PNative->maxImageDimension2D;
        public uint               MaxImageDimension3D                             => this.PNative->maxImageDimension3D;
        public uint               MaxImageDimensionCube                           => this.PNative->maxImageDimensionCube;
        public uint               MaxImageArrayLayers                             => this.PNative->maxImageArrayLayers;
        public uint               MaxTexelBufferElements                          => this.PNative->maxTexelBufferElements;
        public uint               MaxUniformBufferRange                           => this.PNative->maxUniformBufferRange;
        public uint               MaxStorageBufferRange                           => this.PNative->maxStorageBufferRange;
        public uint               MaxPushConstantsSize                            => this.PNative->maxPushConstantsSize;
        public uint               MaxMemoryAllocationCount                        => this.PNative->maxMemoryAllocationCount;
        public uint               MaxSamplerAllocationCount                       => this.PNative->maxSamplerAllocationCount;
        public VkDeviceSize       BufferImageGranularity                          => this.PNative->bufferImageGranularity;
        public VkDeviceSize       SparseAddressSpaceSize                          => this.PNative->sparseAddressSpaceSize;
        public uint               MaxBoundDescriptorSets                          => this.PNative->maxBoundDescriptorSets;
        public uint               MaxPerStageDescriptorSamplers                   => this.PNative->maxPerStageDescriptorSamplers;
        public uint               MaxPerStageDescriptorUniformBuffers             => this.PNative->maxPerStageDescriptorUniformBuffers;
        public uint               MaxPerStageDescriptorStorageBuffers             => this.PNative->maxPerStageDescriptorStorageBuffers;
        public uint               MaxPerStageDescriptorSampledImages              => this.PNative->maxPerStageDescriptorSampledImages;
        public uint               MaxPerStageDescriptorStorageImages              => this.PNative->maxPerStageDescriptorStorageImages;
        public uint               MaxPerStageDescriptorInputAttachments           => this.PNative->maxPerStageDescriptorInputAttachments;
        public uint               MaxPerStageResources                            => this.PNative->maxPerStageResources;
        public uint               MaxDescriptorSetSamplers                        => this.PNative->maxDescriptorSetSamplers;
        public uint               MaxDescriptorSetUniformBuffers                  => this.PNative->maxDescriptorSetUniformBuffers;
        public uint               MaxDescriptorSetUniformBuffersDynamic           => this.PNative->maxDescriptorSetUniformBuffersDynamic;
        public uint               MaxDescriptorSetStorageBuffers                  => this.PNative->maxDescriptorSetStorageBuffers;
        public uint               MaxDescriptorSetStorageBuffersDynamic           => this.PNative->maxDescriptorSetStorageBuffersDynamic;
        public uint               MaxDescriptorSetSampledImages                   => this.PNative->maxDescriptorSetSampledImages;
        public uint               MaxDescriptorSetStorageImages                   => this.PNative->maxDescriptorSetStorageImages;
        public uint               MaxDescriptorSetInputAttachments                => this.PNative->maxDescriptorSetInputAttachments;
        public uint               MaxVertexInputAttributes                        => this.PNative->maxVertexInputAttributes;
        public uint               MaxVertexInputBindings                          => this.PNative->maxVertexInputBindings;
        public uint               MaxVertexInputAttributeOffset                   => this.PNative->maxVertexInputAttributeOffset;
        public uint               MaxVertexInputBindingStride                     => this.PNative->maxVertexInputBindingStride;
        public uint               MaxVertexOutputComponents                       => this.PNative->maxVertexOutputComponents;
        public uint               MaxTessellationGenerationLevel                  => this.PNative->maxTessellationGenerationLevel;
        public uint               MaxTessellationPatchSize                        => this.PNative->maxTessellationPatchSize;
        public uint               MaxTessellationControlPerVertexInputComponents  => this.PNative->maxTessellationControlPerVertexInputComponents;
        public uint               MaxTessellationControlPerVertexOutputComponents => this.PNative->maxTessellationControlPerVertexOutputComponents;
        public uint               MaxTessellationControlPerPatchOutputComponents  => this.PNative->maxTessellationControlPerPatchOutputComponents;
        public uint               MaxTessellationControlTotalOutputComponents     => this.PNative->maxTessellationControlTotalOutputComponents;
        public uint               MaxTessellationEvaluationInputComponents        => this.PNative->maxTessellationEvaluationInputComponents;
        public uint               MaxTessellationEvaluationOutputComponents       => this.PNative->maxTessellationEvaluationOutputComponents;
        public uint               MaxGeometryShaderInvocations                    => this.PNative->maxGeometryShaderInvocations;
        public uint               MaxGeometryInputComponents                      => this.PNative->maxGeometryInputComponents;
        public uint               MaxGeometryOutputComponents                     => this.PNative->maxGeometryOutputComponents;
        public uint               MaxGeometryOutputVertices                       => this.PNative->maxGeometryOutputVertices;
        public uint               MaxGeometryTotalOutputComponents                => this.PNative->maxGeometryTotalOutputComponents;
        public uint               MaxFragmentInputComponents                      => this.PNative->maxFragmentInputComponents;
        public uint               MaxFragmentOutputAttachments                    => this.PNative->maxFragmentOutputAttachments;
        public uint               MaxFragmentDualSrcAttachments                   => this.PNative->maxFragmentDualSrcAttachments;
        public uint               MaxFragmentCombinedOutputResources              => this.PNative->maxFragmentCombinedOutputResources;
        public uint               MaxComputeSharedMemorySize                      => this.PNative->maxComputeSharedMemorySize;
        public uint[]             MaxComputeWorkGroupCount                        => Get(ref this.maxComputeWorkGroupCount, this.PNative->maxComputeWorkGroupCount, 3);
        public uint               MaxComputeWorkGroupInvocations                  => this.PNative->maxComputeWorkGroupInvocations;
        public uint[]             MaxComputeWorkGroupSize                         => Get(ref this.maxComputeWorkGroupSize, this.PNative->maxComputeWorkGroupSize, 3);
        public uint               SubPixelPrecisionBits                           => this.PNative->subPixelPrecisionBits;
        public uint               SubTexelPrecisionBits                           => this.PNative->subTexelPrecisionBits;
        public uint               MipmapPrecisionBits                             => this.PNative->mipmapPrecisionBits;
        public uint               MaxDrawIndexedIndexValue                        => this.PNative->maxDrawIndexedIndexValue;
        public uint               MaxDrawIndirectCount                            => this.PNative->maxDrawIndirectCount;
        public float              MaxSamplerLodBias                               => this.PNative->maxSamplerLodBias;
        public float              MaxSamplerAnisotropy                            => this.PNative->maxSamplerAnisotropy;
        public uint               MaxViewports                                    => this.PNative->maxViewports;
        public uint[]             MaxViewportDimensions                           => Get(ref this.maxViewportDimensions, this.PNative->maxViewportDimensions, 2);
        public float[]            ViewportBoundsRange                             => Get(ref this.viewportBoundsRange, this.PNative->viewportBoundsRange, 2);
        public uint               ViewportSubPixelBits                            => this.PNative->viewportSubPixelBits;
        public ulong              MinMemoryMapAlignment                           => this.PNative->minMemoryMapAlignment;
        public VkDeviceSize       MinTexelBufferOffsetAlignment                   => this.PNative->minTexelBufferOffsetAlignment;
        public VkDeviceSize       MinUniformBufferOffsetAlignment                 => this.PNative->minUniformBufferOffsetAlignment;
        public VkDeviceSize       MinStorageBufferOffsetAlignment                 => this.PNative->minStorageBufferOffsetAlignment;
        public int                MinTexelOffset                                  => this.PNative->minTexelOffset;
        public uint               MaxTexelOffset                                  => this.PNative->maxTexelOffset;
        public int                MinTexelGatherOffset                            => this.PNative->minTexelGatherOffset;
        public uint               MaxTexelGatherOffset                            => this.PNative->maxTexelGatherOffset;
        public float              MinInterpolationOffset                          => this.PNative->minInterpolationOffset;
        public float              MaxInterpolationOffset                          => this.PNative->maxInterpolationOffset;
        public uint               SubPixelInterpolationOffsetBits                 => this.PNative->subPixelInterpolationOffsetBits;
        public uint               MaxFramebufferWidth                             => this.PNative->maxFramebufferWidth;
        public uint               MaxFramebufferHeight                            => this.PNative->maxFramebufferHeight;
        public uint               MaxFramebufferLayers                            => this.PNative->maxFramebufferLayers;
        public VkSampleCountFlags FramebufferColorSampleCounts                    => this.PNative->framebufferColorSampleCounts;
        public VkSampleCountFlags FramebufferDepthSampleCounts                    => this.PNative->framebufferDepthSampleCounts;
        public VkSampleCountFlags FramebufferStencilSampleCounts                  => this.PNative->framebufferStencilSampleCounts;
        public VkSampleCountFlags FramebufferNoAttachmentsSampleCounts            => this.PNative->framebufferNoAttachmentsSampleCounts;
        public uint               MaxColorAttachments                             => this.PNative->maxColorAttachments;
        public VkSampleCountFlags SampledImageColorSampleCounts                   => this.PNative->sampledImageColorSampleCounts;
        public VkSampleCountFlags SampledImageIntegerSampleCounts                 => this.PNative->sampledImageIntegerSampleCounts;
        public VkSampleCountFlags SampledImageDepthSampleCounts                   => this.PNative->sampledImageDepthSampleCounts;
        public VkSampleCountFlags SampledImageStencilSampleCounts                 => this.PNative->sampledImageStencilSampleCounts;
        public VkSampleCountFlags StorageImageSampleCounts                        => this.PNative->storageImageSampleCounts;
        public uint               MaxSampleMaskWords                              => this.PNative->maxSampleMaskWords;
        public VkBool32           TimestampComputeAndGraphics                     => this.PNative->timestampComputeAndGraphics;
        public float              TimestampPeriod                                 => this.PNative->timestampPeriod;
        public uint               MaxClipDistances                                => this.PNative->maxClipDistances;
        public uint               MaxCullDistances                                => this.PNative->maxCullDistances;
        public uint               MaxCombinedClipAndCullDistances                 => this.PNative->maxCombinedClipAndCullDistances;
        public uint               DiscreteQueuePriorities                         => this.PNative->discreteQueuePriorities;
        public float[]            PointSizeRange                                  => Get(ref this.pointSizeRange, this.PNative->pointSizeRange, 2);
        public float[]            LineWidthRange                                  => Get(ref this.lineWidthRange, this.PNative->lineWidthRange, 2);
        public float              PointSizeGranularity                            => this.PNative->pointSizeGranularity;
        public float              LineWidthGranularity                            => this.PNative->lineWidthGranularity;
        public VkBool32           StrictLines                                     => this.PNative->strictLines;
        public VkBool32           StandardSampleLocations                         => this.PNative->standardSampleLocations;
        public VkDeviceSize       OptimalBufferCopyOffsetAlignment                => this.PNative->optimalBufferCopyOffsetAlignment;
        public VkDeviceSize       OptimalBufferCopyRowPitchAlignment              => this.PNative->optimalBufferCopyRowPitchAlignment;
        public VkDeviceSize       NonCoherentAtomSize                             => this.PNative->nonCoherentAtomSize;

        internal Limits(in VkPhysicalDeviceLimits limits) : base(limits) { }
    }
}
