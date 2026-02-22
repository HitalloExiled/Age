using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public class TraversalEnumeratorTest
{
    [Fact]
    public void Enumerate()
    {
        var tree = TreeFactory.Linear<TestNode>(static name => new(name), 2, 3);

        var enumerator = new Node.TraversalEnumerator(tree);

        var nodes = new List<string>();

        string[] expected =
        [
            "$.1",
            "$.1.1",
            "$.1.2",
            "$.1.3",
            "$.2",
            "$.2.1",
            "$.2.2",
            "$.2.3",
            "$.3",
            "$.3.1",
            "$.3.2",
            "$.3.3",
        ];

        while (enumerator.MoveNext())
        {
            nodes.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, nodes);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        var tree = TreeFactory.Linear<TestNode>(static name => new(name), 2, 3);

        var enumerator = new Node.TraversalEnumerator(tree);

        var nodes = new List<string>();

        string[] expected =
        [
            "$.1",
            "$.1.1",
            "$.1.2",
            "$.1.3",
            "$.3",
            "$.3.1",
            "$.3.2",
            "$.3.3",
        ];

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Name == "$.2")
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                nodes.Add(enumerator.Current.Name!);
            }
        }

        Assert.Equal(expected, nodes);
    }
}
