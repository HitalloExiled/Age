using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

/// <inheritdoc cref="VkSubpassDependency" />
public unsafe record SubpassDependency : NativeReference<VkSubpassDependency>
{
    public uint SrcSubpass
    {
        get => this.PNative->srcSubpass;
        init => this.PNative->srcSubpass = value;
    }

    public uint DstSubpass
    {
        get => this.PNative->dstSubpass;
        init => this.PNative->dstSubpass = value;
    }

    public VkPipelineStageFlags SrcStageMask
    {
        get => this.PNative->srcStageMask;
        init => this.PNative->srcStageMask = value;
    }

    public VkPipelineStageFlags DstStageMask
    {
        get => this.PNative->dstStageMask;
        init => this.PNative->dstStageMask = value;
    }

    public VkAccessFlags SrcAccessMask
    {
        get => this.PNative->srcAccessMask;
        init => this.PNative->srcAccessMask = value;
    }

    public VkAccessFlags DstAccessMask
    {
        get => this.PNative->dstAccessMask;
        init => this.PNative->dstAccessMask = value;
    }

    public VkDependencyFlags DependencyFlags
    {
        get => this.PNative->dependencyFlags;
        init => this.PNative->dependencyFlags = value;
    }
}
