using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public abstract class Resource<T>(VulkanRenderer renderer, T value) : Disposable
{
    protected VulkanRenderer Renderer { get; } = renderer;

    public T Value { get; } = value;

    public static implicit operator T(Resource<T> value) => value.Value;
}
