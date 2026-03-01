using Age.Core.Collections;

namespace ThirdParty.Slang;

public struct TargetDesc()
{
    public SlangCompileTarget               Format;
    public SlangProfileID                   Profile;
    public SlangTargetFlags                 Flags = SlangTargetFlags.GenerateSpirvDirectly;
    public SlangFloatingPointMode           FloatingPointMode;
    public SlangLineDirectiveMode           LineDirectiveMode;
    public bool                             ForceGlslScalarBufferLayout;
    public InlineList4<CompilerOptionEntry> CompilerOptionEntries = [];
}
