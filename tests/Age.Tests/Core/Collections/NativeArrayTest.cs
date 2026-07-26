using Age.Core.Collections;
using Age.Core.Extensions;

namespace Age.Tests.Core.Collections;

public unsafe class NativeArrayTest
{
    private static void AssertIt(NativeArray<int> list, ReadOnlySpan<int> values)
    {
        Assert.Equal(values.Length, list.Length);

        Assert.True(list.AsSpan().SequenceEqual(values));
    }

    [Fact]
    public void Create_AllocatesAndSetsElements()
    {
        using var array = new NativeArray<int>(4);

        array[0] = 1;
        array[1] = 2;
        array[2] = 3;
        array[3] = 4;

        AssertIt(array, [1, 2, 3, 4]);

        array[0] = 2;
        array[1] = 3;
        array[2] = 4;
        array[3] = 5;

        AssertIt(array, [2, 3, 4, 5]);
    }

    [Fact]
    public void CreateInitialized_AllocatesWithInitialValues()
    {
        using NativeArray<int> array = [1, 2, 3, 4];

        AssertIt(array, [1, 2, 3, 4]);

        array[0] = 2;
        array[1] = 3;
        array[2] = 4;
        array[3] = 5;

        AssertIt(array, [2, 3, 4, 5]);
    }

    [Fact]
    public void Enumerate_IteratesOverAllElements()
    {
        using var array = new NativeArray<int>([1, 2, 3, 4, 5, 6]);

        var list = new List<int>(6);

        foreach (var item in array)
        {
            list.Add(item);
        }

        AssertIt(array, list.AsSpan());
    }
}
