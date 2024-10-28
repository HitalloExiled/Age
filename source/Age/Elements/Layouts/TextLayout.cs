using Age.Commands;
using Age.Core.Extensions;
using Age.Rendering.Shaders.Canvas;
using Age.Services;

namespace Age.Elements.Layouts;

internal partial class TextLayout(TextNode target): Layout
{
    private string? text;
    private Range? selection;

    public uint CaretPosition { get; private set; }
    public Range? Selection
    {
        get => this.selection;
        private set
        {
            if (this.selection != value)
            {
                this.selection = value;
                this.RequestUpdate();
            }
        }
    }

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

                this.Selection = this.Selection?.WithEnd(uint.Min(this.Selection.Value.End, (uint)(this.text?.Length ?? 0)));

                this.RequestUpdate();
            }
        }
    }

    public void ClearSelection()
    {
        this.CaretPosition = 0;
        this.Selection     = default;
    }

    private void GetCharacterOffset(ushort x, ushort _, ref uint character)
    {
        var command = (RectCommand)this.Target.Commands[(int)character];

        var position = this.Target.Transform.Position + command.Rect.Position;

        if (x > position.X + command.Rect.Size.Width / 2)
        {
            character++;
        }
    }

    internal void PropagateSelection(uint characterPosition)
    {
        if (this.text == null || char.IsWhiteSpace(this.text[(int)characterPosition]))
        {
            return;
        }

        var start = (int)characterPosition - 1;
        var end   = (int)characterPosition;

        while (start > -1 && !char.IsWhiteSpace(this.text[start]))
        {
            start--;
        }

        start++;

        while (end < this.text.Length && !char.IsWhiteSpace(this.text[end]))
        {
            end++;
        }

        this.Selection = new((uint)start, (uint)end);
    }

    public void SetCaret(uint position)
    {
        // this.GetCharacterOffset(x, y, ref character);

        this.CaretPosition = position;
        this.Selection     = new(position, position);
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

    public void UpdateSelection(ushort x, ushort y, uint character)
    {
        this.GetCharacterOffset(x, y, ref character);

        this.Selection = this.Selection?.WithEnd(character);
    }

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            if (string.IsNullOrEmpty(this.text))
            {
                this.Target.Commands.Clear();

                this.BaseLine      = -1;
                this.CaretPosition = 0;
                this.LineHeight    = 0;
                this.selection     = default;
                this.Size          = default;
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
