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
    private readonly VulkanRenderer                                  renderer;
    private readonly Dictionary<string, Texture2D>                   stringMap = [];
    private readonly Dictionary<Texture2D, ResourceEntry<Texture2D>> textures = [];

    [AllowNull]
    public static TextureStorage Singleton { get; private set; }

    public TextureStorage(VulkanRenderer renderer)
    {
        SingletonViolationException.ThrowIfNoSingleton(Singleton);

        Singleton = this;

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

    public void Add(string name, Texture2D texture)
    {
        this.textures.Add(texture, new(texture, 1));
        this.stringMap.Add(name, texture);
    }

    public Texture2D Get(string name) =>
        this.TryGet(name, out var texture)
            ? texture
            : throw new InvalidOperationException($"texture {name} not found");

    public void Release(Texture2D texture)
    {
        ref var entry = ref this.textures.GetValueRefOrNullRef(texture);

        if (!Unsafe.IsNullRef(in entry))
        {
            entry.Users--;

            if (entry.Users == 0)
            {
                entry.Resource.Dispose();
                this.textures.Remove(texture);

                foreach (var (key, value) in this.stringMap)
                {
                    if (value == texture)
                    {
                        this.stringMap.Remove(key);
                    }
                }
            }
        }
    }

    public void Release(string name)
    {
        if (this.stringMap.TryGetValue(name, out var texture))
        {
            this.Release(texture);
        }
    }

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
