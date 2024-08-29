using ThirdParty.Shaderc.Enums;
using ThirdParty.SpirvCross;
using ThirdParty.SpirvCross.Enums;
using SpirvCompiler = ThirdParty.Shaderc.Compiler;

namespace ThirdParty.Tests.SpirvCross;

public class ContextTest
{
    [Fact]
    public void Instanciate()
    {
        using var context = new Context();

        Assert.True(true);
    }

    [Fact]
    public void Parse()
    {
        var source =
        """
        #version 450

        layout(binding = 0) uniform sampler2D texSampler;

        layout(location = 0) in vec3 fragColor;
        layout(location = 1) in vec2 fragTexCoord;

        layout(location = 0) out vec4 outColor;

        void main() {
            outColor = texture(texSampler, fragTexCoord);
        }
        """;

        using var spirvCompiler = new SpirvCompiler();
        using var context       = new Context();

        var bytes        = spirvCompiler.CompileIntoSpv(source, ShaderKind.GlslFragmentShader, "shader.frag", "main").Bytes;
        var parsedSpirv  = context.ParseSpirv(bytes);
        var compiler     = context.CreateCompiler(Backend.Glsl, parsedSpirv, CaptureMode.TakeOwnership);
        var resources    = compiler.CreateShaderResources();
        var sampledImage = resources.GetResourceListForType(ResorceType.SampledImage);
        var inputs       = resources.GetResourceListForType(ResorceType.StageInput);
        var compiled     = compiler.Compile();

        Assert.Equal("texSampler", sampledImage[0].Name);
        Assert.Equal(0u, compiler.GetDecoration(sampledImage[0].Id, Decoration.Binding));

        Assert.Equal("fragTexCoord", inputs[0].Name);
        Assert.Equal(1u, compiler.GetDecoration(inputs[0].Id, Decoration.Location));

        Assert.Equal("fragColor", inputs[1].Name);
        Assert.Equal(0u, compiler.GetDecoration(inputs[1].Id, Decoration.Location));

        Assert.True(compiled.Length > 0);
    }

    [Fact]
    public void ParseInvalidSpirv()
    {
        using var context = new Context();

        Assert.Throws<Exception>(() => context.ParseSpirv(new byte[255]));
    }
}
