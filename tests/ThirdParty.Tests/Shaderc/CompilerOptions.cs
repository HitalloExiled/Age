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

        using var clone = compilerOptions.Clone();

        Assert.Equal(compilerOptions.SourceLanguage, clone.SourceLanguage);
    }
}
