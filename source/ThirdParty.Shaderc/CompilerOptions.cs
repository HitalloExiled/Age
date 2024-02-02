using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Shaderc;

public unsafe class CompilerOptions : IDisposable
{
    private readonly shaderc_compile_options_t handler;

    private bool           disposed;
    private SourceLanguage sourceLanguage;


    public SourceLanguage SourceLanguage
    {
        get => this.sourceLanguage;
        set => PInvoke.shaderc_compile_options_set_source_language(this.handler, this.sourceLanguage = value);
    }

    private CompilerOptions(shaderc_compile_options_t handler) =>
        this.handler = handler;

    public CompilerOptions() : this(PInvoke.shaderc_compile_options_initialize())
    { }

    ~CompilerOptions() => this.Dispose(false);

    protected virtual void Dispose(bool disposing)
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
            SourceLanguage = this.sourceLanguage,
        };

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static implicit operator shaderc_compile_options_t(CompilerOptions? compiler) => compiler?.handler ?? default;
}
