using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Texture : Resource
{
    private readonly bool ownsImage;

    public required Image       Image       { get; init; }
    public required VkImageView ImageView   { get; init; }

    internal Texture(VulkanRenderer renderer, bool ownsImage) : base(renderer) =>
        this.ownsImage = ownsImage;

    protected override void OnDispose()
    {
        if (this.ownsImage)
        {
            this.Image.Dispose();
        }

        this.ImageView.Dispose();
    }

    public void Update(Span<byte> data) =>
        this.Image.Update(data);
}
