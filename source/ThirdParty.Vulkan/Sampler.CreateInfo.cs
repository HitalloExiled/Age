using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class Sampler
{
    /// <inheritdoc cref="VkSamplerCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkSamplerCreateInfo>
    {
        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public Filter MagFilter
        {
            get => this.PNative->magFilter;
            init => this.PNative->magFilter = value;
        }

        public Filter MinFilter
        {
            get => this.PNative->minFilter;
            init => this.PNative->minFilter = value;
        }

        public SamplerMipmapMode  MipmapMode
        {
            get => this.PNative->mipmapMode;
            init => this.PNative->mipmapMode = value;
        }

        public SamplerAddressMode AddressModeU
        {
            get => this.PNative->addressModeU;
            init => this.PNative->addressModeU = value;
        }

        public SamplerAddressMode AddressModeV
        {
            get => this.PNative->addressModeV;
            init => this.PNative->addressModeV = value;
        }

        public SamplerAddressMode AddressModeW
        {
            get => this.PNative->addressModeW;
            init => this.PNative->addressModeW = value;
        }

        public float MipLodBias
        {
            get => this.PNative->mipLodBias;
            init => this.PNative->mipLodBias = value;
        }

        public bool AnisotropyEnable
        {
            get => this.PNative->anisotropyEnable;
            init => this.PNative->anisotropyEnable = value;
        }

        public float MaxAnisotropy
        {
            get => this.PNative->maxAnisotropy;
            init => this.PNative->maxAnisotropy = value;
        }

        public bool CompareEnable
        {
            get => this.PNative->compareEnable;
            init => this.PNative->compareEnable = value;
        }

        public CompareOp CompareOp
        {
            get => this.PNative->compareOp;
            init => this.PNative->compareOp = value;
        }

        public float MinLod
        {
            get => this.PNative->minLod;
            init => this.PNative->minLod = value;
        }

        public float MaxLod
        {
            get => this.PNative->maxLod;
            init => this.PNative->maxLod = value;
        }

        public BorderColor BorderColor
        {
            get => this.PNative->borderColor;
            init => this.PNative->borderColor = value;
        }
        public bool UnnormalizedCoordinates
        {
            get => this.PNative->unnormalizedCoordinates;
            init => this.PNative->unnormalizedCoordinates = value;
        }
    }
}
