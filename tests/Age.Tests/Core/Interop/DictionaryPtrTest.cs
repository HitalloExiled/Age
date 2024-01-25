using System.Collections;
using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public class DictionaryPtrTest
{
    private const string ONE   = "one";
    private const string TWO   = "Two";
    private const string THREE = "Three";

    [Fact]
    public unsafe void Add()
    {
        Console.WriteLine(sizeof(Ptr<int>));

        var dictionary = new DictionaryPtr<string, int>
        {
            { ONE, 1 },
            { TWO, 2 },
            { THREE, 3 }
        };

        Assert.Equal(3, dictionary.Count);

        Assert.Equal(1, *dictionary.Get(ONE));
        Assert.Equal(2, *dictionary.Get(TWO));
        Assert.Equal(3, *dictionary.Get(THREE));
    }

    [Fact]
    public unsafe void Remove()
    {
        var dictionary = new DictionaryPtr<string, int>
        {
            { ONE, 1 },
            { TWO, 2 },
            { THREE, 3 }
        };

        Assert.Equal(3, dictionary.Count);

        dictionary.Remove(ONE);

        Assert.Equal(2, dictionary.Count);
    }

    [Fact]
    public unsafe void GetEnumerator()
    {
        var dictionary = new DictionaryPtr<string, int>
        {
            { ONE, 1 },
            { TWO, 2 },
            { THREE, 3 }
        };

        var actual = new int[3];

        var i = 0;

        foreach (var item in (IEnumerable)dictionary)
        {
            actual[i++] = ((KeyValuePair<string, Ptr<int>>)item).Value.Value;
        }

        Assert.Equal([1, 2, 3], actual);
    }

    [Fact]
    public unsafe void GetGenericEnumerator()
    {
        var dictionary = new DictionaryPtr<string, int>
        {
            { ONE, 1 },
            { TWO, 2 },
            { THREE, 3 }
        };

        var actual = new int[3];

        var i = 0;

        foreach (var item in dictionary)
        {
            actual[i++] = item.Value;
        }

        Assert.Equal([1, 2, 3], actual);
    }



    [Fact]
    public unsafe void Dispose()
    {
        var dictionary = new DictionaryPtr<string, int>
        {
            { ONE, 1 },
            { TWO, 2 },
            { THREE, 3 }
        };

        Assert.Equal(3, dictionary.Count);

        dictionary.Dispose();

        Assert.Equal(0, dictionary.Count);
    }
}
