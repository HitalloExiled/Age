using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public class PipelineInputAssemblyState
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkPipelineInputAssemblyStateCreateInfo.html">VkPipelineInputAssemblyStateCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkPipelineInputAssemblyStateCreateInfo>
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

        public PrimitiveTopology Topology
        {
            get => this.PNative->topology;
            init => this.PNative->topology = value;
        }

        public bool PrimitiveRestartEnable
        {
            get => this.PNative->primitiveRestartEnable;
            init => this.PNative->primitiveRestartEnable = value;
        }
    }
}
