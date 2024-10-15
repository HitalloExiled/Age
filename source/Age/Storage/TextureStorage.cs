using Age.Core;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan.Enums;

namespace Age.Storage;

public class TextureStorage : Disposable
{
    private static TextureStorage? singleton;

    public static TextureStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly VulkanRenderer renderer;
    private readonly Dictionary<string, Texture> textures = [];

    public Texture DefaultTexture { get; }
    public Texture EmptyTexture   { get; }

    public TextureStorage(VulkanRenderer renderer)
    {
        singleton = this;

        this.renderer = renderer;

        const int DEFAULT_SIZE = 8;

        var textureCreateInfo = new TextureCreateInfo
        {
            Format    = VkFormat.B8G8R8A8Unorm,
            ImageType = VkImageType.N2D,
            Width     = DEFAULT_SIZE,
            Height    = DEFAULT_SIZE,
            Depth     = 1,
        };

        this.DefaultTexture = this.renderer.CreateTexture(textureCreateInfo);
        this.DefaultTexture.Image.ClearColor(Color.Margenta, VkImageLayout.ShaderReadOnlyOptimal);

        textureCreateInfo.Width  = 1;
        textureCreateInfo.Height = 1;
        textureCreateInfo.Format = VkFormat.R8Unorm;

        this.EmptyTexture = this.renderer.CreateTexture(textureCreateInfo);
        this.EmptyTexture.Image.ClearColor(Color.White, VkImageLayout.ShaderReadOnlyOptimal);
    }

    public void Add(string name, Texture texture) =>
        this.textures[name] = texture;

    protected override void Disposed()
    {
        this.renderer.DeferredDispose(this.DefaultTexture);
        this.renderer.DeferredDispose(this.EmptyTexture);
        this.renderer.DeferredDispose(this.textures.Values);
    }
}
