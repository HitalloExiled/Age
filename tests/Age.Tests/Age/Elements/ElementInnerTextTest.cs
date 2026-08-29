using Age.Elements;
using Age.Styling;
using Age.Tests.Age.Fixtures;

namespace Age.Tests.Age.Elements;

#pragma warning disable CS9113

[Collection("GPU")]
public class ElementInnerTextTest(GpuFixture _)
{
    [Fact]
    public void InnerText_NoChildren_ReturnsEmpty()
    {
        var flexBox = new FlexBox();

        var result = flexBox.InnerText;

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void InnerText_SingleText_ReturnsBuffer()
    {
        var flexBox = new FlexBox();
        var text    = new Text("Hello");

        flexBox.AppendChild(text);

        Assert.Equal("Hello", flexBox.InnerText);
    }

    [Fact]
    public void InnerText_MultipleTexts_Concatenates()
    {
        var flexBox = new FlexBox();

        flexBox.AppendChild(new Text("Hello"));
        flexBox.AppendChild(new Text(" "));
        flexBox.AppendChild(new Text("World"));

        Assert.Equal("Hello World", flexBox.InnerText);
    }

    [Fact]
    public void InnerText_NonTextChild_Ignored()
    {
        var flexBox = new FlexBox();
        var child   = new FlexBox();

        flexBox.AppendChild(new Text("Hello"));
        flexBox.AppendChild(child);
        flexBox.AppendChild(new Text("World"));

        child.AppendChild(new Text("Nested"));

        Assert.Equal("HelloNestedWorld", flexBox.InnerText);
    }

    [Fact]
    public void InnerText_VerticalStack_AppendsNewlines()
    {
        var flexBox = new FlexBox();

        flexBox.AppendChild(new Text("Line1"));
        flexBox.AppendChild(new Text("Line2"));

        flexBox.ComputedStyle.StackDirection = StackDirection.Vertical;

        Assert.Equal("Line1\nLine2", flexBox.InnerText);
    }
}
