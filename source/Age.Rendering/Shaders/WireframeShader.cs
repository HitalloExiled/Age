using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public class WireframeShader : IShader<CanvasShader.Vertex, CanvasShader.PushConstant>
{
    public static string Name { get; } = nameof(WireframeShader);

    public static Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = new()
    {
        [VkShaderStageFlags.Fragment] = IShader.ReadFragmentShader(Name),
        [VkShaderStageFlags.Vertex]   = CanvasShader.Stages[VkShaderStageFlags.Vertex],
    };

    public static VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;
}
