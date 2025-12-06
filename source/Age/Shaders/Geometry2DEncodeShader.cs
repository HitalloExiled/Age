using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DEncodeShader(VkRenderPass renderPass, in ShaderOptions shaderOptions)
: Geometry2DShader($"{nameof(Geometry2DEncodeShader)}.slang", renderPass, shaderOptions)
{
    public override string Name { get; } = nameof(Geometry2DEncodeShader);

    public Geometry2DEncodeShader(VkRenderPass renderPass, bool watch)
    :  this(renderPass, new ShaderOptions() { Watch = watch }) { }
}
