using System.Runtime.CompilerServices;
using ThirdParty.Slang;

namespace ThirdParty.Tests.Slang;

public class GlobalSessionTest
{
    public static string GetFileLocation([CallerFilePath] string? callerFilePath = default) =>
        callerFilePath!;

    [Fact]
    public void CreateGlobalSession()
    {
        using var globalSession = new GlobalSession(0);

        using var session = globalSession.CreateSession(default);

        Assert.True(true);
    }
}
