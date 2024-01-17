using Age.Rendering.Services;

namespace Age.Rendering.Drawing;

public class Label : Node
{
    private Element? textElement;
    private string? text;

    public string? Text  { get => this.text; set => this.UpdateText(value); }
    public Style   Style { get; set; }

    public Label(string? text, Style? style = null)
    {
        this.Style = style ?? new() { FontSize = 20 };
        this.Text  = text;
    }

    private void UpdateText(string? value)
    {
        if (value != null && value != this.text)
        {
            this.textElement?.Remove();

            this.Add(this.textElement = TextService.Singleton.DrawText(value, this.Style.FontSize, this.Style.Position));

            this.text = value;
        }
    }
}
