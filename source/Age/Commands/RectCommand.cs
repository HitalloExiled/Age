using Age.Numerics;
using Age.Shaders;
using Age.Resources;

namespace Age.Commands;

public record RectCommand : Command
{
    public CanvasShader.Border  Border     { get; set; }
    public Color                Color      { get; set; }
    public CanvasShader.Flags   Flags      { get; set; }
    public Size<float>          Size       { get; set; }
    public TextureMap           TextureMap { get; set; } = TextureMap.Default;
    public Transform2D          Transform  { get; set; } = Transform2D.Identity;

    public Rect<float> GetAffineRect() => new(this.Size, this.Transform.Position.ToPoint());

    public override void Reset()
    {
        base.Reset();

        this.Border     = default;
        this.Color      = default;
        this.Flags      = default;
        this.Size       = default;
        this.TextureMap = TextureMap.Default;
        this.Transform  = Transform2D.Identity;
    }
}
