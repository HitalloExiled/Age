using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static class PipelineShaderStage
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineShaderStageCreateInfo.html">VkPipelineShaderStageCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineShaderStageCreateInfo>
    {
        private ShaderModule?       module;
        private string?             name;
        private SpecializationInfo? specializationInfo;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public VkPipelineShaderStageCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public ShaderStageFlags Stage
        {
            get => this.PNative->stage;
            init => this.PNative->stage = value;
        }

        public ShaderModule? Module
        {
            get => this.module;
            init => this.PNative->module = this.module = value;
        }

        public string? Name
        {
            get => this.name;
            init => Init(ref this.name, ref this.PNative->pName, value);
        }

        public SpecializationInfo? SpecializationInfo
        {
            get => this.specializationInfo;
            init => this.PNative->pSpecializationInfo = this.specializationInfo = value;
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pName);
    }
}
