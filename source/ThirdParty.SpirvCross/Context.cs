using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Enums;

namespace ThirdParty.SpirvCross;

public unsafe class Context : IDisposable
{
    private readonly spvc_context handler;

    private bool    disposed;
    private string? lastError;

    public Context()
    {
        fixed (spvc_context* pHandler = &this.handler)
        {
            this.CheckResult(PInvoke.spvc_context_create(pHandler));
        }

        PInvoke.spvc_context_set_error_callback(this.handler, Marshal.GetFunctionPointerForDelegate(this.ErrorCallback), default);
    }

    ~Context() => this.Dispose(false);

    internal void CheckResult(spvc_result result)
    {
        if (result != Result.Success)
        {
            throw new Exception($"Error: {this.lastError}");
        }
    }

    private unsafe void ErrorCallback(void* userdata, byte* error) =>
        this.lastError = Marshal.PtrToStringAnsi((nint)error);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            PInvoke.spvc_context_destroy(this.handler);

            this.disposed = true;
        }
    }

    public ParsedSpirv ParseSpirv(byte[] spirv)
    {
        spvc_parsed_ir parsedIr;

        var spirvSpan = MemoryMarshal.Cast<byte, uint>(spirv);

        fixed (uint* pSpirv = spirvSpan)
        {
            this.CheckResult(PInvoke.spvc_context_parse_spirv(this.handler, pSpirv, (ulong)spirvSpan.Length, &parsedIr));
        }

        return parsedIr;
    }

    public Compiler CreateCompiler(Backend backend, ParsedSpirv parsedSpirv, CaptureMode captureMode)
    {
        spvc_compiler compiler;

        this.CheckResult(PInvoke.spvc_context_create_compiler(this.handler, backend, parsedSpirv, captureMode, &compiler));

        return new(compiler, this);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
}
