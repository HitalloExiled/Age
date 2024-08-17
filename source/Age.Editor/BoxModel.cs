using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor;

public class BoxModel : Element
{
    public override string NodeName { get; } = nameof(BoxModel);

    public BoxModel()
    {
        const int BORDER_SIZE = 4;
        this.Name = "BoxModel";
        this.Style.Border = new(BORDER_SIZE, 0, Color.Red);
        this.Style.Size      = SizeUnit.Percentage(100);
        this.Style.BoxSizing = BoxSizing.Border;

        var stackL = new Span
        {
            Name = "StackL",
            Style = new()
            {
                Stack     = StackType.Vertical,
                Border    = new(BORDER_SIZE, 0, Color.Green),
                Size      = new(Unit.Percentage(50), Unit.Percentage(100)),
            }
        };

        var stackText = new Span
        {
            Text = "This elements should be stacked vertically...",
            Name  = "StackText",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Color     = Color.White,
                Border    = new(10, 0, Color.Margenta),
            }
        };

        var boxAL = new Span
        {
            Name  = "BoxAL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Cyan),
                Size   = new(Unit.Percentage(100), Unit.Percentage(50)),
            }
        };

        var boxBL = new Span
        {
            Name  = "BoxBL",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = new(Unit.Percentage(100), Unit.Percentage(50)),
            }
        };

        var boxCL = new Span
        {
            Name  = "BoxCL",
            Style = new()
            {
                Alignment = AlignmentType.Center,
                Color     = Color.White,
                Border    = new(BORDER_SIZE, 0, Color.Yellow),
                Size      = SizeUnit.Pixel(50),
            }
        };

        var stackR = new Span
        {
            Name = "StackR",
            Style = new()
            {
                Stack  = StackType.Vertical,
                Border = new(BORDER_SIZE, 0, Color.Blue),
                Size   = new(Unit.Percentage(100), Unit.Percentage(100)),
            }
        };

        var boxAR = new Span
        {
            Name  = "BoxAR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Cyan),
                Size   = SizeUnit.Pixel(50),
            }
        };

        var boxBR = new Span
        {
            Name  = "BoxBR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.White),
                Size   = SizeUnit.Percentage(100),
            }
        };

        var boxCR = new Span
        {
            Name  = "BoxCR",
            Style = new()
            {
                Color  = Color.White,
                Border = new(BORDER_SIZE, 0, Color.Yellow),
                Size   = SizeUnit.Pixel(50),
            }
        };

        this.AppendChild(stackL);
            stackL.AppendChild(stackText);
            stackL.AppendChild(boxAL);
            stackL.AppendChild(boxBL);
            stackL.AppendChild(boxCL);

        this.AppendChild(stackR);
            stackR.AppendChild(boxAR);
            stackR.AppendChild(boxBR);
            stackR.AppendChild(boxCR);
    }
}
