namespace Age.Vulkan.Interfaces;

public interface IVkFlags
{
    bool HasFlag(uint value);
}

public interface IVkFlags<T> where T : Enum
{
    bool HasFlag(T value);
}
