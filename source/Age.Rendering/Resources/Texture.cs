using Age.Numerics;
using Age.Rendering.Extensions;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public partial class Texture : Resource
{
    internal bool IsImageOwner { get; }

    internal Image       Image     { get; }
    internal VkImageView ImageView { get; }
    internal Sampler     Sampler   { get; } = new();

    public TextureFormat Format  => (TextureFormat)this.Image.Format;
    public SampleCount   Samples => (SampleCount)this.Image.Samples;
    public TextureType   Type    => (TextureType)this.Image.Type;
    public TextureUsage  Usage   => (TextureUsage)this.Image.Usage;

    public TextureAspect Aspect { get; }

    public Extent<uint> Extent => this.Image.Extent.ToExtent();

    internal Texture(Image image, bool owner, TextureAspect aspect)
    {
        this.Image        = image;
        this.ImageView    = CreateImageView(image, aspect);
        this.IsImageOwner = owner;
        this.Aspect       = aspect;
    }

    public Texture(in CreateInfo createInfo)
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = createInfo.Extent.ToExtent3D(),
            Format        = (VkFormat)createInfo.Format,
            ImageType     = (VkImageType)createInfo.Type,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = createInfo.Mipmap,
            Samples       = (VkSampleCountFlags)createInfo.Samples,
            Tiling        = VkImageTiling.Optimal,
            Usage         = (VkImageUsageFlags)createInfo.Usage,
        };

        this.IsImageOwner = true;
        this.Image      = new(imageCreateInfo);
        this.ImageView  = CreateImageView(this.Image, createInfo.Aspect);
        this.Aspect     = createInfo.Aspect;
    }

    public Texture(in CreateInfo createInfo, in Color clearColor) : this(createInfo) =>
        this.Image.ClearColor(clearColor, VkImageLayout.ShaderReadOnlyOptimal);

    public Texture(in CreateInfo createInfo, ReadOnlySpan<byte> buffer) : this(createInfo) =>
        this.Image.Update(buffer);

    private static VkImageView CreateImageView(Image image, TextureAspect aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = image.Format,
            Image            = image.Instance.Handle,
            SubresourceRange =
            {
                AspectMask = (VkImageAspectFlags)aspect,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        return VulkanRenderer.Singleton.Context.Device.CreateImageView(imageViewCreateInfo);
    }

    protected override void OnDisposed()
    {
        VulkanRenderer.Singleton.DeferredDispose(this.ImageView);
        this.Sampler.Dispose();

        if (this.IsImageOwner)
        {
            this.Image.Dispose();
        }
    }

    public void Update(ReadOnlySpan<byte> buffer) =>
        this.Image.Update(buffer);
}
