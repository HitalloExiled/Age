namespace Age.Tests.Loaders.Wavefront.Parsers;

public abstract class ParserTest
{
    protected static string GetFullPath(string value) =>
        Path.GetFullPath(Path.Join(Directory.GetCurrentDirectory(), value));
}
