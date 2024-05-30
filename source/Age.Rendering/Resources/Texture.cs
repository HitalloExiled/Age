using Age.Rendering.Enums;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Texture : Disposable
{
    public required Allocation  Allocation  { get; init; }
    public required VkExtent3D  Extent      { get; init; }
    public required VkImage     Image       { get; init; }
    public required VkImageView ImageView   { get; init; }
    public required TextureType TextureType { get; init; }

    protected override void OnDispose()
    {
        this.Allocation.Memory.Dispose();
        this.Image.Dispose();
        this.ImageView.Dispose();
    }
}
