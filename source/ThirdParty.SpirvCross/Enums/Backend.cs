namespace ThirdParty.SpirvCross.Enums;

public enum Backend
{
    /// <summary>
    /// This backend can only perform reflection, no compiler options are supported. Maps to spirv_cross::Compiler.
    /// </summary>
    None = 0,

	/// <summary>
    /// spirv_cross::CompilerGLSL
    /// </summary>
    Glsl = 1,

	/// <summary>
    /// CompilerHLSL
    /// </summary>
    Hlsl = 2,

	/// <summary>
    /// CompilerMSL
    /// </summary>
    Msl = 3,

	/// <summary>
    /// CompilerCPP
    /// </summary>
    Cpp = 4,

	/// <summary>
    /// CompilerReflection w/ JSON backend
    /// </summary>
    Json = 5,

	IntMax = 0x7fffffff
}
