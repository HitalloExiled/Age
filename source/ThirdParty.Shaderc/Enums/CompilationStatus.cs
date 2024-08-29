namespace ThirdParty.Shaderc.Enums;

/// <summary>
/// Indicate the status of a compilation.
/// </summary>
public enum CompilationStatus
{
    Success = 0,

    /// <summary>
    /// Error stage deduction
    /// </summary>
    InvalidStage = 1,
    CompilationError = 2,

    /// <summary>
    /// Unexpected failure
    /// </summary>
    InternalError = 3,
    NullResultObject = 4,
    InvalidAssembly = 5,
    ValidationError = 6,
    TransformationError = 7,
    ConfigurationError = 8,
}
