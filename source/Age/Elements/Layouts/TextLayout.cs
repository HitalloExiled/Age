using Age.Services;

namespace Age.Elements.Layouts;

internal class TextLayout(TextNode target): Layout
{
    private string? text;

    public override TextNode   Target => target;
    public override BoxLayout? Parent => target.ParentElement?.Layout;    

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

    public override void Hide() =>
        this.HasPendingUpdate = false;

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
