using Age.Resources.Loaders.Wavefront.Parsers;

namespace Age.Resources.Loaders.Wavefront;

public class MtlLoader
{
    public virtual IList<Material> Load(string filepath)
    {
        using var reader = new StreamReader(File.OpenRead(filepath));

        return new MtlParser(filepath, reader).Parse();
    }

    public virtual bool TryLoad(string filepath, out IList<Material> materials)
    {
        if (File.Exists(filepath))
        {
            materials = this.Load(filepath);

            return true;
        }
        else
        {
            materials = [];

            return false;
        }
    }
}
