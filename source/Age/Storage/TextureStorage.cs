using System.Diagnostics.CodeAnalysis;
using Age.Core;
using Age.Rendering.Vulkan;
using Age.Resources;

namespace Age.Storage;

public struct ResourceEntry<T>(T resource, int users)
{
    public T   Resource = resource;
    public int Users    = users;
}

public class TextureStorage : Disposable
{
    private static TextureStorage? singleton;

    public static TextureStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly VulkanRenderer renderer;
    private readonly Dictionary<string, ResourceEntry<Texture2D>> textures = [];

    public TextureStorage(VulkanRenderer renderer)
    {
        singleton = this;

        this.renderer = renderer;
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            foreach (var entry in this.textures.Values)
            {
                this.renderer.DeferredDispose(entry.Resource);
            }

            this.textures.Clear();
        }
    }

    public void Add(string name, Texture2D texture) =>
        this.textures.Add(name, new(texture, 1));

    public Texture2D Get(string name) =>
        this.textures[name].Resource;

    public bool TryGet(string name, [NotNullWhen(true)] out Texture2D? texture)
    {
        if (this.textures.TryGetValue(name, out var entry))
        {
            texture = entry.Resource;
            return true;
        }

        texture = null;
        return false;
    }
}
