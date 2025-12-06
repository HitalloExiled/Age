using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Age.Core;
using Age.Core.Exceptions;
using Age.Core.Extensions;
using Age.Rendering.Resources;
using Age.Rendering.Vulkan;

namespace Age.Storage;

public struct ResourceEntry<T>(T resource, int users)
{
    public T   Resource = resource;
    public int Users    = users;
}

public sealed class TextureStorage : Disposable
{
    private readonly VulkanRenderer                            renderer;
    private readonly Dictionary<string, int>                   stringMap = [];
    private readonly Dictionary<int, ResourceEntry<Texture2D>> textures = [];

    [AllowNull]
    public static TextureStorage Singleton { get; private set; }

    public TextureStorage(VulkanRenderer renderer)
    {
        SingletonViolationException.ThrowIfNoSingleton(Singleton);

        Singleton = this;

        this.renderer = renderer;
    }

    private void Release(int hashcode)
    {
        ref var entry = ref this.textures.GetValueRefOrNullRef(hashcode);

        if (!Unsafe.IsNullRef(in entry))
        {
            entry.Users--;

            if (entry.Users == 0)
            {
                entry.Resource.Dispose();
                this.textures.Remove(hashcode);

                foreach (var (key, value) in this.stringMap)
                {
                    if (value == hashcode)
                    {
                        this.stringMap.Remove(key);
                    }
                }
            }
        }
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

    public void Add(string name, Texture2D texture)
    {
        var hashcode = texture.GetHashCode();

        this.textures.Add(hashcode, new(texture, 1));
        this.stringMap.Add(name, hashcode);
    }

    public Texture2D Get(string name) =>
        this.TryGet(name, out var texture)
            ? texture
            : throw new InvalidOperationException($"texture {name} not found");

    public void Release(string name)
    {
        if (this.stringMap.TryGetValue(name, out var hash))
        {
            this.Release(hash);
        }
    }

    public void Release(Texture texture) =>
        this.Release(texture.GetHashCode());

    public bool TryGet(string name, [NotNullWhen(true)] out Texture2D? texture)
    {
        if (this.stringMap.TryGetValue(name, out var hash))
        {
            ref var entry = ref this.textures.GetValueRefOrNullRef(hash);

            entry.Users++;

            texture = entry.Resource;
            return true;
        }

        texture = null;
        return false;
    }
}
