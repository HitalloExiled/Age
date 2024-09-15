using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class PaddingTest
{
    public static void Setup(Canvas canvas, in TestContext testContext)
    {
        var container_no_padding = new Span
        {
            Name  = "no_padding_container",
            Style = new()
            {
                Border = new(testContext.BorderSize, 0, Color.Margenta),
            }
        };

        var content_no_padding = new Span
        {
            Name = "no_padding_content",
            Style = new()
            {
                Border = new(testContext.BorderSize, 0, Color.Margenta * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var container_padding_px10_100px = new Span
        {
            Name  = "container_padding_px10_100px",
            Style = new()
            {
                Border  = new(testContext.BorderSize, 0, Color.Red),
                Padding = new((Pixel)10),
            }
        };

        var content_padding_px10_100px = new Span
        {
            Name  = "content_padding_px10_100px",
            Style = new()
            {
                Border = new(testContext.BorderSize, 0, Color.Red * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var container_padding_px10_100pc = new Span
        {
            Name  = "container_padding_px10_100pc",
            Style = new()
            {
                Border  = new(testContext.BorderSize, 0, Color.Green),
                Padding = new((Pixel)10),
            }
        };

        var content_padding_px10_100pc = new Span
        {
            Name  = "content_padding_px10_100pc",
            Style = new()
            {

                Border = new(testContext.BorderSize, 0, Color.Green * 0.8f),
                Size   = new((Percentage)100),
            }
        };

        var host_padding_pc10_100pc = new Span
        {
            Name  = "host_padding_pc10_100pc",
            Style = new()
            {
                Border = new(1, 0, Color.Yellow),
                Size   = new((Pixel)200),
            }
        };

        var container_padding_pc10_100pc = new Span
        {
            Name  = "container_padding_pc10_100pc",
            Style = new()
            {
                Border  = new(testContext.BorderSize, 0, Color.Blue),
                Padding = new((Percentage)10),
            }
        };

        var content_padding_pc10_100pc = new Span
        {
            Name  = "content_padding_pc10_100pc",
            Style = new()
            {

                Border = new(testContext.BorderSize, 0, Color.Blue * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var host_padding_100pc_pc10_100pc = new Span
        {
            Name  = "host_padding_100pc_pc10_100pc",
            Style = new()
            {
                Border = new(1, 0, Color.Yellow),
                Size   = new((Pixel)200),
            }
        };

        var container_padding_100pc_pc10_100pc = new Span
        {
            Name  = "container_padding_100pc_pc10_100pc",
            Style = new()
            {
                Border  = new(testContext.BorderSize, 0, Color.Cyan),
                Padding = new((Percentage)10),
                Size    = new((Percentage)100),
            }
        };

        var content_padding_100pc_pc10_100pc = new Span
        {
            Name  = "content_padding_100pc_pc10_100pc",
            Style = new()
            {

                Border = new(testContext.BorderSize, 0, Color.Cyan * 0.8f),
                Size   = new((Pixel)100),
            }
        };

        var text_with_padding = new Span
        {
            Text = "Text With Padding",
            Style = new()
            {
                Color   = Color.White,
                Border  = new(testContext.BorderSize, 0, Color.White * 0.8f),
                Padding = new((Pixel)20, (Pixel)10),
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
