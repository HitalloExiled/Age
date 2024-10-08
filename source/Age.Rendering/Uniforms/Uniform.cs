using ThirdParty.Vulkan.Enums;

namespace Age.Rendering.Uniforms;

public abstract record Uniform
{
    public abstract VkDescriptorType Type { get; }

    public required uint Binding { get; init; }

    public bool Bounded { get; set; }
}
