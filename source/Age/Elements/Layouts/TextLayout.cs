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

    public override void Update()
    {
        if (this.HasPendingUpdate)
        {
            if (string.IsNullOrEmpty(this.text))
            {
                this.Target.Commands.Clear();
            }
            else
            {
                var info = TextService.Singleton.DrawText(this.Target, this.text);

                target.Layout.BaseLine   = info.Start;
                target.Layout.LineHeight = info.LineHeight;
                target.Layout.Size       = info.Boundings;
            }

            this.HasPendingUpdate = false;
        }
    }
}
