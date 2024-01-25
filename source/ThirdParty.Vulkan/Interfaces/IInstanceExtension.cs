namespace ThirdParty.Vulkan.Interfaces;

public interface IInstanceExtension<T> where T : IInstanceExtension<T>
{
    static abstract string Name { get; }
    static abstract T Create(Instance instance);
}
