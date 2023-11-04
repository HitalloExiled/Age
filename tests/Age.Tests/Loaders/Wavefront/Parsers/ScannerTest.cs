using System.Text;
using Age.Resources.Loaders.Wavefront.Exceptions;
using Age.Resources.Loaders.Wavefront.Parsers;

namespace Age.Tests.Loaders.Wavefront.Parsers;

public partial class ScannerTest
{
    [Theory]
    [MemberData(nameof(Scenarios.Valid), MemberType= typeof(Scenarios))]
    public void ValidScenarios(ValidScenario scenario)
    {
        if (scenario.Skip)
        {
            return;
        }

        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(scenario.Source)));
        var scanner      = new Scanner(reader);

        var expected = scenario.Expected;
        var actual   = scanner.NextToken(scenario.Unrestricted);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Scenarios.Invalid), MemberType= typeof(Scenarios))]
    public void InvalidScenarios(InvalidScenario scenario)
    {
        if (scenario.Skip)
        {
            return;
        }

        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(scenario.Source)));
        var scanner      = new Scanner(reader);

        var expected = scenario.Expected;
        var actual   = Assert.Throws<SyntaxErrorException>(() => scanner.NextToken(false));

        Assert.Equal(expected.Message, actual.Message);
        Assert.Equal(expected.Column,  actual.Column);
        Assert.Equal(expected.Index,   actual.Index);
        Assert.Equal(expected.Line,    actual.Line);
    }
}
