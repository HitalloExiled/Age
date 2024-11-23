using Age.Components;

namespace Age.StoryBook.Pages;

public class TextBoxPage : Page
{
    public override string NodeName { get; } = nameof(TextBoxPage);
    public override string Title    { get; } = nameof(TextBox);

    public TextBoxPage()
    {
        var textBox = new TextBox();

        this.AppendChild(textBox);
    }
}
