using System.Runtime.InteropServices;
using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Shaderc;

public delegate IncludeResult IncludeResolve(string requestedSource, IncludeType type, string requestingSource, ulong includeDepth);

public sealed unsafe class CompilerOptions : IDisposable
{
    private static unsafe void IncludeResultReleaseFn(void* userData, shaderc_include_result* includeResult)
    {
        NativeMemory.Free(includeResult->SourceName);
        NativeMemory.Free(includeResult->Content);
        NativeMemory.Free(includeResult);
    }

    private static unsafe shaderc_include_result* IncludeResolveFn(void* userData, byte* requestedSource, int type, byte* requestingSource, size_t includeDepth)
    {
        var fn = Marshal.GetDelegateForFunctionPointer<IncludeResolve>((nint)userData);

        var result = fn.Invoke(
            Marshal.PtrToStringAnsi((nint)requestedSource)!,
            (IncludeType)type,
            Marshal.PtrToStringAnsi((nint)requestingSource)!,
            includeDepth
        );

        var nativeResult = (shaderc_include_result*)NativeMemory.AllocZeroed((uint)sizeof(shaderc_include_result));
        var sourceName   = (byte*)Marshal.StringToHGlobalAnsi(result.SourceName);
        var content      = (byte*)NativeMemory.Alloc((uint)(sizeof(byte) * result.Content.Length));

        Marshal.Copy(result.Content, 0, (nint)content, result.Content.Length);

        nativeResult->SourceName       = sourceName;
        nativeResult->SourceNameLength = (ulong)result.SourceName.Length;
        nativeResult->Content          = content;
        nativeResult->ContentLength    = (ulong)result.Content.Length;

        return nativeResult;
    }

    private readonly shaderc_compile_options_t handler;

    private bool           disposed;
    private SourceLanguage sourceLanguage;
    private IncludeResolve? includeResolver;

    public SourceLanguage SourceLanguage
    {
        get => this.sourceLanguage;
        set => PInvoke.shaderc_compile_options_set_source_language(this.handler, this.sourceLanguage = value);
    }

    public IncludeResolve? IncludeResolver
    {
        get => this.includeResolver;
        set
        {
            if (value == null)
            {
                PInvoke.shaderc_compile_options_set_include_callbacks(this.handler, null, null, null);
            }
            else
            {
                PInvoke.shaderc_compile_options_set_include_callbacks(this.handler, IncludeResolveFn, IncludeResultReleaseFn, (void*)Marshal.GetFunctionPointerForDelegate(value));
            }

            this.includeResolver = value;
        }
    }

    private CompilerOptions(shaderc_compile_options_t handler) => this.handler = handler;

    public CompilerOptions() : this(PInvoke.shaderc_compile_options_initialize())
    { }

    ~CompilerOptions() => this.Dispose(false);

    private void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            PInvoke.shaderc_compile_options_release(this.handler);
            this.disposed = true;
        }
    }

    public CompilerOptions Clone() =>
        new(PInvoke.shaderc_compile_options_clone(this.handler))
        {
            SourceLanguage  = this.sourceLanguage,
            IncludeResolver = this.IncludeResolver,
        };

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static implicit operator shaderc_compile_options_t(CompilerOptions? compiler) => compiler?.handler ?? default;
}
