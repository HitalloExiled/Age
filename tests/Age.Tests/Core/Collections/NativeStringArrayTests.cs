using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringArrayTests
{
    [Fact]
    public void ToListShouldPass()
    {
        var list = new[]
        {
            "One",
            "Two",
            "Three",
        };

        using var stringArrayPtr = new NativeStringArray(list);

        Assert.True(list.SequenceEqual(stringArrayPtr.ToArray()));
    }

    [Fact]
    public unsafe void ImplicitOperatorShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        using var stringArrayPtr = new NativeStringArray(list);

        byte** ppData = stringArrayPtr;

        Assert.True(ppData == stringArrayPtr.AsPointer());
    }

    [Fact]
    public void DisposeShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        var stringArrayPtr = new NativeStringArray(list);

        stringArrayPtr.Dispose();
        stringArrayPtr.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void DestructorShouldPass()
    {
        _ = new NativeStringArray(Array.Empty<string>());

        Assert.True(true);
    }
}
