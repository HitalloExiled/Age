using Age.Commands;
using Age.Numerics;
using Age.Passes;
using Age.Scenes;

namespace Age.Elements;

public sealed class EmbeddedViewport : Layoutable
{
    public SubViewport Viewport { get; }

    public override string NodeName => nameof(EmbeddedViewport);

    internal override bool IsParentDependent => false;

    public EmbeddedViewport(SubViewport viewport)
    {
        this.Viewport = viewport;

        this.AllocateCommands(2, CommandPool.RectCommand);
        this.UpdateCommands();

        this.Viewport.Resized += this.UpdateCommands;
    }

    private protected override void OnDisposedInternal()
    {
        base.OnDisposedInternal();

        this.ReleaseCommands(2, CommandPool.RectCommand);

        this.Viewport.Resized -= this.UpdateCommands;
    }

    private void UpdateCommands()
    {
        var color = (RectCommand)this.Commands[0];

        color.Size          = this.Viewport.Size;
        color.CommandFilter = CommandFilter.Color;
        color.TextureMap    = new(this.Viewport.RenderTarget.ColorAttachments[0].Texture, UVRect.Normalized);

        var encodeCompositeRenderPass = this.Viewport.RenderGraph.GetNode<EncodeCompositeRenderPass>();

        var encode = (RectCommand)this.Commands[1];

        encode.Size          = this.Viewport.Size;
        encode.CommandFilter = CommandFilter.Encode;
        encode.TextureMap    = new(encodeCompositeRenderPass.Output!, UVRect.Normalized);
        encode.Flags         = Shaders.Geometry2DShader.Flags.SamplerAsEncode;

        this.MakeDirty();
    }

    internal override void UpdateLayout() =>
        this.Boundings = this.Viewport.Size;
}
