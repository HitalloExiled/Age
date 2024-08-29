namespace ThirdParty.SpirvCross.Enums;

public enum Result
{
    /// <summary>
    /// Success.
    /// </summary>
	Success = 0,

	/// <summary>
    /// The SPIR-V is invalid. Should have been caught by validation ideally.
    /// </summary>
	ErrorInvalidSpirv = -1,

	/// <summary>
    /// The SPIR-V might be valid or invalid, but SPIRV-Cross currently cannot correctly translate this to your target language.
    /// </summary>
	ErrorUnsupportedSpirv = -2,

	/// <summary>
    /// If for some reason we hit this, new or malloc failed.
    /// </summary>
	ErrorOutOfMemory = -3,

	/// <summary>
    /// Invalid API argument.
    /// </summary>
	ErrorInvalidArgument = -4,

	ErrorIntMax = 0x7fffffff
}
