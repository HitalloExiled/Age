using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class Texture : Resource
{
    private readonly bool imageOwner;

    public required Image       Image     { get; init; }
    public required VkImageView ImageView { get; init; }

    public Sampler Sampler { get; } = new();

    internal Texture(bool imageOwner) =>
        this.imageOwner = imageOwner;

    protected override void Disposed()
    {
        if (this.imageOwner)
        {
            this.Image.Dispose();
        }

        this.ImageView.Dispose();
        this.Sampler.Dispose();
    }

    public void Update(Span<byte> data) =>
        this.Image.Update(data);
}
