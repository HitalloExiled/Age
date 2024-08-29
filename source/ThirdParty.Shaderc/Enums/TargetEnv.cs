namespace ThirdParty.Shaderc.Enums;

public enum TargetEnv
{
    /// <summary>
    /// SPIR-V under Vulkan semantics
    /// </summary>
    Vulkan,

    /// <summary>
    /// SPIR-V under OpenGL semantics
    /// </summary>
    Opengl,

    /// <summary>
    /// SPIR-V under OpenGL semantics, including compatibility profile functions
    /// </summary>
    /// <remarks>NOTE: SPIR-V code generation is not supported for shaders under OpenGL compatibility profile.</remarks>
    OpenglCompat,

    /// <summary>
    /// Deprecated, SPIR-V under WebGPU semantics
    /// </summary>
    Webgpu,

    Default = Vulkan
}
