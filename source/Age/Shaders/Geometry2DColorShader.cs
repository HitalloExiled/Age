using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DColorShader(VkRenderPass renderPass, uint subpass, bool watch)
: Geometry2DShader($"{nameof(Geometry2DColorShader)}.slang", renderPass, subpass, StencilOp.None, watch)
{
    public override string Name { get; } = nameof(Geometry2DColorShader);

    public Geometry2DColorShader(VkRenderPass renderPass, bool watch) : this(renderPass, 0, watch)
    { }

    public Geometry2DColorShader(RenderTarget renderTarget, bool watch) : this(renderTarget.RenderPass, 0, watch)
    { }
}
