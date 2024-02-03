using Age.Numerics;
using Normal   = Age.Numerics.Vector3<float>;
using TexCoord = Age.Numerics.Vector3<float>;
using Vertex   = Age.Numerics.Vector4<float>;

namespace Age.Resources.Loaders.Wavefront.Parsers;

public partial class ObjParser
{
    private record Context
    {
        private readonly Dictionary<Color, int>  colorIndices          = [];
        private readonly List<Color>             colors                = [];
        private readonly Group                   global;
        private readonly List<Group>             groups                = [];
        private readonly List<Material>          materials             = [];
        private readonly Dictionary<string, int> materialIndices       = [];
        private readonly List<Object>            objects               = [];
        private readonly Dictionary<int, int>    texCoordIndices       = [];
        private readonly Dictionary<int, int>    vertexColorIndex      = [];
        private readonly Dictionary<int, int>    vertexNormalIndices   = [];

        public List<Normal>   Normals   { get; } = [];
        public List<TexCoord> TexCoords { get; } = [];
        public List<Vertex>   Vertices  { get; } = [];

        public Group  CurrentGroup    { get; private set; }
        public int    CurrentMaterial { get; private set; } = -1;
        public Object CurrentObject   { get; private set; }
        public int    GroupIndex      { get; private set; } = -1;

        public bool ShadedSmooth { get; set; }

        public Context(string name)
        {
            this.CurrentGroup  = this.global = new(name);
            this.CurrentObject = new(name);
        }

        private void EndCurrentObject()
        {
            if (this.objects.Count > 0 && this.CurrentObject.Mesh.Faces.Count == 0 && this.CurrentObject.Mesh.Lines.Count == 0)
            {
                this.objects.RemoveAt(this.objects.Count - 1);
            }
        }

        public void AddFace(Face face)
        {
            this.CurrentObject.Mesh.Faces.Add(face);

            foreach (var vertexData in face.Indices)
            {
                this.CurrentObject.Mesh.Vertices.Remove(vertexData.Index);
            }

            this.CurrentGroup.Faces.Add(face);
        }

        public void AddLine(Line line)
        {
            this.CurrentObject.Mesh.Lines.Add(line);
            this.CurrentObject.Mesh.Vertices.Remove(line.Start);
            this.CurrentObject.Mesh.Vertices.Remove(line.End);
            this.CurrentGroup.Lines.Add(line);
        }

        public void AddMaterials(IEnumerable<Material> materials)
        {
            foreach (var material in materials)
            {
                if (!this.materialIndices.ContainsKey(material.Name))
                {
                    this.materialIndices[material.Name] = this.materials.Count;

                    this.materials.Add(material);
                }
            }
        }

        public void AddNormal(Normal normal) =>
            this.Normals.Add(normal);

        public void AddTexCoord(TexCoord texCoord) =>
            this.TexCoords.Add(texCoord);

        public void AddVertexWithColor(Vertex vertex, Color? color)
        {
            this.CurrentObject.Mesh.Vertices.Add(this.Vertices.Count);
            this.CurrentGroup.Vertices.Add(this.Vertices.Count);

            this.Vertices.Add(vertex);

            if (color.HasValue)
            {
                if (!this.colorIndices.TryGetValue(color.Value, out var index))
                {
                    this.colorIndices[color.Value] = index = this.colors.Count;
                    this.colors.Add(color.Value);
                }

                this.vertexColorIndex[this.Vertices.Count - 1] = index;
            }
        }

        public int GetColorIndex(int vertexIndex) =>
            this.vertexColorIndex.TryGetValue(vertexIndex, out var colorIndex) ? colorIndex : -1;

        public void NewGroup(string name)
        {
            if (name != "off")
            {
                this.CurrentGroup = new(name);
                this.GroupIndex   = this.groups.Count;

                this.groups.Add(this.CurrentGroup);
            }
            else
            {
                this.CurrentGroup = this.global;
                this.GroupIndex   = -1;
            }
        }

        public void NewObject(string name)
        {
            this.EndCurrentObject();

            this.objects.Add(this.CurrentObject = new(name));
        }

        public void SetCurrentMaterial(string name) =>
            this.CurrentMaterial = this.materialIndices.TryGetValue(name, out var index) ? index : -1;

        public void SetTexCoordIndex(int vertexIndex, int texCoordIndex) =>
            this.texCoordIndices[vertexIndex] = texCoordIndex;

        public void SetVertexNormalIndex(int vertexIndex, int normalIndex) =>
            this.vertexNormalIndices[vertexIndex] = normalIndex;

        public Data ToData()
        {
            this.EndCurrentObject();

            return new()
            {
                Attributes = new()
                {
                    Colors    = this.colors,
                    Groups    = this.groups,
                    Materials = this.materials,
                    Normals   = this.Normals,
                    TexCoords = this.TexCoords,
                    Vertices  = this.Vertices,
                },
                Objects = this.objects.Count == 0 ? [this.CurrentObject] : this.objects,
            };
        }
    }
}
