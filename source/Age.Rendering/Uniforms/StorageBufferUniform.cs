using ThirdParty.Vulkan.Enums;
using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Uniforms;

public sealed record StorageBufferUniform : Uniform
{
    public override VkDescriptorType Type => VkDescriptorType.StorageBuffer;

    public required Buffer Buffer { get; init; }
}
