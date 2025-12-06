using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DStencilMaskShader(VkRenderPass renderPass, in ShaderOptions shaderOptions)
: Geometry2DShader($"{nameof(Geometry2DStencilMaskShader)}.slang", renderPass, shaderOptions), IShaderFactory
{
    public override string Name { get; } = nameof(Geometry2DStencilMaskShader);

    public Geometry2DStencilMaskShader(VkRenderPass renderPass, StencilOp stencilOp, bool watch)
    :  this(renderPass, new() { StencilOp = stencilOp, Watch = watch }) { }

    public static Shader Create(VkRenderPass renderPass, in ShaderOptions options) =>
        new Geometry2DStencilMaskShader(renderPass, options);
}
