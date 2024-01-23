using System.Runtime.InteropServices;

namespace ThirdParty.Shaderc;

internal unsafe static partial class PInvoke
{
    //// <summary>
    //// Returns a shaderc_compiler_t that can be used to compile modules.
    //// A return of NULL indicates that there was an error initializing the compiler.
    //// Any function operating on shaderc_compiler_t must offer the basic
    //// thread-safety guarantee.
    //// [http://herbsutter.com/2014/01/13/gotw-95-solution-thread-safety-and-synchronization/]
    //// That is: concurrent invocation of these functions on DIFFERENT objects needs
    //// no synchronization; concurrent invocation of these functions on the SAME
    //// object requires synchronization IF AND ONLY IF some of them take a non-const
    //// argument.
    //// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial shaderc_compiler_t shaderc_compiler_initialize();

    //// <summary>
    //// Releases the resources held by the shaderc_compiler_t.
    //// After this call it is invalid to make any future calls to functions
    //// involving this shaderc_compiler_t.
    //// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void shaderc_compiler_release(shaderc_compiler_t compiler);

    /// <summary>
    /// Takes a GLSL source string and the associated shader kind, input file
    /// name, compiles it according to the given additional_options. If the shader
    /// kind is not set to a specified kind, but shaderc_glslc_infer_from_source,
    /// the compiler will try to deduce the shader kind from the source
    /// string and a failure in deducing will generate an error. Currently only
    /// #pragma annotation is supported. If the shader kind is set to one of the
    /// default shader kinds, the compiler will fall back to the default shader
    /// kind in case it failed to deduce the shader kind from source string.
    /// The input_file_name is a null-termintated string. It is used as a tag to
    /// identify the source string in cases like emitting error messages. It
    /// doesn't have to be a 'file name'.
    /// The source string will be compiled into SPIR-V binary and a
    /// shaderc_compilation_result will be returned to hold the results.
    /// The entry_point_name null-terminated string defines the name of the entry
    /// point to associate with this GLSL source. If the additional_options
    /// parameter is not null, then the compilation is modified by any options
    /// present.  May be safely called from multiple threads without explicit
    /// synchronization. If there was failure in allocating the compiler object,
    /// </summary>
    /// null will be returned.
    [LibraryImport(PLATFORM_PATH)]
    internal static partial shaderc_compilation_result_t shaderc_compile_into_spv(
        shaderc_compiler_t        compiler,
        byte*                     source_text,
        ulong                     source_text_size,
        shaderc_shader_kind       shader_kind,
        byte*                     input_file_name,
        byte*                     entry_point_name,
        shaderc_compile_options_t additional_options
    );

    /// <summary>
    /// Returns a copy of the given shaderc_compile_options_t.
    /// If NULL is passed as the parameter the call is the same as
    /// shaderc_compile_options_init.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial shaderc_compile_options_t shaderc_compile_options_clone(shaderc_compile_options_t options);

    /// <summary>
    /// Returns a default-initialized shaderc_compile_options_t that can be used
    /// to modify the functionality of a compiled module.
    /// A return of NULL indicates that there was an error initializing the options.
    /// Any function operating on shaderc_compile_options_t must offer the
    /// basic thread-safety guarantee.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial shaderc_compile_options_t shaderc_compile_options_initialize();

    /// <summary>
    /// Releases the compilation options. It is invalid to use the given
    /// shaderc_compile_options_t object in any future calls. It is safe to pass
    /// NULL to this function, and doing such will have no effect.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void shaderc_compile_options_release(shaderc_compile_options_t options);

    /// <summary>
    /// Sets the source language.  The default is GLSL.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void shaderc_compile_options_set_source_language(shaderc_compile_options_t options, shaderc_source_language lang);

    /// <summary>
    /// Returns a pointer to the start of the compilation output data bytes, either
    /// SPIR-V binary or char string. When the source string is compiled into SPIR-V
    /// binary, this is guaranteed to be castable to a uint32_t*. If the result
    /// contains assembly text or preprocessed source text, the pointer will point to
    /// the resulting array of characters.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* shaderc_result_get_bytes(shaderc_compilation_result_t result);

    /// <summary>
    /// Returns the compilation status, indicating whether the compilation succeeded,
    /// or failed due to some reasons, like invalid shader stage or compilation
    /// errors.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial shaderc_compilation_status shaderc_result_get_compilation_status(shaderc_compilation_result_t result);

    /// <summary>
    /// Returns a null-terminated string that contains any error messages generated
    /// during the compilation.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* shaderc_result_get_error_message(shaderc_compilation_result_t result);

    /// <summary>
    /// Returns the number of bytes of the compilation output data in a result
    /// object.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t shaderc_result_get_length(shaderc_compilation_result_t result);

    /// <summary>
    /// Returns the number of errors generated during the compilation.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t shaderc_result_get_num_errors(shaderc_compilation_result_t result);

    /// <summary>
    /// Returns the number of warnings generated during the compilation.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t shaderc_result_get_num_warnings(shaderc_compilation_result_t result);

    /// <summary>
    /// Releases the resources held by the result object. It is invalid to use the
    /// result object for any further operations.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void shaderc_result_release(shaderc_compilation_result_t result);
}
