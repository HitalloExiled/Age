using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.StoryBook.Pages;

public class TextBoxPage : Page
{
    public override string NodeName { get; } = nameof(TextBoxPage);
    public override string Title    { get; } = nameof(TextBox);

    public TextBoxPage()
    {
        var containerStyle = new Style
        {
            Color                = Color.White,
            Stack                = StackKind.Vertical,
            ContentJustification = ContentJustificationKind.Start,
        };

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
                new FlexBox
                {
                    Style    = containerStyle,
                    Children =
                    [
                        new Text("Default"),
                        new TextBox
                        {
                            Style = textBoxStyle,
                        },
                    ]
                },
                new FlexBox
                {
                    Style    = containerStyle,
                    Children =
                    [
                        new Text("Multiline"),
                        new TextBox
                        {
                            Style     = textBoxStyle,
                            Multiline = true,
                        },
                    ]
                },
                new FlexBox
                {
                    Style    = containerStyle,
                    Children =
                    [
                        new Text("Fixed Size"),
                        new TextBox
                        {
                            Style = textBoxStyle with
                            {
                                Overflow = OverflowKind.ScrollX,
                                Size     = new((Pixel)100, null),
                            },
                        },
                    ]
                },
                new FlexBox
                {
                    Style    = containerStyle,
                    Children =
                    [
                        new Text("Fixed Size Multiline"),
                        new TextBox
                        {
                            Value =
                            """
                            1111
                            2222
                            3333
                            4444
                            """,
                            Multiline = true,
                            Style     = textBoxStyle with
                            {
                                Overflow = OverflowKind.Scroll,
                                Size     = new((Pixel)200),
                            },
                        },
                    ]
                },

            ]
        };

        this.AppendChild(container);
    }
}
