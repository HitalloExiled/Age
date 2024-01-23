namespace ThirdParty.Shaderc.Enums;

public enum ShaderKind
{
    /// <summary>
    /// Forced shader kinds. These shader kinds force the compiler to compile the
    // source code as the specified kind of shader.
    /// </summary>
    VertexShader,
    FragmentShader,
    ComputeShader,
    GeometryShader,
    TessControlShader,
    TessEvaluationShader,

    GlslVertexShader = VertexShader,
    GlslFragmentShader = FragmentShader,
    GlslComputeShader = ComputeShader,
    GlslGeometryShader = GeometryShader,
    GlslTessControlShader = TessControlShader,
    GlslTessEvaluationShader = TessEvaluationShader,

    /// <summary>
    /// Deduce the shader kind from #pragma annotation in the source code. Compiler
    // will emit error if #pragma annotation is not found.
    /// </summary>
    GlslInferFromSource,

    /// <summary>
    /// Default shader kinds. Compiler will fall back to compile the source code as
    /// the specified kind of shader when #pragma annotation is not found in the
    /// source code.
    /// </summary>
    GlslDefaultVertexShader,
    GlslDefaultFragmentShader,
    GlslDefaultComputeShader,
    GlslDefaultGeometryShader,
    GlslDefaultTessControlShader,
    GlslDefaultTessEvaluationShader,
    SpirvAssembly,
    RaygenShader,
    AnyhitShader,
    ClosesthitShader,
    MissShader,
    IntersectionShader,
    CallableShader,
    GlslRaygenShader = RaygenShader,
    GlslAnyhitShader = AnyhitShader,
    GlslClosesthitShader = ClosesthitShader,
    GlslMissShader = MissShader,
    GlslIntersectionShader = IntersectionShader,
    GlslCallableShader = CallableShader,
    GlslDefaultRaygenShader,
    GlslDefaultAnyhitShader,
    GlslDefaultClosesthitShader,
    GlslDefaultMissShader,
    GlslDefaultIntersectionShader,
    GlslDefaultCallableShader,
    TaskShader,
    MeshShader,
    GlslTaskShader = TaskShader,
    GlslMeshShader = MeshShader,
    GlslDefaultTaskShader,
    GlslDefaultMeshShader,
}
