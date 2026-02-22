using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry3DEncodeShader : Geometry3DShader, IShaderFactory<Geometry3DEncodeShader>
{
    public override string Name => nameof(Geometry3DEncodeShader);

    private Geometry3DEncodeShader(VkRenderPass renderPass)
    : base($"{nameof(Geometry3DEncodeShader)}.slang", renderPass) { }

    public static Geometry3DEncodeShader Create(VkRenderPass renderPass) =>
        new(renderPass);
}
