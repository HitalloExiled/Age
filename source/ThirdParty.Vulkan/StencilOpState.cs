using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkStencilOpState.html">VkStencilOpState</see>
/// </summary>
public unsafe record StencilOpState : NativeReference<VkStencilOpState>
{
    public StencilOp FailOp
    {
        get => this.PNative->failOp;
        init => this.PNative->failOp = value;
    }

    public StencilOp PassOp
    {
        get => this.PNative->passOp;
        init => this.PNative->passOp = value;
    }

    public StencilOp DepthFailOp
    {
        get => this.PNative->depthFailOp;
        init => this.PNative->depthFailOp = value;
    }

    public CompareOp CompareOp
    {
        get => this.PNative->compareOp;
        init => this.PNative->compareOp = value;
    }

    public uint CompareMask
    {
        get => this.PNative->compareMask;
        init => this.PNative->compareMask = value;
    }

    public uint WriteMask
    {
        get => this.PNative->writeMask;
        init => this.PNative->writeMask = value;
    }

    public uint Reference
    {
        get => this.PNative->reference;
        init => this.PNative->reference = value;
    }
}
