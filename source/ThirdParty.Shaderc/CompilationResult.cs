using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Shaderc;

public unsafe class CompilationResult
{
    public required byte[]            Bytes             { get; init; }
    public required CompilationStatus CompilationStatus { get; init; }
    public required string            ErrorMessage      { get; init; }
    public required ulong             Errors            { get; init; }
    public required ulong             Warnings          { get; init; }
}
