using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Playground.Tests.Styling;

public static class ContentJustificationTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 10u;
        const int  WIDTH       = 200;
        const int  HEIGHT      = 240;

        var horizontalContainerStyle = new Style()
        {
            Border   = new(BORDER_SIZE, 0, Color.Margenta),
            FontSize = 32,
            Size     = new(WIDTH, null),
        };

        var verticalContainerStyle = new Style()
        {
            Border         = new(BORDER_SIZE, 0, Color.Cyan),
            FontSize       = 32,
            Size           = new(null, HEIGHT),
            StackDirection = StackDirection.Vertical,
        };

        var redChildStyle = new Style()
        {
            Border = new(BORDER_SIZE, 0, Color.Red),
        };

        var greenChildStyle = new Style()
        {
            Border = new(BORDER_SIZE, 0, Color.Green),
        };

        var blueChildStyle = new Style()
        {
            Border = new(BORDER_SIZE, 0, Color.Blue),
        };

        FlexBox[] createChildrens(string name) =>
            [
                new FlexBox()
                {
                    Name      = $"{name}_item_01",
                    InnerText = "01",
                    Style     = redChildStyle
                },
                new FlexBox()
                {
                    Name      = $"{name}_item_02",
                    InnerText = "02",
                    Style     = greenChildStyle
                },
                new FlexBox()
                {
                    Name      = $"{name}_item_03",
                    InnerText = "03",
                    Style     = blueChildStyle
                },
            ];

        canvas.Children =
        [
            new FlexBox()
            {
                Name  = "horizontal_a_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.Start,
                },
                Children = createChildrens("horizontal_a"),
            },
            new FlexBox()
            {
                Name  = "horizontal_b_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.Center,
                },
                Children = createChildrens("horizontal_b"),
            },
            new FlexBox()
            {
                Name  = "horizontal_c_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.End,
                },
                Children = createChildrens("horizontal_c"),
            },
            new FlexBox()
            {
                Name  = "horizontal_d_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceAround,
                },
                Children = createChildrens("horizontal_d"),
            },
            new FlexBox()
            {
                Name  = "horizontal_e_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceBetween,
                },
                Children = createChildrens("horizontal_e"),
            },
            new FlexBox()
            {
                Name  = "horizontal_f_container",
                Style = horizontalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceEvenly,
                },
                Children = createChildrens("horizontal_f"),
            },
            new FlexBox()
            {
                Name  = "vertical_a_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.Start,
                },
                Children = createChildrens("vertical_a"),
            },
            new FlexBox()
            {
                Name  = "vertical_b_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.Center,
                },
                Children = createChildrens("vertical_b"),
            },
            new FlexBox()
            {
                Name  = "vertical_c_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.End,
                },
                Children = createChildrens("vertical_c"),
            },
            new FlexBox()
            {
                Name  = "vertical_d_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceAround,
                },
                Children = createChildrens("vertical_d"),
            },
            new FlexBox()
            {
                Name  = "vertical_e_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceBetween,
                },
                Children = createChildrens("vertical_e"),
            },
            new FlexBox()
            {
                Name  = "vertical_f_container",
                Style = verticalContainerStyle with
                {
                    ContentJustification = ContentJustification.SpaceEvenly,
                },
                Children = createChildrens("vertical_f"),
            },
        ];
    }
}
