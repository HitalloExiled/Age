using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.StoryBook.Pages;

public class TextBoxPage : Page
{
    public override string NodeName => nameof(TextBoxPage);
    public override string Title    => nameof(TextBox);

    public TextBoxPage()
    {
        var containerStyle = new Style
        {
            Color                = Color.White,
            StackDirection       = StackDirection.Vertical,
            ContentJustification = ContentJustification.Start,
        };

        var textBoxStyle = new Style
        {
            Margin = new(Unit.Px(2), null),
        };

        var container = new FlexBox
        {
            Style = new()
            {
                ContentJustification = ContentJustification.SpaceAround,
                Size                 = new(Unit.Pc(100), null),
                Margin               = new(Unit.Px(10)),
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
                        new Text("Readonly"),
                        new TextBox
                        {
                            Style    = textBoxStyle,
                            Value    = "Readonly",
                            Readonly = true
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
                                Overflow = Overflow.ScrollX,
                                Size     = new(Unit.Px(100), null),
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
                                Overflow = Overflow.Scroll,
                                Size     = new(Unit.Px(200)),
                            },
                        },
                    ]
                },

            ]
        };

        this.AppendChild(container);
    }
}
