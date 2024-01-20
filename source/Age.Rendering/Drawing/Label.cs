using Age.Rendering.Services;

namespace Age.Rendering.Drawing;

public class Label : Element
{
    private string? text;

    public string? Text  { get => this.text; set => this.UpdateText(value); }

    public Label(string? text, Style? style = null)
    {
        if (style != null)
        {
            this.Style = style;
        }

        this.Text  = text;
    }

    private void UpdateText(string? value)
    {
        if (value != null && value != this.text)
        {
            Singleton.TextService.DrawText(this, value);

            this.text = value;
        }
    }
}
