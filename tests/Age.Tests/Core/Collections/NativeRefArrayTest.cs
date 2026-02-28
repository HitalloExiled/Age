using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public unsafe class NativeRefArrayTest
{
    private static void AssertIt(in NativeRefArray<int> list, ReadOnlySpan<int> values)
    {
        Assert.Equal(values.Length, list.Length);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Create()
    {
        using var array = new NativeRefArray<int>(4);

        var ptr = array.Buffer;

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
    public void CreateInitialized()
    {
        using NativeRefArray<int> array = [1, 2, 3, 4];

        var ptr = array.Buffer;

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
        var pointer = new NativeRefArray<int>(4);

        pointer.Dispose();

        try
        {
            pointer[0] = 1;
        }
        catch (Exception exception)
        {
            Assert.IsType<ObjectDisposedException>(exception);
        }
    }
}
