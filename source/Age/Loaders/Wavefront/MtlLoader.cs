using Age.Loaders.Wavefront.Parsers;
using Age.Platform.Abstractions.Interfaces;

namespace Age.Loaders.Wavefront;

public class MtlLoader(IFileSystem fileSystem)
{
    public virtual IList<Material> Load(string filepath)
    {
        using var reader = new StreamReader(fileSystem.File.OpenRead(filepath))!;

        return new MtlParser(filepath, reader).Parse();
    }

    public virtual bool TryLoad(string filepath, out IList<Material> materials)
    {
        try
        {
            materials = this.Load(filepath);

            return true;
        }
        catch
        {
            materials = [];

            return false;
        }
    }
}
