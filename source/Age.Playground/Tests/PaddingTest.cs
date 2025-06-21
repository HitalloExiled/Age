using Age.Numerics;
using Age.Elements;

namespace Age.Playground.Tests;

public static class PaddingTest
{
    public static void Setup(Canvas canvas)
    {
        const uint BORDER_SIZE = 10u;

        var container_no_padding = new FlexBox
        {
            Name  = "no_padding_container",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Margenta),
            }
        };

        var content_no_padding = new FlexBox
        {
            Name = "no_padding_content",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Margenta * 0.8f),
                Size   = new(Unit.Px(100)),
            }
        };

        var container_padding_px10_100px = new FlexBox
        {
            Name  = "container_padding_px10_100px",
            Style = new()
            {
                Border  = new(BORDER_SIZE, 0, Color.Red),
                Padding = new(Unit.Px(10)),
            }
        };

        var content_padding_px10_100px = new FlexBox
        {
            Name  = "content_padding_px10_100px",
            Style = new()
            {
                Border = new(BORDER_SIZE, 0, Color.Red * 0.8f),
                Size   = new(Unit.Px(100)),
            }
        };

        var container_padding_px10_100pc = new FlexBox
        {
            Name  = "container_padding_px10_100pc",
            Style = new()
            {
                Border  = new(BORDER_SIZE, 0, Color.Green),
                Padding = new(Unit.Px(10)),
            }
        };

        var content_padding_px10_100pc = new FlexBox
        {
            Name  = "content_padding_px10_100pc",
            Style = new()
            {

                Border = new(BORDER_SIZE, 0, Color.Green * 0.8f),
                Size   = new(Unit.Pc(100)),
            }
        };

        var host_padding_pc10_100pc = new FlexBox
        {
            Name  = "host_padding_pc10_100pc",
            Style = new()
            {
                Border = new(1, 0, Color.Yellow),
                Size   = new(Unit.Px(200)),
            }
        };

        var container_padding_pc10_100pc = new FlexBox
        {
            Name  = "container_padding_pc10_100pc",
            Style = new()
            {
                Border  = new(BORDER_SIZE, 0, Color.Blue),
                Padding = new(Unit.Pc(10)),
            }
        };

        var content_padding_pc10_100pc = new FlexBox
        {
            Name  = "content_padding_pc10_100pc",
            Style = new()
            {

                Border = new(BORDER_SIZE, 0, Color.Blue * 0.8f),
                Size   = new(Unit.Px(100)),
            }
        };

        var host_padding_100pc_pc10_100pc = new FlexBox
        {
            Name  = "host_padding_100pc_pc10_100pc",
            Style = new()
            {
                Border = new(1, 0, Color.Yellow),
                Size   = new(Unit.Px(200)),
            }
        };

        var container_padding_100pc_pc10_100pc = new FlexBox
        {
            Name  = "container_padding_100pc_pc10_100pc",
            Style = new()
            {
                Border  = new(BORDER_SIZE, 0, Color.Cyan),
                Padding = new(Unit.Pc(10)),
                Size    = new(Unit.Pc(100)),
            }
        };

        var content_padding_100pc_pc10_100pc = new FlexBox
        {
            Name  = "content_padding_100pc_pc10_100pc",
            Style = new()
            {

                Border = new(BORDER_SIZE, 0, Color.Cyan * 0.8f),
                Size   = new(Unit.Px(100)),
            }
        };

        var text_with_padding = new FlexBox
        {
            Text = "Text With Padding",
            Style = new()
            {
                Color   = Color.White,
                Border  = new(BORDER_SIZE, 0, Color.White * 0.8f),
                Padding = new(Unit.Px(20), Unit.Px(10)),
            }
        };

        canvas.AppendChild(text_with_padding);

        canvas.AppendChild(container_no_padding);
            container_no_padding.AppendChild(content_no_padding);

        canvas.AppendChild(container_padding_px10_100px);
            container_padding_px10_100px.AppendChild(content_padding_px10_100px);

        canvas.AppendChild(container_padding_px10_100pc);
            container_padding_px10_100pc.AppendChild(content_padding_px10_100pc);

        canvas.AppendChild(host_padding_pc10_100pc);
            host_padding_pc10_100pc.AppendChild(container_padding_pc10_100pc);
                container_padding_pc10_100pc.AppendChild(content_padding_pc10_100pc);

        canvas.AppendChild(host_padding_100pc_pc10_100pc);
            host_padding_100pc_pc10_100pc.AppendChild(container_padding_100pc_pc10_100pc);
                container_padding_100pc_pc10_100pc.AppendChild(content_padding_100pc_pc10_100pc);
    }
}
