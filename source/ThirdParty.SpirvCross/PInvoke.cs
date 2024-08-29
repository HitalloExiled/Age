using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Native;

namespace ThirdParty.SpirvCross;

internal static unsafe partial class PInvoke
{
    /// <summary>
    /// Compile IR into a string. *source is owned by the context, and caller must not free it themselves.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_compiler_compile(spvc_compiler compiler, byte** source);

    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_compiler_create_shader_resources(spvc_compiler compiler, spvc_resources *resources);

    [LibraryImport(PLATFORM_PATH)]
    public static partial uint spvc_compiler_get_decoration(spvc_compiler compiler, SpvId id, SpvDecoration decoration);

    /// <summary>
    /// Context is the highest-level API construct.
    /// The context owns all memory allocations made by its child object hierarchy, including various non-opaque structs and strings.
    /// This means that the API user only has to care about one "destroy" call ever when using the C API.
    /// All pointers handed out by the APIs are only valid as long as the context
    /// is alive and spvc_context_release_allocations has not been called.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_context_create(spvc_context* context);

    /// <summary>
    /// Create a compiler backend. Capture mode controls if we construct by copy or move semantics.
    /// It is always recommended to use SPVC_CAPTURE_MODE_TAKE_OWNERSHIP if you only intend to cross-compile the IR once.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_context_create_compiler(spvc_context context, spvc_backend backend, spvc_parsed_ir parsed_ir, spvc_capture_mode mode, spvc_compiler *compiler);

    /// <summary>
    /// Frees all memory allocations and objects associated with the context and its child objects.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial void spvc_context_destroy(spvc_context context);

    /// <summary>
    /// SPIR-V parsing interface. Maps to Parser which then creates a ParsedIR, and that IR is extracted into the handle.
    /// </summary>
    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_context_parse_spirv(spvc_context context, SpvId *spirv, size_t word_count, spvc_parsed_ir *parsed_ir);

    [LibraryImport(PLATFORM_PATH)]
    public static partial void spvc_context_set_error_callback(spvc_context context, spvc_error_callback cb, void *userdata);

    [LibraryImport(PLATFORM_PATH)]
    public static partial spvc_result spvc_resources_get_resource_list_for_type(spvc_resources resources, spvc_resource_type type, spvc_reflected_resource** resource_list, size_t *resource_size);
}
