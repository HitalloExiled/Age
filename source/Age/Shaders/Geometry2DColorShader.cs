using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DColorShader : Geometry2DShader, IShaderFactory<Geometry2DColorShader>
{
    public override string Name { get; } = nameof(Geometry2DColorShader);

    private Geometry2DColorShader(VkRenderPass renderPass)
    : base($"{nameof(Geometry2DColorShader)}.slang", renderPass) { }

    public static Geometry2DColorShader Create(VkRenderPass renderPass) => new(renderPass);
}
