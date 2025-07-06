using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public partial class KeyedListTest
{
    private static void AssertIt<TKey, TValue>(KeyedList<TKey, TValue> list, ReadOnlySpan<KeyValuePair<TKey, TValue>> expected)
    where TKey   : unmanaged, Enum
    where TValue : notnull
    {
        var entries = list.ToArray();

        Assert.True(entries.SequenceEqual(expected));
    }

    [Fact]
    public unsafe void AddToEnum8b()
    {
        var list = new KeyedList<Enum8b, string>
        {
            [Enum8b.V04] = "V04",
            [Enum8b.V02] = "V02",
            [Enum8b.V07] = "V07"
        };

        AssertIt(
            list,
            [
                new(Enum8b.V02, "V02"),
                new(Enum8b.V04, "V04"),
                new(Enum8b.V07, "V07"),
            ]
        );
    }

    [Fact]
    public unsafe void AddToEnum16b()
    {
        var list = new KeyedList<Enum16b, string>
        {
            [Enum16b.V11] = "V11",
            [Enum16b.V04] = "V04",
            [Enum16b.V15] = "V15"
        };

        AssertIt(
            list,
            [
                new(Enum16b.V04, "V04"),
                new(Enum16b.V11, "V11"),
                new(Enum16b.V15, "V15"),
            ]
        );
    }

    [Fact]
    public unsafe void AddToEnum32b()
    {
        var list = new KeyedList<Enum32b, string>
        {
            [Enum32b.V04] = "V04",
            [Enum32b.V02] = "V02",
            [Enum32b.V30] = "V30"
        };

        AssertIt(
            list,
            [
                new(Enum32b.V02, "V02"),
                new(Enum32b.V04, "V04"),
                new(Enum32b.V30, "V30"),
            ]
        );
    }

    [Fact]
    public unsafe void AddToEnum64b()
    {
        var list = new KeyedList<Enum64b, string>
        {
            [Enum64b.V30] = "V30",
            [Enum64b.V02] = "V02",
            [Enum64b.V50] = "V50"
        };

        AssertIt(
            list,
            [
                new(Enum64b.V02, "V02"),
                new(Enum64b.V30, "V30"),
                new(Enum64b.V50, "V50"),
            ]
        );

        list.Remove(Enum64b.V30);

        AssertIt(
            list,
            [
                new(Enum64b.V02, "V02"),
                new(Enum64b.V50, "V50"),
            ]
        );
    }
}
