using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public class WireframeShader : IShader
{
    public static string Name { get; } = nameof(WireframeShader);

    public static Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = new()
    {
        [VkShaderStageFlags.Fragment] = IShader.ReadFragmentShader(nameof(WireframeShader)),
        [VkShaderStageFlags.Vertex]   = IShader.ReadVertexShader(nameof(CanvasShader)),
    };

    public static VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.LineList;
}
