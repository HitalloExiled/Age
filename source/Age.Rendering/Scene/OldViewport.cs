using Age.Numerics;
using Age.Rendering.Commands;
using Age.Rendering.Resources;

namespace Age.Rendering.Scene;

public class OldViewport : Node2D
{
    public override string NodeName => nameof(OldViewport);

    private Texture?   texture;
    private Size<uint> size;

    public Texture? Texture
    {
        get => this.texture;
        internal set => this.SetTexture(value);
    }

    public Size<uint> Size
    {
        get => this.size;
        set => this.Set(ref this.size, value);
    }

    private void SetTexture(Texture? value)
    {
        this.Set(ref this.texture, value);

        if (value == null)
        {
            this.Commands.Clear();
        }
        else
        {
            RectCommand command;

            if (this.Commands.Count == 0)
            {
                this.Commands.Add(command = new RectCommand());
            }
            else
            {
                command = (RectCommand)this.Commands[0];
            }

            command.Rect           = new Rect<float>(value.Image.Extent.Width, value.Image.Extent.Height, 0, 0);
            command.SampledTexture = new(value, Container.Singleton.TextureStorage.DefaultSampler, UVRect.Normalized);
        }
    }

    protected override void OnTransformChanged()
    {
        if (this.IsConnected)
        {
            this.Tree.IsDirty = true;
        }
    }
}
