using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringArrayTest
{
    [Fact]
    public void AllocateAndSet_AllocatesAndSetsStringElements()
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
    public void ToArrayShouldPass_ReturnsCopyOfStrings()
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
    public unsafe void ImplicitOperator_ConvertsToPointer()
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
    public void DisposeShouldPass_DoesNotThrowOnDoubleDispose()
    {
        var array = new NativeStringArray(["one"]);

        array.Dispose();
        array.Dispose();

        Assert.True(true);
    }
}
