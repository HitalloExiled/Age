namespace ThirdParty.Slang;

public enum SlangTypeKind : uint
{
    None,
    Struct,
    Array,
    Matrix,
    Vector,
    Scalar,
    ConstantBuffer,
    Resource,
    SamplerState,
    TextureBuffer,
    ShaderStorageBuffer,
    ParameterBlock,
    GenericTypeParameter,
    Interface,
    OutputStream,
    MeshOutput,
    Specialized,
    Feedback,
    Pointer,
    DynamicResource,
    Count,
}
