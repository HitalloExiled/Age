using System.Diagnostics;

namespace Age.Vulkan.Native;

[DebuggerDisplay("{Value}")]
public record struct VkInstance(nint Value)
{
    public static implicit operator VkInstance(nint value) => new(value);

    public static implicit operator nint(VkInstance value) => value.Value;
}
