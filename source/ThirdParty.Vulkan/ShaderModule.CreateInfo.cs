using System.Runtime.InteropServices;
using Age.Core.Interop;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public partial class ShaderModule
{
    /// <inheritdoc cref="VkShaderModuleCreateInfo" />
    public unsafe record CreateInfo : NativeReference<VkShaderModuleCreateInfo>
    {
        private byte[] code = [];

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

        public byte[] Code
        {
            get => this.code;
            init
            {
                this.code              = value;
                this.PNative->pCode    = PointerHelper.Alloc(MemoryMarshal.Cast<byte, uint>(value));
                this.PNative->codeSize = (uint)value.Length;
            }
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pCode);
    }
}
