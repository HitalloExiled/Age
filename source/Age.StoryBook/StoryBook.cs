using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Scene;
using Age.Styling;

namespace Age.StoryBook;

public sealed class StoryBook : Node
{
    public override string NodeName { get; } = nameof(StoryBook);

    private readonly Canvas  canvas;
    private readonly FlexBox header;
    private readonly FlexBox outlet;

    public StoryBook()
    {
        var buttonLinkStyle = new Style
        {
            Size           = new((Percentage)100, null),
            ItemsAlignment = ItemsAlignmentKind.Center,
            Margin         = new(null, (Pixel)2),
        };

        Button buttonLink;
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
                        Size                 = new((Pixel)200, (Percentage)100),
                        Border               = new(1, 0, Color.Red),
                        Stack                = StackKind.Vertical,
                        ContentJustification = ContentJustificationKind.Start,
                        ItemsAlignment       = ItemsAlignmentKind.Center,
                        Padding              = new((Pixel)10),
                    },
                    Children =
                    [
                        buttonLink = new Button
                        {
                            Name  = "ButtonLink",
                            Text  = "Button",
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
                        Size                 = new((Percentage)100),
                        Border               = new(1, 0, Color.Green),
                        ContentJustification = ContentJustificationKind.Center,
                        Stack                = StackKind.Vertical,
                    },
                    Children =
                    [
                        this.header = new FlexBox
                        {
                            Name = "Header",
                            Style = new()
                            {
                                Size            = new((Percentage)100, null),
                                Border          = new(1, 0, Color.Blue),
                                BackgroundColor = Color.White.WithAlpha(0.2f),
                                Padding         = new(null, (Pixel)10),
                                Stack           = StackKind.Vertical,
                            },
                            Children =
                            [
                                new FlexBox
                                {
                                    Name = "Title",
                                    Text = "Title",
                                    Style = new()
                                    {
                                        Alignment  = AlignmentKind.Center,
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
                                Size                 = new((Percentage)100),
                                Border               = new(1, 0, Color.Green),
                                // ContentJustification = ContentJustificationKind.Center,
                            },
                        }
                    ]
                }
            ]
        };

        buttonLink.Clicked += (in MouseEvent _) => this.SetPage(new ButtonPage());
        emptyLink.Clicked += (in MouseEvent _) => this.outlet.RemoveChildren();

        this.AppendChild(this.canvas);
        //     canvas.AppendChild(navbar);
        //         navbar.AppendChild(buttonLink);
        //         navbar.AppendChild(emptyLink);
        //     canvas.AppendChild(content);
        //         content.AppendChild(header);
        //             header.AppendChild(title);
        //         content.AppendChild(outlet);

        // this.canvas = canvas;
        // this.header = header;
        // this.outlet = outlet;
    }

    public void SetPage<T>(T page) where T : Page
    {
        this.header.FirstElementChild!.Text = page.Title;
        this.outlet.ReplaceChildren(page);
    }
}
