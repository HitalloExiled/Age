using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class ShaderModule
{
    /// <inheritdoc cref="VkShaderModuleCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkShaderModuleCreateInfo>
    {
        private byte[] code = [];

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

        public byte[] Code
        {
            get => this.code;
            init
            {
                this.code              = value;
                this.PNative->pCode    = PointerHelper.Alloc(value, BitConverter.ToUInt32);
                this.PNative->codeSize = (uint)value.Length / sizeof(uint);
            }
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pCode);
    }
}
