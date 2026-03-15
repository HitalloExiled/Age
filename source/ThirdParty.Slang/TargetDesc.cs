namespace ThirdParty.Slang;

public unsafe struct TargetDesc()
{
#pragma warning disable RCS1213, IDE0052 // Remove unused member declaration
    private readonly size_t structureSize = (size_t)sizeof(TargetDesc);
#pragma warning restore RCS1213, IDE0052

    public SlangCompileTarget     Format;
    public SlangProfileID         Profile;
    public SlangTargetFlags       Flags =  SlangTargetFlags.GenerateSpirvDirectly;
    public SlangFloatingPointMode FloatingPointMode;
    public SlangLineDirectiveMode LineDirectiveMode;
    public bool                   ForceGlslScalarBufferLayout;
    public CompilerOptionEntry*   CompilerOptionEntries;
    public uint32_t               CompilerOptionEntryCount;
}
