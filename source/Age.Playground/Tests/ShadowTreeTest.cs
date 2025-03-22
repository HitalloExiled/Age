using Age.Elements;
using Age.Numerics;
using Age.Styling;

namespace Age.Playground.Tests;

public class Host : Element
{
    public override string NodeName { get; } = nameof(Host);

    public Host()
    {
        this.AttachShadowTree();

        this.ShadowTree.Children =
        [
            new FlexBox
            {
                Text  = "### Shadow Element ###",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                }
            },
            new Slot
            {
                //Name  = "1",
                Text  = "Default",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                }
            },
            new Slot
            {
                Text  = "Default Content",
                Name  = "s-1",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                    Size   = new((Pixel)200, null),
                }
            },
            new Slot
            {
                Text  = "Default Content",
                Name  = "s-2",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                }
            },
        ];
    }
}

public class ShadowTreeTest
{
    public static void Setup(Canvas canvas)
    {
        var host = new Host
        {
            Style = new()
            {
                Stack  = StackKind.Vertical,
                //Size   = new((Pixel)100),
                Border = new(1, 0, Color.Red),
            },
            Children =
            [
                new FlexBox
                {
                    Text  = "Light Element 1",
                    Name  = "light-1",
                    Slot  = "s-1",
                    Style = new()
                    {
                        Border = new(1, 0, Color.Green),
                        Color  = Color.White,
                        Size   = new((Percentage)100, null),
                    }
                },
                new FlexBox
                {
                    Text  = "Light Element 2",
                    Name  = "light-1",
                    Slot  = "s-2",
                    Style = new()
                    {
                        Border = new(1, 0, Color.Green),
                        Color  = Color.White,
                    }
                }
            ]
        };

        canvas.AppendChild(host);
    }
}
