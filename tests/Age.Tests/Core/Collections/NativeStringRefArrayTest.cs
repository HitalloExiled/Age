using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringRefArrayTest
{
    [Fact]
    public void ToArrayShouldPass()
    {
        var list = new[]
        {
            "One",
            "Two",
            "Three",
        };

        using var stringArrayPtr = new NativeStringRefArray(list);

        Assert.True(list.SequenceEqual(stringArrayPtr.ToArray()));
    }

    [Fact]
    public unsafe void ImplicitOperatorShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        using var stringArrayPtr = new NativeStringRefArray(list);

        byte** ppData = stringArrayPtr;

        Assert.True(ppData == stringArrayPtr.Buffer);
    }

    [Fact]
    public void DisposeShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        var stringArrayPtr = new NativeStringRefArray(list);

        stringArrayPtr.Dispose();
        stringArrayPtr.Dispose();

        Assert.True(true);
    }

    [Fact]
    public void DestructorShouldPass()
    {
        _ = new NativeStringRefArray(Array.Empty<string>());

        Assert.True(true);
    }
}
