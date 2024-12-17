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

        var source =
        """
        struct UniformBufferObject
        {
            float4x4 model;
            float4x4 view;
            float4x4 proj;
        };

        [vk::binding(0)]
        cbuffer ubo
        {
            UniformBufferObject ubo;
        }

        [vk::binding(1)]
        Sampler2D sampledTexture;

        struct VertexInput
        {
            float3 position : LOCATION0;
            float3 color    : LOCATION1;
            float2 uv       : LOCATION2;
        };

        struct VertexOutput
        {
            float4 position : SV_Position;
            float3 color    : LOCATION0;
            float2 uv       : LOCATION1;
        };

        struct FragmentInput
        {
            float3 color : LOCATION0;
            float2 uv    : LOCATION1;
        };

        [shader("vertex")]
        VertexOutput main(VertexInput input)
        {
            var worldPosition = mul(float4(input.position, 1.0), ubo.model);
            var viewPosition  = mul(worldPosition, ubo.view);
            var clipPosition  = mul(viewPosition, ubo.proj);

            return
            {
                clipPosition,
                input.color,
                input.uv
            };
        }

        [shader("fragment")]
        float4 main(FragmentInput input) : SV_Target
        {
            return sampledTexture.Sample(input.uv);
        }
        """;

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, null);
        // var entryPointIndex      = request.AddEntryPoint(translationUnitIndex, "main", SlangStage.Vertex);

        request.AddTranslationUnitSourceString(translationUnitIndex, "shader.slang", source);

        request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        request.SetTargetProfile(0, session.FindProfile("spirv_1_0"));

        if (!request.Compile())
        {
            var diagnostic = request.GetDiagnosticOutput();
            Console.WriteLine(diagnostic);
        }

        var dependencies = request.GetDependencyFiles();

        var reflection = request.GetReflection();

        var ubo            = reflection.GlobalParamsVarLayout.TypeLayout?.Fields[0].Variable?.Name;
        var sampledTexture = reflection.GlobalParamsVarLayout.TypeLayout?.Fields[1].Variable?.Name;

        // Assert.Equal(2u, reflection.EntryPointCount);
        // Assert.Equal(0u, reflection.GlobalConstantBufferBinding);
        // Assert.Equal(96u, reflection.GlobalConstantBufferSize);
        // Assert.NotNull(reflection.GlobalParamsTypeLayout);
        //     Assert.Equal(1u, reflection.GlobalParamsTypeLayout.CategoryCount);
        //     Assert.NotNull(reflection.GlobalParamsTypeLayout.ContainerVarLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.PendingDataLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.SemanticName);
        //         Assert.Equal(default, reflection.GlobalParamsTypeLayout.ContainerVarLayout.Stage);
        //         Assert.NotNull(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout);
        //             Assert.Equal(1u, reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.CategoryCount);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.ContainerVarLayout);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.ElementTypeLayout);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.ElementVarLayout);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.ExplicitCounter);
        //             Assert.Equal(default, reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.FieldCount);
        //             Assert.Equal(-1, reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.GenericParamIndex);
        //             Assert.Equal(default, reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.Kind);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.PendingDataTypeLayout);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.ReflectionType);
        //             Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.TypeLayout!.SpecializedTypePendingDataVarLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ContainerVarLayout.Variable);
        //     Assert.NotNull(reflection.GlobalParamsTypeLayout.ElementTypeLayout);
        //     Assert.NotNull(reflection.GlobalParamsTypeLayout.ElementVarLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ElementVarLayout.PendingDataLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ElementVarLayout.SemanticName);
        //         Assert.Equal(default, reflection.GlobalParamsTypeLayout.ElementVarLayout.Stage);
        //         Assert.NotNull(reflection.GlobalParamsTypeLayout.ElementVarLayout.TypeLayout);
        //         Assert.Null(reflection.GlobalParamsTypeLayout.ElementVarLayout.Variable);
        //     Assert.Null(reflection.GlobalParamsTypeLayout.ExplicitCounter);
        //     Assert.Equal(0u, reflection.GlobalParamsTypeLayout.FieldCount);
        //     Assert.Equal(-1, reflection.GlobalParamsTypeLayout.GenericParamIndex);
        //     Assert.Equal(SlangTypeKind.ConstantBuffer, reflection.GlobalParamsTypeLayout.Kind);
        //     Assert.Null(reflection.GlobalParamsTypeLayout.PendingDataTypeLayout);
        //     Assert.Null(reflection.GlobalParamsTypeLayout.ReflectionType);
        //     Assert.Null(reflection.GlobalParamsTypeLayout.SpecializedTypePendingDataVarLayout);

        // Assert.NotNull(reflection.GlobalParamsVarLayout);
        // Assert.NotNull(reflection.Request);
        // Assert.NotNull(reflection.Session);

        foreach (var entryPoint in reflection.EntryPoints)
        {
            foreach (var entryPointParameter in entryPoint.Parameters)
            {
                // Assert.NotNull(entryPointParameter.TypeLayout);
                // Assert.NotNull(entryPointParameter.Variable);
                // Assert.NotNull(entryPointParameter.Variable.Name);
                // Assert.False(entryPointParameter.Variable.HasDefaultValue);
                // Assert.NotNull(entryPointParameter.Variable.ReflectionType);
                // Assert.NotNull(entryPointParameter.Variable.ReflectionType.ResourceResultType);
                // Assert.NotNull(entryPointParameter.Variable.ReflectionType.ElementType);
                // Assert.NotNull(entryPointParameter.Variable.GenericContainer);
                // Assert.Null(entryPointParameter.PendingDataLayout);
            }
        }
    }
}
