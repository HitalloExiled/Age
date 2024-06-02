using Age.Resources.Loaders.Wavefront.Parsers;

namespace Age.Resources.Loaders.Wavefront;

public static class Loader
{
    public static Data Load(string filepath, ObjParser.Options? options = null)
    {
        using var reader = new StreamReader(File.OpenRead(filepath))!;

        var mtlLoader = new MtlLoader();

        return new ObjParser(filepath, reader, mtlLoader, options).Parse();
    }
}
