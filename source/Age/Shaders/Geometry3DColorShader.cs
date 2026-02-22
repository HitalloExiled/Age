using Age.Rendering.Resources;
using ThirdParty.Vulkan;

namespace Age.Shaders;

public sealed class Geometry3DColorShader : Geometry3DShader, IShaderFactory<Geometry3DColorShader>
{
    public override string Name => nameof(Geometry3DColorShader);

    private Geometry3DColorShader(VkRenderPass renderPass)
    : base($"{nameof(Geometry3DColorShader)}.slang", renderPass) { }

    public static Geometry3DColorShader Create(VkRenderPass renderPass) =>
        new(renderPass);
}
