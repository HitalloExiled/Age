using Age.Numerics;
using Age.Shaders;
using Age.Resources;

namespace Age.Commands;

public record RectCommand : Command2D
{
    public CanvasShader.Border  Border     { get; set; }
    public Color                Color      { get; set; }
    public CanvasShader.Flags   Flags      { get; set; }
    public TextureMap           TextureMap { get; set; } = TextureMap.Default;

    public override void Reset()
    {
        base.Reset();

        this.Border     = default;
        this.Color      = default;
        this.Flags      = default;
        this.Size       = default;
        this.TextureMap = TextureMap.Default;
    }
}
