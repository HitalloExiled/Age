using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public sealed class Texture : Resource
{
    private readonly bool imageOwner;

    public Image       Image     { get; }
    public VkImageView ImageView { get; }

    public Sampler Sampler { get; } = new();

    public Texture(in TextureCreateInfo textureCreateInfo)
    {
        const VkSampleCountFlags SAMPLES = VkSampleCountFlags.N1;
        const VkImageTiling      TILING  = VkImageTiling.Optimal;
        const VkImageUsageFlags  USAGE   = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers = 1,
            Extent = new()
            {
                Width  = textureCreateInfo.Width,
                Height = textureCreateInfo.Height,
                Depth  = textureCreateInfo.Depth
            },
            Format        = textureCreateInfo.Format,
            ImageType     = textureCreateInfo.ImageType,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = SAMPLES,
            Tiling        = TILING,
            Usage         = USAGE,
        };

        this.Image      = new Image(imageCreateInfo, VkImageLayout.TransferDstOptimal);
        this.ImageView  = CreateImageView(this.Image);
        this.imageOwner = true;
    }

    private static VkImageView CreateImageView(Image image)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = image.Format,
            Image            = image.Instance.Handle,
            SubresourceRange = new()
            {
                AspectMask = VkImageAspectFlags.Color,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        return VulkanRenderer.Singleton.Context.Device.CreateImageView(imageViewCreateInfo);
    }

    public Texture(Image image)
    {
        this.Image     = image;
        this.ImageView = CreateImageView(image);
    }

    public Texture(in TextureCreateInfo textureCreate, scoped ReadOnlySpan<byte> data) : this(textureCreate) =>
        this.Image!.Update(data);

    protected override void OnDisposed()
    {
        if (this.imageOwner)
        {
            this.Image.Dispose();
        }

        this.ImageView.Dispose();
        this.Sampler.Dispose();
    }

    public void Update(scoped ReadOnlySpan<byte> data) =>
        this.Image.Update(data);
}
