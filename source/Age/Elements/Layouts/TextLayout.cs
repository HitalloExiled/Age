using Age.Services;

namespace Age.Elements.Layouts;

internal class TextLayout(TextNode target): Layout
{
    private bool isDirty;
    private string? text;

    public override TextNode   Target => target;
    public override BoxLayout? Parent => target.ParentElement?.Layout;

    public float LineHeight { get; set; }

    public string? Text
    {
        get => this.text;
        internal set
        {
            if (value != this.text)
            {
                this.text = value;

                this.isDirty = true;
                this.Update();
            }
        }
    }

    public override void Update()
    {
        if (this.isDirty && (this.Target.ParentElement?.IsConnected ?? false))
        {
            if (string.IsNullOrEmpty(this.text))
            {
                this.Target.Commands.Clear();
            }
            else
            {
                var size = this.Size;

                TextService.Singleton.DrawText(this.Target, this.text);

                if (this.Size != size)
                {
                    this.Parent?.RequestUpdate();
                }
                else if (this.Target.IsConnected)
                {
                    this.Target.Tree.IsDirty = true;
                }
            }

            this.isDirty = false;
        }
    }
}
