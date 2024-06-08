using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public abstract class Resource : Disposable
{
    protected VulkanRenderer Renderer { get; }

    internal Resource(VulkanRenderer renderer) =>
        this.Renderer = renderer;
}

public abstract class Resource<T> : Resource
{
    public T Value { get; }

    internal Resource(VulkanRenderer renderer, T value) : base(renderer) =>
        this.Value = value;

    public static implicit operator T(Resource<T> value) => value.Value;
}
