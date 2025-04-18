using Age.Numerics;
using Age.Styling;

namespace Age.Tests.Age.Elements.Styling;

public class StyleTest
{
    [Fact]
    public void GetToString()
    {
        var style = new Style();

        Assert.Equal("", style.ToString());
    }

    [Fact]
    public void Diff()
    {
        var left = new Style
        {
            ContentJustification = ContentJustificationKind.Center,
            Size                 = new(null, (Pixel)100),
            Color                = Color.Black,
        };

        var right = new Style
        {
            Alignment = AlignmentKind.Baseline,
            Size      = new((Percentage)200, (Pixel)100),
            Color     = Color.Black,
        };

        var expected = StyleProperty.Alignment | StyleProperty.ContentJustification | StyleProperty.Size;
        var actual   = Style.Diff(left, right);

        Assert.Equal(expected, actual);
    }
}
