using System.Runtime.InteropServices;
using System.Text;
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

        // var x = reflection.GlobalParamsVarLayout.TypeLayout.Type?.GenericContainer; // Crashes
        var y = reflection.GlobalParamsVarLayout.TypeLayout?.Fields[0].TypeLayout.Type?.ElementType?.Fields[0].Type;
        var z = reflection.GlobalParamsVarLayout.TypeLayout?.Fields[1].Variable?.Name;

        // TODO: Implements asserts
    }

    [Fact]
    public unsafe void ReflectionUsingPInvoke()
    {
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

        var session = PInvoke.spCreateSession(null);
        var request = PInvoke.spCreateCompileRequest(session);

        var translationUnitIndex = PInvoke.spAddTranslationUnit(request, SlangSourceLanguage.Slang, null);

        fixed (byte* pName   = "shader.slang"u8)
        fixed (byte* pSource = Encoding.UTF8.GetBytes(source))
        {
            PInvoke.spAddTranslationUnitSourceString(request, translationUnitIndex, pName, pSource);
        }

        PInvoke.spSetCodeGenTarget(request, SlangCompileTarget.Spirv);

        fixed (byte* pProfile = "spirv_1_0"u8)
        {
            PInvoke.spSetTargetProfile(request, 0, PInvoke.spFindProfile(session, pProfile));
        }

        if (PInvoke.spCompile(request) < 0)
        {
            var diagnostic = Marshal.PtrToStringAnsi((nint)PInvoke.spGetDiagnosticOutput(request));
            Console.WriteLine(diagnostic);
        }

        var reflection = PInvoke.spGetReflection(request);

        //reflection.GlobalParamsVarLayout.TypeLayout.CategoryCount;
        var reflection_GlobalParamsVarLayout                          = PInvoke.spReflection_getGlobalParamsVarLayout(reflection);
        var reflection_GlobalParamsVarLayout_TypeLayout               = PInvoke.spReflectionVariableLayout_GetTypeLayout(reflection_GlobalParamsVarLayout);
        var reflection_GlobalParamsVarLayout_TypeLayout_CategoryCount = PInvoke.spReflectionTypeLayout_GetCategoryCount(reflection_GlobalParamsVarLayout_TypeLayout);

        // reflection.GlobalParamsVarLayout.TypeLayout?.Fields[0].TypeLayout.Type?.ElementType?.Fields[0].TypeLayout.Type;
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0                                          = PInvoke.spReflectionTypeLayout_GetFieldByIndex(reflection_GlobalParamsVarLayout_TypeLayout, 0);
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout                               = PInvoke.spReflectionVariableLayout_GetTypeLayout(reflection_GlobalParamsVarLayout_TypeLayout_Fields0);
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type                          = PInvoke.spReflectionTypeLayout_GetType(reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout);
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type_ElementType              = PInvoke.spReflectionType_GetElementType(reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type);
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type_ElementType_Fields0      = PInvoke.spReflectionType_GetFieldByIndex(reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type_ElementType, 0);
        var reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type_ElementType_Fields0_Type = PInvoke.spReflectionVariable_GetType(reflection_GlobalParamsVarLayout_TypeLayout_Fields0_TypeLayout_Type_ElementType_Fields0);

        PInvoke.spDestroyCompileRequest(request);
        PInvoke.spDestroySession(session);
    }
}
