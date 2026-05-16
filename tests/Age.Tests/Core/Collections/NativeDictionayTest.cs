using Age.Core.Collections;

namespace Age.Tests.Core.Collections;

public class NativeDictionayTest
{
    private static void AssertIt<K, V>(in NativeDictionary<K, V> dictionary, ReadOnlySpan<KeyValuePair<K, V>> entries, int capacity)
    where K : unmanaged, IEquatable<K>
    where V : unmanaged
    {
        Assert.Equal(entries.Length, dictionary.Count);
        Assert.Equal(capacity, dictionary.Capacity);

        var keys   = new List<K>(entries.Length);
        var values = new List<V>(entries.Length);

        foreach (var (key, value) in entries)
        {
            Assert.Equal(value, dictionary[key]);

            keys.Add(key);
            values.Add(value);
        }

        Assert.Equal(keys.Order(), dictionary.Keys.ToArray().Order());
        Assert.Equal(values.Order(), dictionary.Values.ToArray().Order());
    }

    [Fact]
    public void Create()
    {
        using NativeDictionary<int, long> dictionary = new()
        {
            [100] = 1,
            [200] = 2,
            [300] = 3,
        };

        AssertIt(dictionary, [new(100, 1), new(200, 2), new(300, 3)], 3);
    }

    [Fact]
    public void Add()
    {
        using var dictionary = new NativeDictionary<int, int>(0);

        dictionary[0] = 100;
        dictionary[3] = 200;

        AssertIt(dictionary, [new(0, 100), new(3, 200)], 3);

        dictionary[6] = 300;
        dictionary[9] = 400;

        AssertIt(dictionary, [new(0, 100), new(3, 200), new(6, 300), new(9, 400)], 7);
    }

    [Fact]
    public void Set()
    {
        using var dictionary = new NativeDictionary<int, int>();

        dictionary[0] = 100;

        AssertIt(dictionary, [new(0, 100)], 3);

        dictionary[0] = 200;

        AssertIt(dictionary, [new(0, 200)], 3);
    }

    [Fact]
    public void Remove()
    {
        using var dictionary = new NativeDictionary<int, int>(4);

        dictionary[1] = 100;
        dictionary[2] = 200;
        dictionary[3] = 300;
        dictionary[4] = 400;

        AssertIt(dictionary, [new(1, 100), new(2, 200), new(3, 300), new(4, 400)], 7);

        dictionary.Remove(2);

        AssertIt(dictionary, [new(1, 100), new(3, 300), new(4, 400)], 7);

        dictionary.Remove(4);

        AssertIt(dictionary, [new(1, 100), new(3, 300)], 7);

        dictionary[6] = 600;

        AssertIt(dictionary, [new(1, 100), new(3, 300), new(6, 600)], 7);

        dictionary[9] = 900;

        AssertIt(dictionary, [new(1, 100), new(3, 300), new(6, 600), new(9, 900)], 7);
    }

    [Fact]
    public void RemoveAndOut()
    {
        using var dictionary = new NativeDictionary<int, int>(4);

        dictionary[1] = 100;
        dictionary[2] = 200;
        dictionary[3] = 300;
        dictionary[4] = 400;

        AssertIt(dictionary, [new(1, 100), new(2, 200), new(3, 300), new(4, 400)], 7);

        Assert.True(dictionary.Remove(2, out var value) && value == 200);

        AssertIt(dictionary, [new(1, 100), new(3, 300), new(4, 400)], 7);
    }

    [Fact]
    public void DisposeShouldPass()
    {
        var list = new NativeDictionary<int, int>
        {
            [0] = 1,
            [1] = 2,
        };

        list.Dispose();
        list.Dispose();

        Assert.True(true);
    }
}
