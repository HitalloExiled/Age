using Age.Components;

namespace Age.Tests.Age.Components;

public class TextBoxTest
{
    private static void AssertLineInfo(in TextBox.LineInfo lineInfo, uint start, uint end, uint length, ReadOnlySpan<char> text)
    {
        Assert.Equal(start,  lineInfo.Start);
        Assert.Equal(end,    lineInfo.End);
        Assert.Equal(length, lineInfo.Length);
        Assert.Equal(length, lineInfo.Length);
        Assert.Equal(text,   lineInfo.Text);
    }

    [Fact]
    public void LineInfoEmptyString()
    {
        const string TEXT = "";

        var lineInfo = new TextBox.LineInfo(TEXT, 0);

        AssertLineInfo(lineInfo, 0, 0, 0, "");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoSingleCharacter()
    {
        const string TEXT = "x";

        var lineInfo = new TextBox.LineInfo(TEXT, 0);

        AssertLineInfo(lineInfo, 0, 0, 1, "x");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoSingleNewLine()
    {
        const string TEXT = "\n";

        var lineInfo = new TextBox.LineInfo(TEXT, 0);

        AssertLineInfo(lineInfo, 0, 0, 1, "\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoMultiplesSingleNewLine()
    {
        const string TEXT = "\n\n\n";

        var lineInfo = new TextBox.LineInfo(TEXT, 1);

        AssertLineInfo(lineInfo, 1, 1, 1, "\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 2, 2, 1, "\n");
    }

    [Fact]
    public void LineInfoWithNewLine()
    {
        const string TEXT = "1111\n2222";

        var lineInfo = new TextBox.LineInfo(TEXT, 0);

        AssertLineInfo(lineInfo, 0, 4, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 5, 8, 4, "2222");
    }

    [Fact]
    public void LineInfoFromTheMidleWithNewLine()
    {
        const string TEXT = "1111\n2222\n3333";

        var lineInfo = new TextBox.LineInfo(TEXT, 7);

        AssertLineInfo(lineInfo, 5, 9, 5, "2222\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 4, 5, "1111\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 10, 13, 4, "3333");
    }

    [Fact]
    public void NextLineWithTraillingNewLine()
    {
        const string TEXT = "\n1111\n";

        var lineInfo = new TextBox.LineInfo(TEXT, 4);

        AssertLineInfo(lineInfo, 1, 5, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 0, 0, 0, "");
    }

    [Fact]
    public void LineInfoWithNewLineAndCarriageReturn()
    {
        const string TEXT = "1111\n\r2222";

        var lineInfo = new TextBox.LineInfo(TEXT, 0);

        AssertLineInfo(lineInfo, 0, 4, 5, "1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 0, "");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 5, 9, 5, "\r2222");
    }

    [Fact]
    public void LineInfoFromTheMidleWithNewLineAndCarriageReturn()
    {
        const string TEXT = "1111\n\r2222\n\r3333";

        var lineInfo = new TextBox.LineInfo(TEXT, 8);

        AssertLineInfo(lineInfo, 5, 10, 6, "\r2222\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 4, 5, "1111\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 11, 15, 5, "\r3333");
    }

    [Fact]
    public void NextLineWithTraillingNewLineAndCarriageReturn()
    {
        const string TEXT = "\n\r1111\n\r";

        var lineInfo = new TextBox.LineInfo(TEXT, 5);

        AssertLineInfo(lineInfo, 1, 6, 6, "\r1111\n");

        var previous = lineInfo.PreviousLine();

        AssertLineInfo(previous, 0, 0, 1, "\n");

        var next = lineInfo.NextLine();

        AssertLineInfo(next, 7, 7, 1, "\r");
    }

    [Fact]
    public void LineInfoWithCursorOutsideContent()
    {
        const string TEXT = "xyz";

        var lineInfo = new TextBox.LineInfo(TEXT, 6);

        AssertLineInfo(lineInfo, 0, 2, 3, "xyz");
    }
}
