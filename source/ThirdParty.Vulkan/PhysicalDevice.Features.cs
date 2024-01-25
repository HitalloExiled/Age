using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class PhysicalDevice
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPhysicalDeviceFeatures.html">VkPhysicalDeviceFeatures</see>
    /// </summary>
    public unsafe record Features : NativeReference<VkPhysicalDeviceFeatures>
    {
        public bool RobustBufferAccess
        {
            get => this.PNative->robustBufferAccess;
            init => this.PNative->robustBufferAccess = value;
        }

        public bool FullDrawIndexUint32
        {
            get => this.PNative->fullDrawIndexUint32;
            init => this.PNative->fullDrawIndexUint32 = value;
        }

        public bool ImageCubeArray
        {
            get => this.PNative->imageCubeArray;
            init => this.PNative->imageCubeArray = value;
        }

        public bool IndependentBlend
        {
            get => this.PNative->independentBlend;
            init => this.PNative->independentBlend = value;
        }

        public bool GeometryShader
        {
            get => this.PNative->geometryShader;
            init => this.PNative->geometryShader = value;
        }

        public bool TessellationShader
        {
            get => this.PNative->tessellationShader;
            init => this.PNative->tessellationShader = value;
        }

        public bool SampleRateShading
        {
            get => this.PNative->sampleRateShading;
            init => this.PNative->sampleRateShading = value;
        }

        public bool DualSrcBlend
        {
            get => this.PNative->dualSrcBlend;
            init => this.PNative->dualSrcBlend = value;
        }

        public bool LogicOp
        {
            get => this.PNative->logicOp;
            init => this.PNative->logicOp = value;
        }

        public bool MultiDrawIndirect
        {
            get => this.PNative->multiDrawIndirect;
            init => this.PNative->multiDrawIndirect = value;
        }

        public bool DrawIndirectFirstInstance
        {
            get => this.PNative->drawIndirectFirstInstance;
            init => this.PNative->drawIndirectFirstInstance = value;
        }

        public bool DepthClamp
        {
            get => this.PNative->depthClamp;
            init => this.PNative->depthClamp = value;
        }

        public bool DepthBiasClamp
        {
            get => this.PNative->depthBiasClamp;
            init => this.PNative->depthBiasClamp = value;
        }

        public bool FillModeNonSolid
        {
            get => this.PNative->fillModeNonSolid;
            init => this.PNative->fillModeNonSolid = value;
        }

        public bool DepthBounds
        {
            get => this.PNative->depthBounds;
            init => this.PNative->depthBounds = value;
        }

        public bool WideLines
        {
            get => this.PNative->wideLines;
            init => this.PNative->wideLines = value;
        }

        public bool LargePoints
        {
            get => this.PNative->largePoints;
            init => this.PNative->largePoints = value;
        }

        public bool AlphaToOne
        {
            get => this.PNative->alphaToOne;
            init => this.PNative->alphaToOne = value;
        }

        public bool MultiViewport
        {
            get => this.PNative->multiViewport;
            init => this.PNative->multiViewport = value;
        }

        public bool SamplerAnisotropy
        {
            get => this.PNative->samplerAnisotropy;
            init => this.PNative->samplerAnisotropy = value;
        }

        public bool TextureCompressionEtc2
        {
            get => this.PNative->textureCompressionETC2;
            init => this.PNative->textureCompressionETC2 = value;
        }

        public bool TextureCompressionAstcLdr
        {
            get => this.PNative->textureCompressionASTC_LDR;
            init => this.PNative->textureCompressionASTC_LDR = value;
        }

        public bool TextureCompressionBc
        {
            get => this.PNative->textureCompressionBC;
            init => this.PNative->textureCompressionBC = value;
        }

        public bool OcclusionQueryPrecise
        {
            get => this.PNative->occlusionQueryPrecise;
            init => this.PNative->occlusionQueryPrecise = value;
        }

        public bool PipelineStatisticsQuery
        {
            get => this.PNative->pipelineStatisticsQuery;
            init => this.PNative->pipelineStatisticsQuery = value;
        }

        public bool VertexPipelineStoresAndAtomics
        {
            get => this.PNative->vertexPipelineStoresAndAtomics;
            init => this.PNative->vertexPipelineStoresAndAtomics = value;
        }

        public bool FragmentStoresAndAtomics
        {
            get => this.PNative->fragmentStoresAndAtomics;
            init => this.PNative->fragmentStoresAndAtomics = value;
        }

        public bool ShaderTessellationAndGeometryPointSize
        {
            get => this.PNative->shaderTessellationAndGeometryPointSize;
            init => this.PNative->shaderTessellationAndGeometryPointSize = value;
        }

        public bool ShaderImageGatherExtended
        {
            get => this.PNative->shaderImageGatherExtended;
            init => this.PNative->shaderImageGatherExtended = value;
        }

        public bool ShaderStorageImageExtendedFormats
        {
            get => this.PNative->shaderStorageImageExtendedFormats;
            init => this.PNative->shaderStorageImageExtendedFormats = value;
        }

        public bool ShaderStorageImageMultisample
        {
            get => this.PNative->shaderStorageImageMultisample;
            init => this.PNative->shaderStorageImageMultisample = value;
        }

        public bool ShaderStorageImageReadWithoutFormat
        {
            get => this.PNative->shaderStorageImageReadWithoutFormat;
            init => this.PNative->shaderStorageImageReadWithoutFormat = value;
        }

        public bool ShaderStorageImageWriteWithoutFormat
        {
            get => this.PNative->shaderStorageImageWriteWithoutFormat;
            init => this.PNative->shaderStorageImageWriteWithoutFormat = value;
        }

        public bool ShaderUniformBufferArrayDynamicIndexing
        {
            get => this.PNative->shaderUniformBufferArrayDynamicIndexing;
            init => this.PNative->shaderUniformBufferArrayDynamicIndexing = value;
        }

        public bool ShaderSampledImageArrayDynamicIndexing
        {
            get => this.PNative->shaderSampledImageArrayDynamicIndexing;
            init => this.PNative->shaderSampledImageArrayDynamicIndexing = value;
        }

        public bool ShaderStorageBufferArrayDynamicIndexing
        {
            get => this.PNative->shaderStorageBufferArrayDynamicIndexing;
            init => this.PNative->shaderStorageBufferArrayDynamicIndexing = value;
        }

        public bool ShaderStorageImageArrayDynamicIndexing
        {
            get => this.PNative->shaderStorageImageArrayDynamicIndexing;
            init => this.PNative->shaderStorageImageArrayDynamicIndexing = value;
        }

        public bool ShaderClipDistance
        {
            get => this.PNative->shaderClipDistance;
            init => this.PNative->shaderClipDistance = value;
        }

        public bool ShaderCullDistance
        {
            get => this.PNative->shaderCullDistance;
            init => this.PNative->shaderCullDistance = value;
        }

        public bool ShaderFloat64
        {
            get => this.PNative->shaderFloat64;
            init => this.PNative->shaderFloat64 = value;
        }

        public bool ShaderInt64
        {
            get => this.PNative->shaderInt64;
            init => this.PNative->shaderInt64 = value;
        }

        public bool ShaderInt16
        {
            get => this.PNative->shaderInt16;
            init => this.PNative->shaderInt16 = value;
        }

        public bool ShaderResourceResidency
        {
            get => this.PNative->shaderResourceResidency;
            init => this.PNative->shaderResourceResidency = value;
        }

        public bool ShaderResourceMinLod
        {
            get => this.PNative->shaderResourceMinLod;
            init => this.PNative->shaderResourceMinLod = value;
        }

        public bool SparseBinding
        {
            get => this.PNative->sparseBinding;
            init => this.PNative->sparseBinding = value;
        }

        public bool SparseResidencyBuffer
        {
            get => this.PNative->sparseResidencyBuffer;
            init => this.PNative->sparseResidencyBuffer = value;
        }

        public bool SparseResidencyImage2D
        {
            get => this.PNative->sparseResidencyImage2D;
            init => this.PNative->sparseResidencyImage2D = value;
        }

        public bool SparseResidencyImage3D
        {
            get => this.PNative->sparseResidencyImage3D;
            init => this.PNative->sparseResidencyImage3D = value;
        }

        public bool SparseResidency2Samples
        {
            get => this.PNative->sparseResidency2Samples;
            init => this.PNative->sparseResidency2Samples = value;
        }

        public bool SparseResidency4Samples
        {
            get => this.PNative->sparseResidency4Samples;
            init => this.PNative->sparseResidency4Samples = value;
        }

        public bool SparseResidency8Samples
        {
            get => this.PNative->sparseResidency8Samples;
            init => this.PNative->sparseResidency8Samples = value;
        }

        public bool SparseResidency16Samples
        {
            get => this.PNative->sparseResidency16Samples;
            init => this.PNative->sparseResidency16Samples = value;
        }

        public bool SparseResidencyAliased
        {
            get => this.PNative->sparseResidencyAliased;
            init => this.PNative->sparseResidencyAliased = value;
        }

        public bool VariableMultisampleRate
        {
            get => this.PNative->variableMultisampleRate;
            init => this.PNative->variableMultisampleRate = value;
        }

        public bool InheritedQueries
        {
            get => this.PNative->inheritedQueries;
            init => this.PNative->inheritedQueries = value;
        }

        internal Features(in VkPhysicalDeviceFeatures vkPhysicalDeviceFeatures) : base(vkPhysicalDeviceFeatures) { }

        public Features() : base() { }
    }
}
