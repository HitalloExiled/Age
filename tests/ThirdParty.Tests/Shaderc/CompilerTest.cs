using ThirdParty.Shaderc;
using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Tests.Shaderc;

public class CompilerTest
{
    [Fact]
    public void CompileShouldWork()
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

        using var compiler = new Compiler();

        var result = compiler.CompileIntoSpv(source, ShaderKind.GlslFragmentShader, "shader.frag", "main");

        Assert.Equal(CompilationStatus.Success, result.CompilationStatus);
        Assert.Equal("", result.ErrorMessage);
        Assert.Equal(0ul, result.Errors);
        Assert.Equal(0ul, result.Warnings);
        Assert.True(result.Bytes.Length > 0);
    }

    [Fact]
    public void CompileShouldFail()
    {
        using var compiler = new Compiler();

        var result = compiler.CompileIntoSpv("", ShaderKind.GlslFragmentShader, "shader.frag", "main");

        Assert.True(result.Errors > 0);
    }
}