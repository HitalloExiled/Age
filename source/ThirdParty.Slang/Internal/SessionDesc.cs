namespace ThirdParty.Slang.Internal;

internal unsafe struct SessionDesc()
{
    public size_t                 StructureSize = (size_t)sizeof(SessionDesc);
    public TargetDesc*            Targets;
    public SlangInt               TargetCount;
    public SessionFlags           Flags;
    public SlangMatrixLayoutMode  DefaultMatrixLayoutMode = SlangMatrixLayoutMode.RowMajor;
    public byte**                 SearchPaths;
    public SlangInt               SearchPathCount;
    public PreprocessorMacroDesc* PreprocessorMacros;
    public SlangInt               PreprocessorMacroCount;
    public ISlangFileSystem*      FileSystem;
    public bool                   EnableEffectAnnotations;
    public bool                   AllowGLSLSyntax;
    public CompilerOptionEntry*   CompilerOptionEntries;
    public uint32_t               CompilerOptionEntryCount;
    public bool                   SkipSPIRVValidation;
}
