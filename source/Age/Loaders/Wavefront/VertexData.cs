namespace Age.Loaders.Wavefront;

public record VertexData
{
    public int ColorIndex    { get; } = -1;
    public int Index         { get; } = -1;
    public int NormalIndex   { get; } = -1;
    public int TexCoordIndex { get; } = -1;

    public VertexData(int index, int texCoordIndex, int normalIndex, int colorIndex)
    {
        this.Index         = index;
        this.TexCoordIndex = texCoordIndex;
        this.NormalIndex   = normalIndex;
        this.ColorIndex    = colorIndex;
    }
}
