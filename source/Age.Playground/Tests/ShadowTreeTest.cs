using Age.Elements;
using Age.Numerics;

namespace Age.Playground.Tests;

public class Host : Element
{
    public override string NodeName { get; } = nameof(Host);

    public Host()
    {
        this.AttachShadowTree();

        var shadowElement = new FlexBox
        {
            Text  = "Shadow Element",
            Style = new()
            {
                Border = new(1, 0, Color.Red),
                Color  = Color.White,
            }
        };

        this.ShadowTree.AppendChild(shadowElement);
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
                //Size   = new((Pixel)100),
                Border = new(1, 0, Color.Red),
            },
            Children =
            [
                new FlexBox
                {
                    Text  = "Light Element",
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
