using Age.Numerics;
using Age.Shaders;
using Age.Resources;

namespace Age.Commands;

public record RectCommand : Command
{
    public CanvasShader.Border  Border        { get; set; }
    public Color                Color         { get; set; }
    public CanvasShader.Flags   Flags         { get; set; }
    public MappedTexture        MappedTexture { get; set; } = MappedTexture.Default;
    public Rect<float>          Rect          { get; set; }
}
