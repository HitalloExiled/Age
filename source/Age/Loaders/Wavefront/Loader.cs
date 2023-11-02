using Age.Loaders.Wavefront.Parsers;
using Age.Platform.Abstractions.Interfaces;

namespace Age.Loaders.Wavefront;

public class Loader(IFileSystem fileSystem)
{
    public Data Load(string filepath, ObjParser.Options? options = null)
    {
        using var reader = new StreamReader(fileSystem.File.OpenRead(filepath))!;

        var mtlLoader = new MtlLoader(fileSystem);

        return new ObjParser(filepath, reader, mtlLoader, options).Parse();
    }
}
