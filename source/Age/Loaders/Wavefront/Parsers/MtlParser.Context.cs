namespace Age.Loaders.Wavefront.Parsers;

public partial class MtlParser
{
    public class Context
    {
        private readonly List<Material> materials = [];

        public Material CurrentMaterial { get; private set; } = new("");

        public IList<Material> GetMaterials() => this.materials;

        public void NewMaterial(string name) =>
            this.materials.Add(this.CurrentMaterial = new(name));
    }
}
