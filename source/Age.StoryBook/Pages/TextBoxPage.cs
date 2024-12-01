using Age.Components;
using Age.Elements;
using Age.Styling;

namespace Age.StoryBook.Pages;

public class TextBoxPage : Page
{
    public override string NodeName { get; } = nameof(TextBoxPage);
    public override string Title    { get; } = nameof(TextBox);

    public TextBoxPage()
    {
        var textBoxStyle = new Style
        {
            Margin = new((Pixel)2, null),
        };

        var container = new FlexBox
        {
            Style = new()
            {
                ContentJustification = ContentJustificationKind.SpaceAround,
                Size                 = new((Percentage)100, null),
                Margin               = new((Pixel)10),
            },
            Children =
            [
                new TextBox
                {
                    Style = textBoxStyle,
                },
                new TextBox
                {
                    Style = textBoxStyle with
                    {
                        Overflow = OverflowKind.ScrollX,
                        Size     = new((Pixel)100, null),
                    },
                },
            ]
        };

        this.AppendChild(container);
    }
}
