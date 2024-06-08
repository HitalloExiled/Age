using Age.Rendering.Enums;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Texture : Resource
{
    public required Image       Image       { get; init; }
    public required VkImageView ImageView   { get; init; }
    public required TextureType TextureType { get; init; }

    internal Texture(VulkanRenderer renderer) : base(renderer)
    { }

    protected override void OnDispose()
    {
        this.Image.Dispose();
        this.ImageView.Dispose();
    }

    public void Update(Span<byte> data) =>
        this.Image.Update(data);
}
