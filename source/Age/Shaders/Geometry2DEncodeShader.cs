using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DEncodeShader : Geometry2DShader, IShaderFactory<Geometry2DEncodeShader>
{
    public override string Name => nameof(Geometry2DEncodeShader);

    private Geometry2DEncodeShader(VkRenderPass renderPass)
    : base($"{nameof(Geometry2DEncodeShader)}.slang", renderPass) { }

    public static Geometry2DEncodeShader Create(VkRenderPass renderPass) =>
        new(renderPass);
}
