using System.Text;
using Age.Core.Extensions;

namespace Age.Tests.Core.Extensions;

public class ByteExtensionTest
{
    [Fact]
    public void ConvertToStringShoulPass() =>
        Assert.Equal("Hello", Encoding.Default.GetBytes("Hello").ConvertToString());

    [Fact]
    public void ConvertToStringWithEncodignShoulPass() =>
        Assert.Equal("Hello", Encoding.UTF8.GetBytes("Hello").ConvertToString(Encoding.UTF8));
}
