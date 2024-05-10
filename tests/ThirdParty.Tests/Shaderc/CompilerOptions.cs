using ThirdParty.Shaderc;
using ThirdParty.Shaderc.Enums;

namespace ThirdParty.Tests.Shaderc;

public class CompilerOptionsTest
{
    [Fact]
    public void Instanciate()
    {
        using var compilerOptions      = new CompilerOptions();
        compilerOptions.SourceLanguage = SourceLanguage.Hlsl;

        compilerOptions.IncludeResolver = (string requestedSource, IncludeType type, string requestingSource, ulong includeDepth) =>
        {
            return default;
        };

        using var clone = compilerOptions.Clone();

        Assert.Equal(compilerOptions.SourceLanguage,  clone.SourceLanguage);
        Assert.Equal(compilerOptions.IncludeResolver, clone.IncludeResolver);
    }
}
