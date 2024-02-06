namespace ThirdParty.Vulkan.Native;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceFeatures.html">VkPhysicalDeviceFeatures</see>
/// </summary>
public struct VkPhysicalDeviceFeatures
{
    public VkBool32 RobustBufferAccess;
    public VkBool32 FullDrawIndexUint32;
    public VkBool32 ImageCubeArray;
    public VkBool32 IndependentBlend;
    public VkBool32 GeometryShader;
    public VkBool32 TessellationShader;
    public VkBool32 SampleRateShading;
    public VkBool32 DualSrcBlend;
    public VkBool32 LogicOp;
    public VkBool32 MultiDrawIndirect;
    public VkBool32 DrawIndirectFirstInstance;
    public VkBool32 DepthClamp;
    public VkBool32 DepthBiasClamp;
    public VkBool32 FillModeNonSolid;
    public VkBool32 DepthBounds;
    public VkBool32 WideLines;
    public VkBool32 LargePoints;
    public VkBool32 AlphaToOne;
    public VkBool32 MultiViewport;
    public VkBool32 SamplerAnisotropy;
    public VkBool32 TextureCompressionEtc2;
    public VkBool32 TextureCompressionAstcLdr;
    public VkBool32 TextureCompressionBc;
    public VkBool32 OcclusionQueryPrecise;
    public VkBool32 PipelineStatisticsQuery;
    public VkBool32 VertexPipelineStoresAndAtomics;
    public VkBool32 FragmentStoresAndAtomics;
    public VkBool32 ShaderTessellationAndGeometryPointSize;
    public VkBool32 ShaderImageGatherExtended;
    public VkBool32 ShaderStorageImageExtendedFormats;
    public VkBool32 ShaderStorageImageMultisample;
    public VkBool32 ShaderStorageImageReadWithoutFormat;
    public VkBool32 ShaderStorageImageWriteWithoutFormat;
    public VkBool32 ShaderUniformBufferArrayDynamicIndexing;
    public VkBool32 ShaderSampledImageArrayDynamicIndexing;
    public VkBool32 ShaderStorageBufferArrayDynamicIndexing;
    public VkBool32 ShaderStorageImageArrayDynamicIndexing;
    public VkBool32 ShaderClipDistance;
    public VkBool32 ShaderCullDistance;
    public VkBool32 ShaderFloat64;
    public VkBool32 ShaderInt64;
    public VkBool32 ShaderInt16;
    public VkBool32 ShaderResourceResidency;
    public VkBool32 ShaderResourceMinLod;
    public VkBool32 SparseBinding;
    public VkBool32 SparseResidencyBuffer;
    public VkBool32 SparseResidencyImage2D;
    public VkBool32 SparseResidencyImage3D;
    public VkBool32 SparseResidency2Samples;
    public VkBool32 SparseResidency4Samples;
    public VkBool32 SparseResidency8Samples;
    public VkBool32 SparseResidency16Samples;
    public VkBool32 SparseResidencyAliased;
    public VkBool32 VariableMultisampleRate;
    public VkBool32 InheritedQueries;
}
