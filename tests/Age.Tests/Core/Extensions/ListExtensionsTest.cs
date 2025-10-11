using Age.Core.Extensions;

namespace Age.Tests.Core.Extensions;

public class ListExtensionsTest
{
    [Fact]
    public void Replace()
    {
        List<int> source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        source.ReplaceRange(4..^4, [20, 20]);

        Assert.Equal([1, 2, 3, 4, 20, 20, 7, 8, 9, 10], source);

        source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        source.ReplaceRange(2..^2, [20, 20]);

        Assert.Equal([1, 2, 20, 20, 9, 10], source);

        source = [1, 2, 3, 5, 8 ,9, 10];

        source.ReplaceRange(1..^2, [40, 40, 40, 40, 40, 40, 40, 40]);

        Assert.Equal([1, 40, 40, 40, 40, 40, 40, 40, 40 ,9, 10], source);

        source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        source.ReplaceRange(1..3, [50, 50, 50, 50]);

        Assert.Equal([1, 50, 50, 50, 50, 4, 5, 6, 7, 8, 9, 10], source);

        source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        source.ReplaceRange(2..^2, [50]);

        Assert.Equal([1, 2, 50, 9, 10], source);

        source = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27];

        source.ReplaceRange(9..22, [-1, -2, -3, -4, -5]);

        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, -1, -2, -3, -4, -5, 23, 24, 25, 26, 27], source);
        // 9..22
                // [0..5] > [0..26]
    }
}
