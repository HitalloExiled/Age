using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DEncodeShader(VkRenderPass renderPass, uint subpass, bool watch)
: Geometry2DShader($"{nameof(Geometry2DEncodeShader)}.slang", renderPass, subpass, StencilOp.None, watch)
{
    public override string Name { get; } = nameof(Geometry2DEncodeShader);

    public Geometry2DEncodeShader(VkRenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }

    public Geometry2DEncodeShader(RenderTarget renderTarget, bool watch) : this(renderTarget.RenderPass, 0, watch)
    { }
}
