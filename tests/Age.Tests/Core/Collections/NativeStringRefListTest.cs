using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringRefListTest
{
    private static void AssertList(NativeStringRefList list, int capacty, ReadOnlySpan<string> values)
    {
        Assert.Equal(capacty, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        for (var i = 0; i < values.Length; i++)
        {
            Assert.Equal(values[i], list[i]);
        }
    }

    [Fact]
    public void Add()
    {
        using NativeStringRefList list = ["one", "two", "three"];

        AssertList(list, 3, ["one", "two", "three"]);
    }

    [Fact]
    public void Remove()
    {
        using var list = new NativeStringRefList(["four", "five", "six"]);

        AssertList(list, 3, ["four", "five", "six"]);

        list.Remove(1);

        AssertList(list, 3, ["four", "six"]);
    }

    [Fact]
    public void RemoveWithLength()
    {
        using var list = new NativeStringRefList(["one", "two", "three", "four", "five", "six"]);

        AssertList(list, 6, ["one", "two", "three", "four", "five", "six"]);

        list.Remove(2, 2);

        AssertList(list, 6, ["one", "two", "five", "six"]);

        list.Remove(2, 2);

        AssertList(list, 6, ["one", "two"]);
    }

    [Fact]
    public void Clear()
    {
        using var list = new NativeStringRefList(["four", "five", "six"]);

        AssertList(list, 3, ["four", "five", "six"]);

        list.Clear();

        AssertList(list, 3, []);
    }

    [Fact]
    public void IncreaseCapacity()
    {
        var list = new NativeStringRefList(["one", "two", "three"]);

        AssertList(list, 3, ["one", "two", "three"]);

        list.Capacity = 6;

        AssertList(list, 6, ["one", "two", "three"]);

        list.Add("four");
        list.Add("five");
        list.Add("six");

        AssertList(list, 6, ["one", "two", "three", "four", "five", "six"]);

        list.Dispose();
    }

    [Fact]
    public void DecreaseCapacity()
    {
        var list = new NativeStringRefList(["one", "two", "three", "four", "five", "six"]);

        AssertList(list, 6, ["one", "two", "three", "four", "five", "six"]);

        list.Capacity = 3;

        AssertList(list, 3, ["one", "two", "three"]);

        list.Dispose();
    }

    [Fact]
    public void DisposeShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        var stringArrayPtr = new NativeStringRefList(list);

        stringArrayPtr.Dispose();
        stringArrayPtr.Dispose();

        Assert.True(true);
    }
}
