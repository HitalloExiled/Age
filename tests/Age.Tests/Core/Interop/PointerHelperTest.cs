using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public class PointerHelperTest
{
    [Fact]
    public unsafe void NullIfDefaultShouldPass()
    {
        var defaultValue    = 0;
        var nonDefaultValue = 1;
        var defaultValuepValue     = &defaultValue;
        var pNonDefaultValuepValue = &nonDefaultValue;

        Assert.True(PointerHelper.NullIfDefault(defaultValuepValue)     == null);
        Assert.True(PointerHelper.NullIfDefault(pNonDefaultValuepValue) == pNonDefaultValuepValue);
    }
}
