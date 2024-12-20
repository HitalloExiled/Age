using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangCompileRequest(SlangSession session) : DisposableManagedSlang<SlangCompileRequest>(PInvoke.spCreateCompileRequest(session.Handle))
{
    public SlangSession Session { get; } = session;

    public bool Compiled { get; private set; }

    public int DependencyFileCount
    {
        get
        {
            this.AssertCompiled();
            this.AssertNotDisposed();

            return PInvoke.spGetDependencyFileCount(this.Handle);
        }
    }

    private void AssertCompiled()
    {
        if (!this.Compiled)
        {
            throw new InvalidOperationException("Request not compiled");
        }
    }

    protected override void Disposed(bool disposing) =>
        PInvoke.spDestroyCompileRequest(this.Handle);

    public int AddCodeGenTarget(SlangCompileTarget target)
    {
        this.AssertNotDisposed();

        return PInvoke.spAddCodeGenTarget(this.Handle, target);
    }

    public void AddCodeGenTarget(string path)
    {
        this.AssertNotDisposed();

        fixed (byte* pPath = Encoding.UTF8.GetBytes(path))
        {
            PInvoke.spAddSearchPath(this.Handle, pPath);
        }
    }

    public int AddEntryPoint(int translationUnitIndex, string name, SlangStage stage) =>
        this.AddEntryPoint(translationUnitIndex, name.AsSpan(), stage);

    public int AddEntryPoint(int translationUnitIndex, ReadOnlySpan<char> name, SlangStage stage)
    {
        this.AssertNotDisposed();

        Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(name)];

        Encoding.UTF8.GetBytes(name, buffer);

        fixed (byte* pName = buffer)
        {
            return PInvoke.spAddEntryPoint(this.Handle, translationUnitIndex, pName, stage);
        }
    }

    public void AddSearchPath(string path) =>
        this.AddSearchPath(path.AsSpan());

    public void AddSearchPath(ReadOnlySpan<char> path)
    {
        this.AssertNotDisposed();

        Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(path)];

        Encoding.UTF8.GetBytes(path, buffer);

        fixed (byte* pPath = buffer)
        {
            PInvoke.spAddSearchPath(this.Handle, pPath);
        }
    }

    public int AddTranslationUnit(SlangSourceLanguage language, string? name) =>
        this.AddTranslationUnit(language, name.AsSpan());

    public int AddTranslationUnit(SlangSourceLanguage language, ReadOnlySpan<char> name)
    {
        this.AssertNotDisposed();

        Span<byte> buffer = stackalloc byte[Encoding.UTF8.GetByteCount(name)];

        Encoding.UTF8.GetBytes(name, buffer);

        fixed (byte* inName = buffer)
        {
            return PInvoke.spAddTranslationUnit(this.Handle, language, inName);
        }
    }

    public void AddTranslationUnitSourceFile(int translationUnitIndex, string path) =>
        this.AddTranslationUnitSourceFile(translationUnitIndex, path.AsSpan());

    public void AddTranslationUnitSourceFile(int translationUnitIndex, ReadOnlySpan<char> path)
    {
        this.AssertNotDisposed();

        Span<byte> pathBuffer = stackalloc byte[Encoding.UTF8.GetByteCount(path)];

        Encoding.UTF8.GetBytes(path, pathBuffer);

        fixed (byte* pPath   = pathBuffer)
        {
            PInvoke.spAddTranslationUnitSourceFile(this.Handle, translationUnitIndex, pPath);
        }
    }

    public void AddTranslationUnitSourceString(int translationUnitIndex, string path, string source) =>
        this.AddTranslationUnitSourceString(translationUnitIndex, path.AsSpan(), Encoding.UTF8.GetBytes(source));

    public void AddTranslationUnitSourceString(int translationUnitIndex, ReadOnlySpan<char> path, ReadOnlySpan<byte> source)
    {
        this.AssertNotDisposed();

        Span<byte> pathBuffer = stackalloc byte[Encoding.UTF8.GetByteCount(path)];

        Encoding.UTF8.GetBytes(path, pathBuffer);

        fixed (byte* pPath   = pathBuffer)
        fixed (byte* pSource = source)
        {
            PInvoke.spAddTranslationUnitSourceString(this.Handle, translationUnitIndex, pPath, pSource);
        }
    }

    public string GetDependencyFilePath(int index)
    {
        this.AssertCompiled();
        this.AssertNotDisposed();

        return Marshal.PtrToStringAnsi((nint)PInvoke.spGetDependencyFilePath(this.Handle, index))!;
    }

    public string[] GetDependencyFiles()
    {
        var dependencies = new string[this.DependencyFileCount];

        for (var i = 0; i < dependencies.Length; i++)
        {
            dependencies[i] = this.GetDependencyFilePath(i);
        }

        return dependencies;
    }

    public SlangReflection GetReflection()
    {
        this.AssertCompiled();
        this.AssertNotDisposed();

        return new(this);
    }

    public void SetCodeGenTarget(SlangCompileTarget target)
    {
        this.AssertNotDisposed();

        PInvoke.spSetCodeGenTarget(this.Handle, target);
    }

    public void SetTargetProfile(int targetIndex, SlangProfileID profile)
    {
        this.AssertNotDisposed();

        PInvoke.spSetTargetProfile(this.Handle, targetIndex, profile);
    }

    public void SetOptimizationLevel(SlangOptimizationLevel level)
    {
        this.AssertNotDisposed();

        PInvoke.spSetOptimizationLevel(this.Handle, level);
    }

    public void SetDebugInfoLevel(SlangDebugInfoLevel level)
    {
        this.AssertNotDisposed();

        PInvoke.spSetDebugInfoLevel(this.Handle, level);
    }

    public bool Compile()
    {
        this.AssertNotDisposed();

        return this.Compiled = PInvoke.spCompile(this.Handle) >= 0;
    }

    public string? GetDiagnosticOutput()
    {
        this.AssertNotDisposed();

        return Marshal.PtrToStringAnsi((nint)PInvoke.spGetDiagnosticOutput(this.Handle));
    }

    public byte[] GetEntryPointCode(int entryPointIndex)
    {
        this.AssertCompiled();
        this.AssertNotDisposed();

        var size = 0UL;

        var result = PInvoke.spGetEntryPointCode(this.Handle, entryPointIndex, &size);

        var buffer = new Span<byte>(result, (int)size);

        return buffer.ToArray();
    }
}
