using ThirdParty.Slang;

namespace ThirdParty.Tests.Slang;

public class SlangSessionTest
{
    [Fact]
    public void CreateSlangSession()
    {
        var session = new SlangSession();

        session.Dispose();

        Assert.True(true);
    }
}
