using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Enums;

namespace ThirdParty.SpirvCross;

public sealed unsafe class Context : IDisposable
{
    private readonly spvc_context handle;

    private bool    disposed;
    private string? lastError;

    public Context()
    {
        fixed (spvc_context* pHandle = &this.handle)
        {
            this.CheckResult(PInvoke.spvc_context_create(pHandle));
        }

        PInvoke.spvc_context_set_error_callback(this.handle, Marshal.GetFunctionPointerForDelegate(this.ErrorCallback), default);
    }

    ~Context() => this.Dispose(false);

    private void Dispose(bool _)
    {
        if (!this.disposed)
        {
            PInvoke.spvc_context_destroy(this.handle);

            this.disposed = true;
        }
    }

    private unsafe void ErrorCallback(void* userdata, byte* error) =>
        this.lastError = Marshal.PtrToStringAnsi((nint)error);

    internal void CheckResult(spvc_result result)
    {
        if (result != Result.Success)
        {
            throw new Exception($"Error: {this.lastError}");
        }
    }

    public ParsedSpirv ParseSpirv(byte[] spirv)
    {
        spvc_parsed_ir parsedIr;

        var spirvSpan = MemoryMarshal.Cast<byte, uint>(spirv);

        fixed (uint* pSpirv = spirvSpan)
        {
            this.CheckResult(PInvoke.spvc_context_parse_spirv(this.handle, pSpirv, (ulong)spirvSpan.Length, &parsedIr));
        }

        return parsedIr;
    }

    public Compiler CreateCompiler(Backend backend, ParsedSpirv parsedSpirv, CaptureMode captureMode)
    {
        spvc_compiler compiler;

        this.CheckResult(PInvoke.spvc_context_create_compiler(this.handle, backend, parsedSpirv, captureMode, &compiler));

        return new(compiler, this);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
