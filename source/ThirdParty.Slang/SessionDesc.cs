using Age.Core.Collections;

namespace ThirdParty.Slang;

public struct SessionDesc()
{
    public InlineList4<TargetDesc>             Targets;
    public SessionFlags                        Flags;
    public SlangMatrixLayoutMode               DefaultMatrixLayoutMode = SlangMatrixLayoutMode.RowMajor;
    public InlineList4<string>                 SearchPaths;
    public InlineList4<PreprocessorMacroDesc>  PreprocessorMacros = [];
    public SlangFileSystem?                    FileSystem;
    public bool                                EnableEffectAnnotations;
    public bool                                AllowGLSLSyntax;
    public InlineList4<CompilerOptionEntry>    CompilerOptionEntries = [];
    public bool                                SkipSPIRVValidation;
}
