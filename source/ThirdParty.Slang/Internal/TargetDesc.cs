namespace ThirdParty.Slang.Internal;

public unsafe struct TargetDesc()
{
    public size_t                 StructureSize = (size_t)sizeof(TargetDesc);
    public SlangCompileTarget     Format;
    public SlangProfileID         Profile;
    public SlangTargetFlags       Flags =  SlangTargetFlags.GenerateSpirvDirectly;
    public SlangFloatingPointMode FloatingPointMode;
    public SlangLineDirectiveMode LineDirectiveMode;
    public bool                   ForceGlslScalarBufferLayout;
    public CompilerOptionEntry*   CompilerOptionEntries;
    public uint32_t               CompilerOptionEntryCount;
}
