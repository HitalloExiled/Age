using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

public record Ref<T> where T : struct
{
    public T Value { get; set; }

    public Ref(T value) => this.Value = value;

    public override string? ToString() =>
        this.Value.ToString();
}

public static class TreeFactory
{
    private static void AddCommand<TNode, TCommand>(TNode node, TCommand command)
    where TNode : Renderable<TCommand>
    where TCommand : Command =>
        RenderableAcessor<TCommand>.AddCommand(node, command);

    public static Node[] Flatten(Node root)
    {
        var nodes = new List<Node>
        {
            root
        };

        var traversal = root.GetCompositeTraversalEnumerator();

        while (traversal.MoveNext())
        {
            nodes.Add(traversal.Current);
        }

        return [..nodes];
    }

    public static TNode Linear<TNode>(int depth, int children = 0, string? name = "$") where TNode : Node, new() =>
        Linear(static name => new TNode() { Name = name }, depth, children, name);

    public static TNode Linear<TNode>(Func<string, TNode> factory, int depth, int children = 0, string? name = "$") where TNode : Node
    {
        var node = factory.Invoke(name!);

        if (depth > 0)
        {
            --depth;

            for (var i = 0; i < children; i++)
            {
                node.AppendChild(Linear(factory, depth, children, $"{name}.{i + 1}"));
            }
        }

        return node;
    }

    public static TNode Linear<TNode>(ReadOnlySpan<int> childrensPerDepth, string? name = "$") where TNode : Node, new() =>
        Linear(static name => new TNode() { Name = name }, childrensPerDepth, name);

    public static TNode Linear<TNode>(Func<string, TNode> factory, ReadOnlySpan<int> childrensPerDepth, string? name = "$") where TNode : Node
    {
        var node = factory.Invoke(name!);

        if (childrensPerDepth.Length > 0)
        {
            for (var i = 0; i < childrensPerDepth[0]; i++)
            {
                node.AppendChild(Linear(factory, childrensPerDepth[1..], $"{name}.{i + 1}"));
            }
        }

        return node;
    }

    public static TNode Linear<TNode, TBoundCommand, TCommand>(int depth, int childrenCount = 0, int commandsCount = 0, int commandSeparator = -1, string? name = "$")
    where TNode : Renderable<TBoundCommand>, new()
    where TBoundCommand : Command
    where TCommand : TBoundCommand, new() =>
        Linear<TNode, TBoundCommand, TCommand>(static name => new TNode { Name = name }, depth, childrenCount, commandsCount, commandSeparator, name);


    public static TNode Linear<TNode, TBoundCommand, TCommand>(Func<string, TNode> factory, int depth, int childrenCount = 0, int commandsCount = 0, int commandSeparator = -1, string? name = "$")
    where TNode : Renderable<TBoundCommand>
    where TBoundCommand : Command
    where TCommand : TBoundCommand, new()
    {
        var node = factory.Invoke(name!);

        for (var j = 0; j < commandsCount; j++)
        {
            AddCommand(node, Unsafe.As<TBoundCommand>(new TCommand()));
        }

        if (commandSeparator > -1)
        {
            RenderableAcessor<TBoundCommand>.SetCommandsSeparator(node, commandSeparator);
        }

        if (depth > 0)
        {
            --depth;

            for (var i = 0; i < childrenCount; i++)
            {
                node.AppendChild(Linear<TNode, TBoundCommand, TCommand>(factory, depth, childrenCount, commandsCount, commandSeparator, $"{name}.{i + 1}"));
            }
        }

        return node;
    }
}
