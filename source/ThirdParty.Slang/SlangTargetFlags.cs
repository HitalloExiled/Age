namespace ThirdParty.Slang;

[Flags]
public enum SlangTargetFlags : uint
{
    ParameterBlocksUseRegisterSpaces = 1 << 4,
    GenerateWholeProgram             = 1 << 8,
    DumpIr                           = 1 << 9,
    GenerateSpirvDirectly            = 1 << 10,
};
