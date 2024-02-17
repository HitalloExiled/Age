using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Shaders;

public class CanvasShader : IShader
{
    public static string Name { get; } = nameof(CanvasShader);

    public static Dictionary<VkShaderStageFlags, byte[]> Stages { get; } = new()
    {
        [VkShaderStageFlags.Fragment] = IShader.ReadFragmentShader(Name),
        [VkShaderStageFlags.Vertex]   = IShader.ReadVertexShader(Name),
    };

    public static VkPrimitiveTopology PrimitiveTopology { get; } = VkPrimitiveTopology.TriangleList;
}
