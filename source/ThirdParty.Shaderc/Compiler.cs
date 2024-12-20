using System.Runtime.InteropServices;
using System.Text;
using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Shaderc;

public sealed unsafe class Compiler : IDisposable
{
    private readonly shaderc_compiler_t handle = PInvoke.shaderc_compiler_initialize();
    private bool disposed;

    ~Compiler() => this.Dispose(false);

    private void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            PInvoke.shaderc_compiler_release(this.handle);

            this.disposed = true;
        }
    }

    public CompilationResult CompileIntoSpv(string sourceText, ShaderKind shaderKind, string inputFileName, string entryPointName, CompilerOptions? additionalOptions = null) =>
        this.CompileIntoSpv(Encoding.UTF8.GetBytes(sourceText), shaderKind, inputFileName, entryPointName, additionalOptions);

    public CompilationResult CompileIntoSpv(byte[] source, ShaderKind shaderKind, string inputFileName, string entryPointName, CompilerOptions? additionalOptions = null)
    {
        fixed (byte* pSourceText     = source)
        fixed (byte* pInputFileName  = Encoding.UTF8.GetBytes(inputFileName))
        fixed (byte* pEntryPointName = Encoding.UTF8.GetBytes(entryPointName))
        {
            var result = PInvoke.shaderc_compile_into_spv(this.handle, pSourceText, (ulong)source.Length, shaderKind, pInputFileName, pEntryPointName, additionalOptions);

            var length = PInvoke.shaderc_result_get_length(result);

            var bytes = new byte[length];

            if (bytes.Length > 0)
            {
                var pBytes = PInvoke.shaderc_result_get_bytes(result);

                Marshal.Copy((nint)pBytes, bytes, 0, bytes.Length);
            }

            var compilationStatus = PInvoke.shaderc_result_get_compilation_status(result);
            var errorMessage      = Marshal.PtrToStringAnsi((nint)PInvoke.shaderc_result_get_error_message(result))!;
            var errors            = PInvoke.shaderc_result_get_num_errors(result);
            var warnings          = PInvoke.shaderc_result_get_num_warnings(result);

            PInvoke.shaderc_result_release(result);

            return new()
            {
                Bytes             = bytes,
                CompilationStatus = compilationStatus,
                ErrorMessage      = errorMessage,
                Errors            = errors,
                Warnings          = warnings,
            };
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
