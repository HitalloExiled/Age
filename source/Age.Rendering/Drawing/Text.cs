namespace Age.Rendering.Drawing;

public class Text : Element
{
    private string? value;

    public string? Value { get => this.value; set => this.UpdateText(value); }

    public Text(string? value, Style? style = null)
    {
        if (style != null)
        {
            this.Style = style;
        }

        this.Value = value;
    }

    private void UpdateText(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            this.Commands.Clear();
        }
        else if (value != this.value)
        {
            Container.Singleton.TextService.DrawText(this, value);

            this.value = value;
        }
    }
}
