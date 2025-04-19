using Age.Core;

namespace Age.Tests.Core.Interop;

public unsafe class NativeArrayTests
{
    private static void AssertIt(NativeArray<int> list, ReadOnlySpan<int> values)
    {
        Assert.Equal(values.Length, list.Length);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Create()
    {
        using var array = new NativeArray<int>(4);

        var ptr = array.AsPointer();

        ptr[0] = 1;
        ptr[1] = 2;
        ptr[2] = 3;
        ptr[3] = 4;

        AssertIt(array, [1, 2, 3, 4]);

        array[0] = 2;
        array[1] = 3;
        array[2] = 4;
        array[3] = 5;

        AssertIt(array, [2, 3, 4, 5]);
    }

    [Fact]
    public void GenericPointerAccessAfterDisposedShouldThows()
    {
        var pointer = new NativeArray<int>(4);
        pointer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => pointer[0] == 1);
    }
}
