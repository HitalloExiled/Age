using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class ShaderModule
{
    /// <inheritdoc cref="VkShaderModuleCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkShaderModuleCreateInfo>
    {
        private uint[] code = [];

        public void* Next
        {
            get => this.PNative->pNext;
            init => this.PNative->pNext = value;
        }

        public uint Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public uint[] Code
        {
            get => this.code;
            init => Init(ref this.code, ref this.PNative->pCode, ref this.PNative->codeSize, value);
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pCode);
    }
}
