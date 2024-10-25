using Age.Commands;
using Age.Core.Extensions;
using Age.Rendering.Shaders.Canvas;
using Age.Services;

namespace Age.Elements.Layouts;

internal class TextLayout(TextNode target): Layout
{
    private string? text;

    public override TextNode   Target => target;
    public override BoxLayout? Parent => target.ParentElement?.Layout;

    public override StencilLayer? StencilLayer
    {
        get => base.StencilLayer;
        set
        {
            if (base.StencilLayer != value)
            {
                base.StencilLayer = value;

                foreach (var command in target.Commands)
                {
                    command.StencilLayer = value;
                }
            }
        }
    }

    public string? Text
    {
        get => this.text;
        internal set
        {
            if (value != this.text)
            {
                this.text = value;

                this.RequestUpdate();
            }
        }
    }

    public void TargetIndexed()
    {
        var parentIndex = this.Target.Index + 1;
        var i           = 1;

        foreach (var command in this.Target.Commands.AsSpan())
        {
            if (command.Variant.HasFlag(CanvasShader.Variant.Index))
            {
                command.ObjectId = (uint)((i << 12) | parentIndex);
                i++;
            }
        }
    }

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            if (string.IsNullOrEmpty(this.text))
            {
                this.Target.Commands.Clear();

                this.BaseLine   = -1;
                this.LineHeight = 0;
                this.Size       = default;
            }
            else
            {
                var info = TextService.Singleton.DrawText(this.Target, this.text);

                this.BaseLine   = -info.Start;
                this.LineHeight = info.LineHeight;
                this.Size       = info.Boundings;
            }

            this.HasPendingUpdate = false;
        }
    }
}
