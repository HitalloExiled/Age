using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DStencilMaskShader(VkRenderPass renderPass, StencilOp stencilOp, uint subpass, bool watch)
: Geometry2DShader($"{nameof(Geometry2DStencilMaskShader)}.slang", renderPass, subpass, stencilOp, watch)
{
    public override string Name { get; } = nameof(Geometry2DStencilMaskShader);

    public Geometry2DStencilMaskShader(RenderPass renderPass, StencilOp stencilOp, bool watch) : this(renderPass, stencilOp, 0, watch)
    { }

    public Geometry2DStencilMaskShader(RenderTarget renderTarget, StencilOp stencilOp, bool watch) : this(renderTarget.RenderPass, stencilOp, 0, watch)
    { }
}
