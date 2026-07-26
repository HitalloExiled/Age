using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeStringListTest
{
    private static void AssertList(NativeStringList list, int capacty, ReadOnlySpan<string> values)
    {
        Assert.Equal(capacty, list.Capacity);
        Assert.Equal(values.Length, list.Count);

        for (var i = 0; i < values.Length; i++)
        {
            Assert.Equal(values[i], list[i]);
        }
    }

    [Fact]
    public void Add_CreatesWithInitialStrings()
    {
        using NativeStringList list = ["one", "two", "three"];

        AssertList(list, 3, ["one", "two", "three"]);
    }

    [Fact]
    public void AddAndModify_ModifiesExistingElements()
    {
        using NativeStringList list = ["one", "two", "three"];

        AssertList(list, 3, ["one", "two", "three"]);

        list[0] = "four";
        list[1] = "five";
        list[2] = "six";

        AssertList(list, 3, ["four", "five", "six"]);
    }

    [Fact]
    public void Remove_RemovesElementByIndex()
    {
        using var list = new NativeStringList(["four", "five", "six"]);

        AssertList(list, 3, ["four", "five", "six"]);

        list.Remove(1);

        AssertList(list, 3, ["four", "six"]);
    }

    [Fact]
    public void RemoveWithLength_RemovesRangeOfElements()
    {
        using var list = new NativeStringList(["one", "two", "three", "four", "five", "six"]);

        AssertList(list, 6, ["one", "two", "three", "four", "five", "six"]);

        list.Remove(2, 2);

        AssertList(list, 6, ["one", "two", "five", "six"]);

        list.Remove(2, 2);

        AssertList(list, 6, ["one", "two"]);
    }

    [Fact]
    public void Clear_RemovesAllElements()
    {
        using var list = new NativeStringList(["four", "five", "six"]);

        AssertList(list, 3, ["four", "five", "six"]);

        list.Clear();

        AssertList(list, 3, []);
    }

    [Fact]
    public void IncreaseCapacity_ExpandsCapacity()
    {
        var list = new NativeStringList(["one", "two", "three"]);

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
    public void DecreaseCapacity_ShrinksCapacity()
    {
        var list = new NativeStringList(4);

        Assert.Equal(0, list.Count);
        Assert.Equal(4, list.Capacity);

        list.Add("0");
        list.Add("1");
        list.Add("2");

        Assert.Equal(3, list.Count);
        Assert.Equal(4, list.Capacity);

        list.Capacity = 3;

        Assert.Equal(3, list.Count);
        Assert.Equal(3, list.Capacity);

        Assert.Throws<ArgumentOutOfRangeException>(() => list.Capacity = 2);

        list.Dispose();
    }
}
