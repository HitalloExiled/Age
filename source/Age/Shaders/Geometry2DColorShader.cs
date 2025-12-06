using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry2DColorShader(VkRenderPass renderPass, in ShaderOptions shaderOptions)
: Geometry2DShader($"{nameof(Geometry2DColorShader)}.slang", renderPass, shaderOptions), IShaderFactory
{
    public override string Name { get; } = nameof(Geometry2DColorShader);

    public Geometry2DColorShader(VkRenderPass renderPass, bool watch)
    :  this(renderPass, new ShaderOptions() { Watch = watch }) { }

    public static Shader Create(VkRenderPass renderPass, in ShaderOptions options) => new Geometry2DColorShader(renderPass, options);
}
