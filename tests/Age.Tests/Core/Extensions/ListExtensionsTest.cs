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
    }
}
