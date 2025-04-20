using Age.Scene;
using BenchmarkDotNet.Attributes;

#pragma warning disable CA1001

namespace Age.Benchmarks;

public class BenchNode : Node
{
    public override string NodeName => nameof(BenchNode);
}

[ShortRunJob]
[MemoryDiagnoser]
public class TraversalBenchmarks
{
    private static IEnumerable<Node> Traverse(Node node)
    {
        foreach (var child in node)
        {
            yield return child;

            foreach (var granChild in Traverse(child))
            {
                yield return granChild;
            }
        }
    }

    private readonly Stack<Node> stack = [];


    [Params(1, 5, 10)]
    public int Depth;

    private Node tree = null!;

    private static void AddChilds(Node parent, ref int parentDepth)
    {
        if (parentDepth > 0)
        {
            parentDepth--;

            for (var i = 0; i < 3; i++)
            {
                var child = new BenchNode();

                parent.AppendChild(child);

                var depth = parentDepth;

                AddChilds(child, ref depth);
            }
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        this.tree = new BenchNode();

        var current = this.tree;

        var depth = this.Depth;

        AddChilds(current, ref depth);
    }

    [Benchmark(Baseline = true)]
    public int NodeTraversalRecursive()
    {
        var count = 0;

        foreach (var node in Traverse(this.tree))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalIterator()
    {
        var count = 0;

        var enumerator = this.tree.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalIteratorInForeach()
    {
        var count = 0;

        foreach (var node in this.tree.GetTraversalEnumerator())
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int NodeTraversalStack()
    {
        var count = 0;
        this.stack.Push(this.tree);

        while (this.stack.Count > 0)
        {
            var current = this.stack.Pop();

            foreach (var node in current)
            {
                this.stack.Push(node);
            }

            count++;
        }

        return count;
    }
}
