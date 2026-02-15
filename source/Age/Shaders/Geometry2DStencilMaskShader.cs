using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DStencilMaskShader : Geometry2DShader, IShaderFactory<Geometry2DStencilMaskShader>
{
    public override string Name => nameof(Geometry2DStencilMaskShader);

    private Geometry2DStencilMaskShader(VkRenderPass renderPass)
    : base($"{nameof(Geometry2DStencilMaskShader)}.slang", renderPass) { }

    public static Geometry2DStencilMaskShader Create(VkRenderPass renderPass) =>
        new(renderPass);
}
