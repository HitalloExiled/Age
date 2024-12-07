using ThirdParty.Slang;

namespace ThirdParty.Tests.Slang;

public class SlangCompileRequestTest
{
    [Fact]
    public void CreateSlangCompileRequest()
    {
        var session = new SlangSession();
        var request = new SlangCompileRequest(session);

        request.Dispose();
        session.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void Compile()
    {
        using var session = new SlangSession();
        using var request = new SlangCompileRequest(session);

        var source =
        """
        struct VSOutput
        {
            float4 position : SV_POSITION;
            float3 color : COLOR0;
        };

        VSOutput main(float4 position : POSITION, float3 color : COLOR0)
        {
            VSOutput output;
            output.position = position;
            output.color = color;
            return output;
        }
        """;

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Hlsl, null);
        var entryPointIndex      = request.AddEntryPoint(translationUnitIndex, "main", SlangStage.Vertex);

        request.AddTranslationUnitSourceString(translationUnitIndex, "shader.slang", source);

        request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        request.SetTargetProfile(0, session.FindProfile("spirv_1_0"));
        // request.SetOptimizationLevel(SlangOptimizationLevel.High);
        // request.SetDebugInfoLevel(SlangDebugInfoLevel.Standard);

        if (!request.Compile())
        {
            var diagnostic = request.GetDiagnosticOutput();
            Console.WriteLine(diagnostic);
        }

        Assert.True(request.Compile());

        var compiledCode = request.GetEntryPointCode(entryPointIndex);

        Assert.NotEmpty(compiledCode);
    }

    [Fact]
    public void Reflection()
    {
        using var session = new SlangSession();
        using var request = new SlangCompileRequest(session);

        // var source =
        // """
        // cbuffer MyBuffer : register(b0)
        // {
        //     float4x4 transform;
        //     float time;
        // };

        // Texture2D myTexture : register(t0);
        // SamplerState mySampler : register(s0);

        // float4 main(float4 position : POSITION) : SV_POSITION
        // {
        //     return mul(transform, position);
        // }
        // """;
        // var source = File.ReadAllBytes("C:\\Users\\rafael.franca\\Projects\\Age\\tests\\ThirdParty.Tests\\Slang\\Shaders\\shader-a.slang");

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Hlsl, null);
        var entryPointIndex      = request.AddEntryPoint(translationUnitIndex, "main", SlangStage.Vertex);

        // request.AddTranslationUnitSourceString(translationUnitIndex, "shader.slang", source);
        request.AddTranslationUnitSourceFile(translationUnitIndex, "C:\\Users\\rafael.franca\\Projects\\Age\\tests\\ThirdParty.Tests\\Slang\\Shaders\\shader-a.slang");

        // request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        // request.SetTargetProfile(0, session.FindProfile("spirv_1_5"));
        // request.SetOptimizationLevel(SlangOptimizationLevel.High);
        // request.SetDebugInfoLevel(SlangDebugInfoLevel.Standard);

        if (!request.Compile())
        {
            var diagnostic = request.GetDiagnosticOutput();
            Console.WriteLine(diagnostic);
        }

        Assert.True(request.Compile());

        var dependencies = request.GetDependencyFiles();

        var reflection = request.GetReflection();

        var parameterCount = reflection.GetParameterCount();

        var parameters = new string[(int)parameterCount];

        for (var i = 0; i < parameterCount; i++)
        {
            var param = reflection.GetParameterByIndex((uint)i);
        }

        var compiledCode = request.GetEntryPointCode(entryPointIndex);

        Assert.NotEmpty(compiledCode);
    }
}
