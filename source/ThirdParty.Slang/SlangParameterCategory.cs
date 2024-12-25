namespace ThirdParty.Slang;

public enum SlangParameterCategory : uint
{
    None,
    Mixed,
    ConstantBuffer,
    ShaderResource,
    UnorderedAccess,
    VaryingInput,
    VaryingOutput,
    SamplerState,
    Uniform,
    DescriptorTableSlot,
    SpecializationConstant,
    PushConstantBuffer,

    /// <summary>
    /// HLSL register `space`, Vulkan GLSL `set`
    /// </summary>
    RegisterSpace,

    /// <summary>
    /// A parameter whose type is to be specialized by a global generic type argument
    /// </summary>
    Generic,

    RayPayload,
    HitAttributes,
    CallablePayload,
    ShaderRecord,

    /// <summary>
    /// An existential type parameter represents a "hole" that
    /// needs to be filled with a concrete type to enable
    /// generation of specialized code.
    ///
    /// Consider this example:
    ///
    ///      struct MyParams
    ///      {
    ///          IMaterial material;
    ///          ILight lights[3];
    ///      };
    ///
    /// This `MyParams` type introduces two existential type parameters:
    /// one for `material` and one for `lights`. Even though `lights`
    /// is an array, it only introduces one type parameter, because
    /// we need to have a *single* concrete type for all the array
    /// elements to be able to generate specialized code.
    ///
    /// </summary>
    ExistentialTypeParam,

    /// <summary>
    /// An existential object parameter represents a value
    /// that needs to be passed in to provide data for some
    /// interface-type shader parameter.
    ///
    /// Consider this example:
    ///
    ///      struct MyParams
    ///      {
    ///          IMaterial material;
    ///          ILight lights[3];
    ///      };
    ///
    /// This `MyParams` type introduces four existential object parameters:
    /// one for `material` and three for `lights` (one for each array
    /// element). This is consistent with the number of interface-type
    /// "objects" that are being passed through to the shader.
    /// </summary>
    ExistentialObjectParam,

    /// <summary>
    /// The register space offset for the sub-elements that occupies register spaces.
    /// </summary>
    SubElementRegisterSpace,

    /// <summary>
    /// The input_attachment_index subpass occupancy tracker
    /// </summary>
    Subpass,

    /// <summary>
    /// Metal tier-1 argument buffer element [[id]].
    /// </summary>
    MetalArgumentBufferElement,

    // Metal [[attribute]] inputs.
    MetalAttribute,

    /// <summary>
    /// Metal [[payload]] inputs
    /// </summary>
    MetalPayload,

    Count,

    /// <summary>
    /// Aliases for Metal-specific categories.
    /// </summary>
    MetalBuffer  = ConstantBuffer,
    MetalTexture = ShaderResource,
    MetalSampler = SamplerState,

    // DEPRECATED:
    VertexInput    = VaryingInput,
    FragmentOutput = VaryingOutput,
    CountV1        = Subpass,
}
