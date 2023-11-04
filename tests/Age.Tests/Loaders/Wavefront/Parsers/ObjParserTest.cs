using System.Text;
using Age.Resources.Loaders.Wavefront;
using Age.Resources.Loaders.Wavefront.Exceptions;
using Age.Resources.Loaders.Wavefront.Parsers;
using Age.Platform.Abstractions.Interfaces;

namespace Age.Tests.Loaders.Wavefront.Parsers;

public partial class ObjParserTest : ParserTest
{
    private static readonly IFileSystem fileSystem = new Mock<IFileSystem>().Object;

    [Theory]
    [MemberData(nameof(Scenarios.Valid), MemberType = typeof(Scenarios))]
    public void ValidScenarios(ValidScenario scenario)
    {
        if (scenario.Skip)
        {
            return;
        }

        var mtlLoaderMock = new Mock<MtlLoader>(fileSystem);

        void tryLoad(string _, out IList<Material> materials) => materials = scenario.Materials;

        mtlLoaderMock.Setup(x => x.TryLoad(It.IsAny<string>(), out It.Ref<IList<Material>>.IsAny))
            .Callback(tryLoad)
            .Returns(scenario.Materials.Length > 0);

        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(scenario.Source)));
        var parser       = new ObjParser(GetFullPath("test_object.obj"), reader, mtlLoaderMock.Object, scenario.Options);

        var expected = scenario.Expected;
        var actual   = parser.Parse();

        Assert.Equivalent(expected, actual, true);
    }

    [Theory]
    [MemberData(nameof(Scenarios.Invalid), MemberType = typeof(Scenarios))]
    public void InvalidScenarios(InvalidScenario scenario)
    {
        if (scenario.Skip)
        {
            return;
        }

        using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(scenario.Source)));
        var parser       = new ObjParser(GetFullPath("test_object.obj"), reader, new Mock<MtlLoader>(fileSystem).Object);

        var expected = scenario.Expected;
        var actual   = Assert.Throws<ParseException>(parser.Parse);

        Assert.Equal(expected.Message, actual.Message);
        Assert.Equal(expected.Column,  actual.Column);
        Assert.Equal(expected.Index,   actual.Index);
        Assert.Equal(expected.Line,    actual.Line);
    }
}
