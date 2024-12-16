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
        struct AssembledVertex
        {
            float3 position : POSITION;
            float3 normal   : NORMAL;
            float2 uv       : TEXCOORD;
        };

        struct RasterVertex
        {
            float3 worldPosition;
            float3 worldNormal;
            float2 uv;
        };

        struct Model
        {
            float3x4 modelToWorld;
            float3x4 modelToWorld_inverseTranspose;
        }

        struct Material
        {
            Texture2D<float3> albedoMap;
            Texture2D<float3> normalMap;
            Texture2D<float> glossMap;
            SamplerState sampler;
            float2 uvScale;
            float2 uvBias;
        }

        struct Camera
        {
            float3x4 worldToView;
            float3x4 worldToView_inverseTranspose;

            float4x4 viewToProj;
        }

        struct DirectionalLight
        {
            float3 intensity;
            float3 direction;
        }

        struct Environment
        {
            TextureCube environmentMap;
            DirectionalLight light;
        }

        uniform Model                       model;
        uniform ParameterBlock<Material>    material;
        uniform ConstantBuffer<Camera>      camera;
        uniform ParameterBlock<Environment> environment;

        [shader("vertex")]
        [require(sm_6_0)]
        void vertexMain(
            in  AssembledVertex assembledVertex : A,
            out RasterVertex    rasterVertex    : R,
            in  uint            vertexID        : SV_VertexID,
            out float4          projPosition    :  SV_Position
        )
        {
            float3 worldPosition = mul(model.modelToWorld, float4(assembledVertex.position,1));

            rasterVertex.worldPosition = worldPosition;
            rasterVertex.worldNormal = mul(model.modelToWorld_inverseTranspose, float4(assembledVertex.normal,0));
            rasterVertex.uv = assembledVertex.uv;

            float3 viewPosition = mul(camera.worldToView, float4(worldPosition,1));
            projPosition = mul(camera.viewToProj, float4(viewPosition,1));
        }

        [shader("fragment")]
        [require(sm_6_0)]
        float4 fragmentMain(in RasterVertex vertex : R) : SV_Target0
        {
            float3 normal = vertex.worldNormal;

            float3 albedo = material.albedoMap.Sample(material.sampler, vertex.uv);

            float3 color = albedo * max(0, dot(normal, environment.light.direction));
            return float4(color, 1);
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
