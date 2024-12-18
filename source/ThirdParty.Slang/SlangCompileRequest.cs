using System.Runtime.InteropServices;
using System.Text;

namespace ThirdParty.Slang;

public unsafe class SlangCompileRequest(SlangSession session) : DisposableManagedSlang<SlangCompileRequest>(PInvoke.spCreateCompileRequest(session.Handle))
{
    public SlangSession Session { get; } = session;

    protected override void Disposed(bool disposing) =>
        PInvoke.spDestroyCompileRequest(this.Handle);

    public int AddCodeGenTarget(SlangCompileTarget target) =>
        PInvoke.spAddCodeGenTarget(this.Handle, target);

    public void AddCodeGenTarget(string path)
    {
        fixed (byte* pPath = Encoding.UTF8.GetBytes(path))
        {
            PInvoke.spAddSearchPath(this.Handle, pPath);
        }
    }

    public int AddEntryPoint(int translationUnitIndex, string name, SlangStage stage) =>
        this.AddEntryPoint(translationUnitIndex, name.AsSpan(), stage);

    public int AddEntryPoint(int translationUnitIndex, ReadOnlySpan<char> name, SlangStage stage)
    {
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
        Span<byte> pathBuffer = stackalloc byte[Encoding.UTF8.GetByteCount(path)];

        Encoding.UTF8.GetBytes(path, pathBuffer);

        fixed (byte* pPath   = pathBuffer)
        fixed (byte* pSource = source)
        {
            PInvoke.spAddTranslationUnitSourceString(this.Handle, translationUnitIndex, pPath, pSource);
        }
    }

    public int GetDependencyFileCount() =>
        PInvoke.spGetDependencyFileCount(this.Handle);

    public string GetDependencyFilePath(int index) =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spGetDependencyFilePath(this.Handle, index))!;

    public string[] GetDependencyFiles()
    {
        var dependencies = new string[this.GetDependencyFileCount()];

        for (var i = 0; i < dependencies.Length; i++)
        {
            dependencies[i] = this.GetDependencyFilePath(i);
        }

        return dependencies;
    }

    public SlangReflection GetReflection() =>
        new(this);

    public void SetCodeGenTarget(SlangCompileTarget target) =>
        PInvoke.spSetCodeGenTarget(this.Handle, target);

    public void SetTargetProfile(int targetIndex, SlangProfileID profile) =>
        PInvoke.spSetTargetProfile(this.Handle, targetIndex, profile);

    public void SetOptimizationLevel(SlangOptimizationLevel level) =>
        PInvoke.spSetOptimizationLevel(this.Handle, level);

    public void SetDebugInfoLevel(SlangDebugInfoLevel level) =>
        PInvoke.spSetDebugInfoLevel(this.Handle, level);

    public bool Compile() =>
        PInvoke.spCompile(this.Handle) >= 0;

    public string? GetDiagnosticOutput() =>
        Marshal.PtrToStringAnsi((nint)PInvoke.spGetDiagnosticOutput(this.Handle));

    public byte[] GetEntryPointCode(int entryPointIndex)
    {
        var size = 0UL;

        var result = PInvoke.spGetEntryPointCode(this.Handle, entryPointIndex, &size);

        var buffer = new Span<byte>(result, (int)size);

        return buffer.ToArray();
    }
}
