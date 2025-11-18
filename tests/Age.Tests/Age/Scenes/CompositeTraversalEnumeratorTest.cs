using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public partial class CompositeTraversalEnumeratorTest
{
    private sealed class TestNode : Node
    {
        public override string NodeName => nameof(TestNode);
    }

    private static void AddChilds(Node parent, ref int parentDepth)
    {
        if (parentDepth > 0)
        {
            parentDepth--;

            for (var i = 0; i < 3; i++)
            {
                var child = new HostNode($"{parent.Name}.{i + 1}");

                parent.AppendChild(child);

                var depth = parentDepth;

                AddChilds(child, ref depth);
            }

            var slotted = new HostNode($"{parent.Name}.4");

            parent.AppendChild(slotted);
        }
    }

    [Fact]
    public void TraverseCompositeTree()
    {
        var tree = TreeFactory.Linear<HostNode>(static (name) => new(name), 1, 2, "$");

        var actual = new List<string>();

        var expected = new string[]
        {
            "$.#",
            "$.#.1",
            "$.#.1.1",
            "$.#.1.1.1",
            "$.#.1.1.2",
            "$.#.1.2",
            "$.#.1.2.1",
            "$.#.1.2.2",
            "$.1",
            "$.1.#",
            "$.1.#.1",
            "$.1.#.1.1",
            "$.1.#.1.1.1",
            "$.1.#.1.1.2",
            "$.1.#.1.2",
            "$.1.#.1.2.1",
            "$.1.#.1.2.2",
            "$.2",
            "$.2.#",
            "$.2.#.1",
            "$.2.#.1.1",
            "$.2.#.1.1.1",
            "$.2.#.1.1.2",
            "$.2.#.1.2",
            "$.2.#.1.2.1",
            "$.2.#.1.2.2",
        };

        var enumerator = new Node.CompositeTraversalEnumerator(tree);

        while (enumerator.MoveNext())
        {
            actual.Add(enumerator.Current.Name!);
        }

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SkipToNextSibling()
    {
        var tree = TreeFactory.Linear<HostNode>(static (name) => new(name), 1, 2, "$");

        var actual = new List<string>();

        var expected = new string[]
        {
            "$.#",
            "$.#.1",
            "$.#.1.1",
            "$.#.1.1.1",
            "$.#.1.1.2",
            "$.#.1.2",
            "$.#.1.2.1",
            "$.#.1.2.2",
            "$.2",
            "$.2.#",
            "$.2.#.1",
            "$.2.#.1.2",
            "$.2.#.1.2.1",
            "$.2.#.1.2.2",
        };

        var enumerator = new Node.CompositeTraversalEnumerator(tree);

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Name is "$.1" or "$.2.#.1.1")
            {
                enumerator.SkipToNextSibling();
            }
            else
            {
                actual.Add(enumerator.Current.Name!);
            }
        }

        Assert.Equal(expected, actual);
    }
}
