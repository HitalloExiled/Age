using ThirdParty.Shaderc;
using ThirdParty.Shaderc.Enums;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Interfaces;

public interface IShader
{
    static abstract string                                 Name              { get; }
    static abstract VkPrimitiveTopology                    PrimitiveTopology { get; }
    static abstract Dictionary<VkShaderStageFlags, byte[]> Stages            { get; }


    private static byte[] CompileShader(string name, ShaderKind shaderKind)
    {
        var extension = shaderKind switch
        {
            ShaderKind.FragmentShader => "frag",
            ShaderKind.VertexShader   => "vert",
            _ => throw new InvalidOperationException()
        };

        var source = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, $"Shaders/{name}.{extension}"));

        using var compiler = new Compiler();

        var result = compiler.CompileIntoSpv(source, shaderKind, name, "main");

        return result.CompilationStatus != CompilationStatus.Success
            ? throw new Exception($"Error compiling shader: {result.ErrorMessage}")
            : result.Bytes;
    }

    protected static byte[] ReadFragmentShader(string name) =>
        CompileShader(name, ShaderKind.FragmentShader);

    protected static byte[] ReadVertexShader(string name) =>
        CompileShader(name, ShaderKind.VertexShader);
}
