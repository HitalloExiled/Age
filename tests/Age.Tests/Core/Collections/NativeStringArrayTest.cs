using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringArrayTest
{
    [Fact]
    public void AllocateAndSet()
    {
        var list = new[]
        {
            "One",
            "Two",
            "Three",
        };

        using var stringArrayPtr = new NativeStringArray(3);

        stringArrayPtr[0] = list[0];
        stringArrayPtr[1] = list[1];
        stringArrayPtr[2] = list[2];

        Assert.True(list.SequenceEqual(stringArrayPtr.ToArray()));
    }

    [Fact]
    public void ToArrayShouldPass()
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

        Assert.True(ppData == stringArrayPtr.Buffer);
    }

    [Fact]
    public void DisposeShouldPass()
    {
        var array = new NativeStringArray(["one"]);

        array.Dispose();
        array.Dispose();

        Assert.True(true);
    }
}
