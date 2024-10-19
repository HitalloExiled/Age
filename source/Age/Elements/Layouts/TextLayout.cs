using Age.Commands;
using Age.Services;

namespace Age.Elements.Layouts;

internal class TextLayout(TextNode target): Layout
{
    private string? text;
    private StencilLayer? stencilLayer;

    public override TextNode   Target => target;
    public override BoxLayout? Parent => target.ParentElement?.Layout;

    public override StencilLayer? StencilLayer
    {
        get => this.stencilLayer;
        set
        {
            if (this.stencilLayer != value)
            {
                this.stencilLayer = value;

                foreach (var command in target.Commands)
                {
                    command.StencilLayer = this.stencilLayer;
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

    public void IndexChanged()
    {
        for (var i = 0; i < this.Target.Commands.Count; i++)
        {
            var command = (RectCommand)this.Target.Commands[i];

            command.ObjectId = (uint)((this.Target.Index + 1u) | (i + 1u) << 16);
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
