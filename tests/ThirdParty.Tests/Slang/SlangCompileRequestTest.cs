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

        var translationUnitIndex = request.AddTranslationUnit(SlangSourceLanguage.Slang, null);
        var entryPointIndex      = request.AddEntryPoint(translationUnitIndex, "main", SlangStage.Vertex);

        request.AddTranslationUnitSourceString(translationUnitIndex, "shader.slang", source);

        request.SetCodeGenTarget(SlangCompileTarget.Spirv);
        request.SetTargetProfile(0, session.FindProfile("spirv_1_0"));

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

        struct PushConstant
        {
            float x;
            float y;
            float z;
        }

        [vk::binding(0)]
        cbuffer ubo
        {
            UniformBufferObject ubo;
        }

        [vk::binding(1)]
        Sampler2D sampledTexture;

        [vk::push_constant]
        PushConstant pushConstant;

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

        Assert.Equal(3, reflection.GlobalParamsTypeLayout.Fields.Length);

        var fields      = reflection.GlobalParamsTypeLayout.Fields;
        var entryPoints = reflection.EntryPoints;

        Assert.True(
            fields[0] is
            {
                TypeLayout.ElementTypeLayout.ParameterSize:  192,
                TypeLayout.ElementVarLayout.ParameterOffset: 0,
                TypeLayout.ParameterCategory: SlangParameterCategory.DescriptorTableSlot,
                TypeLayout.Type.Kind:         SlangTypeKind.ConstantBuffer,
                Variable.Name:                "ubo",
            }
        );

        Assert.True(
            fields[1] is
            {
                TypeLayout.ParameterCategory:  SlangParameterCategory.DescriptorTableSlot,
                TypeLayout.Type.Kind:          SlangTypeKind.Resource,
                TypeLayout.Type.ResourceShape: SlangResourceShape.Texture2D,
                Variable.Name:                 "sampledTexture",
            }
        );

        Assert.True(
            fields[2] is
            {
                TypeLayout.ElementTypeLayout.ParameterSize:  12,
                TypeLayout.ElementVarLayout.ParameterOffset: 0,
                TypeLayout.ParameterCategory:                SlangParameterCategory.PushConstantBuffer,
                TypeLayout.Type.Kind:                        SlangTypeKind.ConstantBuffer,
                Variable.Name:                               "pushConstant",
            }
        );

        Assert.True(
            entryPoints[0] is
            {
                Parameters:
                [
                    {
                        TypeLayout.Fields:
                        [
                            {
                                ParameterOffset:          0,
                                SemanticIndex:            0,
                                SemanticName:             "LOCATION",
                                TypeLayout.ParameterSize: 1,
                            },
                            {
                                ParameterOffset:          1,
                                SemanticIndex:            1,
                                SemanticName:             "LOCATION",
                                TypeLayout.ParameterSize: 1,
                            },
                            {
                                ParameterOffset:          2,
                                SemanticIndex:            2,
                                SemanticName:             "LOCATION",
                                TypeLayout.ParameterSize: 1,
                            }
                        ]
                    }
                ],
                Stage: SlangStage.Vertex,
            }
        );

        Assert.True(
            entryPoints[1] is
            {
                Parameters:
                [
                    {
                        TypeLayout.Fields:
                        [
                            {
                                SemanticIndex:            0,
                                SemanticName:             "LOCATION",
                                TypeLayout.ParameterSize: 1,
                            },
                            {
                                SemanticIndex:            1,
                                SemanticName:             "LOCATION",
                                TypeLayout.ParameterSize: 1,
                            },
                        ]
                    }
                ],
                Stage: SlangStage.Fragment,
            }
        );
    }
}
