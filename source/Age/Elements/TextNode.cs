using Age.Services;

namespace Age.Elements;

public class TextNode : ContainerNode
{
    private string? value;

    public override string NodeName { get; } = nameof(TextNode);

    public Element? ParentElement => this.Parent as Element;

    internal float LineHeight { get; set; }

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
            this.Draw();
        }

        this.value = value;
    }

    internal void Draw()
    {
        if (string.IsNullOrEmpty(this.value))
        {
            this.Commands.Clear();
        }
        else
        {
            TextService.Singleton.DrawText(this, this.value);

            if (this.IsConnected)
            {
                this.Tree.IsDirty = true;
            }
        }
    }
}