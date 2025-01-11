using Age.Components;

namespace Age.Tests.Age.Components;

public class TextBoxTest
{
    private static void AssertLineInfo(in TextBox.LineInfo lineInfo, uint start, uint end, uint length, ReadOnlySpan<char> text)
    {
        Assert.Equal(start, lineInfo.Start);
        Assert.Equal(end, lineInfo.End);
        Assert.Equal(length, lineInfo.Length);
        Assert.Equal(length, lineInfo.Length);
        Assert.Equal(text, lineInfo.Text);
    }

    [Fact]
    public void LineInfoEmptyString()
    {
        var text = "";

        var lineInfo = new TextBox.LineInfo(text, 0);

        AssertLineInfo(lineInfo, 0, 0, 0, "");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoSingleCharacter()
    {
        var text = "x";

        var lineInfo = new TextBox.LineInfo(text, 0);

        AssertLineInfo(lineInfo, 0, 0, 1, "x");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoSingleNewLine()
    {
        var text = "\n";

        var lineInfo = new TextBox.LineInfo(text, 0);

        AssertLineInfo(lineInfo, 0, 0, 1, "\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoMultiplesSingleNewLine()
    {
        var text = "\n\n\n";

        var lineInfo = new TextBox.LineInfo(text, 1);

        AssertLineInfo(lineInfo, 1, 1, 1, "\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 2, 2, 1, "\n");
    }

    [Fact]
    public void LineInfoWithNewLine()
    {
        var text = "1111\n2222";

        var lineInfo = new TextBox.LineInfo(text, 0);

        AssertLineInfo(lineInfo, 0, 4, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 5, 8, 4, "2222");
    }

    [Fact]
    public void LineInfoFromTheMidleWithNewLine()
    {
        var text = "1111\n2222\n3333";

        var lineInfo = new TextBox.LineInfo(text, 7);

        AssertLineInfo(lineInfo, 5, 9, 5, "2222\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 4, 5, "1111\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 10, 13, 4, "3333");
    }

    [Fact]
    public void NextLineWithTraillingNewLine()
    {
        var text = "\n1111\n";

        var lineInfo = new TextBox.LineInfo(text, 4);

        AssertLineInfo(lineInfo, 1, 5, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoWithNewLineAndCarriageReturn()
    {
        var text = "1111\n\r2222";

        var lineInfo = new TextBox.LineInfo(text, 0);

        AssertLineInfo(lineInfo, 0, 4, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 5, 9, 5, "\r2222");
    }

    [Fact]
    public void LineInfoFromTheMidleWithNewLineAndCarriageReturn()
    {
        var text = "1111\n\r2222\n\r3333";

        var lineInfo = new TextBox.LineInfo(text, 8);

        AssertLineInfo(lineInfo, 5, 10, 6, "\r2222\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 4, 5, "1111\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 11, 15, 5, "\r3333");
    }

    [Fact]
    public void NextLineWithTraillingNewLineAndCarriageReturn()
    {
        var text = "\n\r1111\n\r";

        var lineInfo = new TextBox.LineInfo(text, 5);

        AssertLineInfo(lineInfo, 1, 6, 6, "\r1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 7, 7, 1, "\r");
    }

    [Fact]
    public void LineInfoWithCursorOutsideContent()
    {
        var text = "xyz";

        var lineInfo = new TextBox.LineInfo(text, 6);

        AssertLineInfo(lineInfo, 0, 2, 3, "xyz");
    }
}
