using Age.Core.Extensions;

namespace Age.Tests.Core.Extensions;

public class SpanExtensionTest
{
    private record Dummy(string Name, int Index);

    [Fact]
    public void TimSort()
    {
        int[] expected =
        [
            10, 10, 10, 10, 10, 10, 10, 10, 12, 12,
            12, 12, 12, 12, 12, 12, 23, 23, 23, 23,
            23, 23, 23, 23, 23, 23, 23, 23, 34, 34,
            34, 34, 34, 34, 34, 34, 34, 34, 34, 34,
            45, 45, 45, 45, 45, 45, 45, 45, 45, 45,
            45, 45, 56, 56, 56, 56, 56, 56, 56, 56,
            56, 56, 56, 56, 67, 67, 67, 67, 67, 67,
            67, 67, 67, 67, 67, 67, 78, 78, 78, 78,
            78, 78, 78, 78, 78, 78, 78, 78, 89, 89,
            89, 89, 89, 89, 89, 89, 89, 89, 89, 89,
        ];

        int[] actual =
        [
            12, 45, 67, 23, 89, 34, 56, 78, 10, 23,
            67, 45, 89, 78, 34, 56, 12, 23, 45, 67,
            89, 10, 34, 56, 78, 23, 45, 67, 89, 12,
            34, 56, 78, 10, 23, 45, 67, 89, 34, 56,
            78, 12, 23, 45, 67, 89, 10, 34, 56, 78,
            23, 45, 67, 89, 12, 34, 56, 78, 10, 23,
            45, 67, 89, 34, 56, 78, 12, 23, 45, 67,
            89, 10, 34, 56, 78, 23, 45, 67, 89, 12,
            34, 56, 78, 10, 23, 45, 67, 89, 34, 56,
            78, 12, 23, 45, 67, 89, 10, 34, 56, 78
        ];

        actual.TimSort();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TimSortWithComparer()
    {
        Dummy[] expected =
        [
            new Dummy("Whale",   10),
            new Dummy("Dolphin", 10),
            new Dummy("Rabbit",  10),
            new Dummy("Lion",    23),
            new Dummy("Bear",    23),
            new Dummy("Panther", 23),
            new Dummy("Horse",   23),
            new Dummy("Tiger",   45),
            new Dummy("Fox",     45),
            new Dummy("Leopard", 45),
            new Dummy("Zebra",   45),
            new Dummy("Wolf",    67),
            new Dummy("Hawk",    67),
            new Dummy("Cheetah", 67),
            new Dummy("Giraffe", 67),
            new Dummy("Shark",   78),
            new Dummy("Falcon",  78),
            new Dummy("Hippo",   78),
            new Dummy("Eagle",   89),
            new Dummy("Owl",     89),
        ];

        Dummy[] actual =
        [
            new Dummy("Lion",    23),
            new Dummy("Tiger",   45),
            new Dummy("Bear",    23), // repeated index
            new Dummy("Wolf",    67),
            new Dummy("Fox",     45), // repeated index
            new Dummy("Eagle",   89),
            new Dummy("Hawk",    67), // repeated index
            new Dummy("Shark",   78),
            new Dummy("Whale",   10),
            new Dummy("Dolphin", 10), // repeated index
            new Dummy("Panther", 23), // repeated index
            new Dummy("Leopard", 45), // repeated index
            new Dummy("Cheetah", 67), // repeated index
            new Dummy("Falcon",  78), // repeated index
            new Dummy("Owl",     89), // repeated index
            new Dummy("Rabbit",  10), // repeated index
            new Dummy("Horse",   23), // repeated index
            new Dummy("Zebra",   45), // repeated index
            new Dummy("Giraffe", 67), // repeated index
            new Dummy("Hippo",   78)  // repeated index
        ];

        actual.TimSort(static (left, right) => left.Index.CompareTo(right.Index));

        Assert.Equal(actual, expected);
    }
}
