using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Scene;
using Age.StoryBook.Pages;
using Age.Styling;

namespace Age.StoryBook;

public sealed class StoryBook : Node
{
    public override string NodeName => nameof(StoryBook);

    private readonly Canvas  canvas;
    private readonly FlexBox header;
    private readonly FlexBox outlet;

    public StoryBook()
    {
        var buttonLinkStyle = new Style
        {
            Size           = new(Unit.Pc(100), null),
            ItemsAlignment = ItemsAlignment.Center,
            Margin         = new(null, Unit.Px(2)),
        };

        Button buttonLink;
        Button iconLink;
        Button textBoxLink;
        Button emptyLink;

        this.canvas = new Canvas
        {
            Children =
            [
                new FlexBox
                {
                    Name  = "NavBar",
                    Style = new()
                    {
                        Size                 = new(Unit.Px(200), Unit.Pc(100)),
                        Border               = new(1, 0, Color.Red),
                        StackDirection       = StackDirection.Vertical,
                        ContentJustification = ContentJustification.Start,
                        ItemsAlignment       = ItemsAlignment.Center,
                        Padding              = new(Unit.Px(10)),
                    },
                    Children =
                    [
                        buttonLink = new Button
                        {
                            Name  = "ButtonLink",
                            Text  = "Button",
                            Style = buttonLinkStyle,
                        },
                        iconLink = new Button
                        {
                            Name  = "IconLink",
                            Text  = "Icon",
                            Style = buttonLinkStyle,
                        },
                        textBoxLink = new Button
                        {
                            Name  = "TextBoxLink",
                            Text  = "TextBox",
                            Style = buttonLinkStyle,
                        },
                        emptyLink = new Button
                        {
                            Name  = "EmptyLink",
                            Text  = "Empty",
                            Style = buttonLinkStyle,
                        }
                    ]
                },
                new FlexBox
                {
                    Name  = "Content",
                    Style = new()
                    {
                        Size                 = new(Unit.Pc(100)),
                        Border               = new(1, 0, Color.Green),
                        ContentJustification = ContentJustification.Center,
                        StackDirection       = StackDirection.Vertical,
                    },
                    Children =
                    [
                        this.header = new FlexBox
                        {
                            Name = "Header",
                            Style = new()
                            {
                                Size            = new(Unit.Pc(100), null),
                                Border          = new(1, 0, Color.Blue),
                                BackgroundColor = Color.White.WithAlpha(0.2f),
                                Padding         = new(null, Unit.Px(10)),
                                StackDirection  = StackDirection.Vertical,
                            },
                            Children =
                            [
                                new FlexBox
                                {
                                    Name = "Title",
                                    Text = "Title",
                                    Style = new()
                                    {
                                        Alignment  = Alignment.Center,
                                        Color      = Color.White,
                                        FontSize   = 24,
                                        FontWeight = FontWeight.Bold,
                                    },
                                }
                            ]
                        },
                        this.outlet = new FlexBox
                        {
                            Name  = "Outlet",
                            Style = new()
                            {
                                Size   = new(Unit.Pc(100)),
                                Border = new(1, 0, Color.Green),
                            },
                        }
                    ]
                }
            ]
        };

        buttonLink.Clicked  += (in _) => this.SetPage(new ButtonPage());
        iconLink.Clicked    += (in _) => this.SetPage(new IconPage());
        textBoxLink.Clicked += (in _) => this.SetPage(new TextBoxPage());

        emptyLink.Clicked += (in _) => this.outlet.RemoveChildren();

        this.AppendChild(this.canvas);
    }

    public void SetPage<T>(T page) where T : Page
    {
        this.header.FirstElementChild!.Text = page.Title;
        this.outlet.ReplaceChildren(page);
    }
}
