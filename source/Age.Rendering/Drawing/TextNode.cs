namespace Age.Rendering.Drawing.Elements;

public class TextNode : Node2D
{
    private string? value;

    public override string NodeName { get; } = nameof(TextNode);

    public Element? ParentElement => this.Parent as Element;

    public string? Value
    {
        get => this.value;
        set => this.UpdateText(value);
    }

    private void UpdateText(string? value)
    {
        var parent = this.ParentElement;

        if (parent != null && parent.IsConnected && value != this.value)
        {
            this.Redraw();
        }

        this.value = value;
    }

    public void Redraw()
    {
        if (string.IsNullOrEmpty(this.value))
        {
            this.Commands.Clear();
        }
        else
        {
            Container.Singleton.TextService.DrawText(this, this.value);
        }
    }
}
