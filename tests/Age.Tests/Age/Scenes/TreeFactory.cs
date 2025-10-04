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
    public static Node[] Flatten(Node root)
    {
        var nodes = new List<Node>
        {
            root
        };

        var traversal = root.GetTraversalEnumerator();

        while (traversal.MoveNext())
        {
            if (traversal.Current is Element element)
            {
                traversal.SkipToNextSibling();

                nodes.Add(element);

                var composedTraversal = element.GetComposedTreeTraversalEnumerator();

                while (composedTraversal.MoveNext())
                {
                    nodes.Add(composedTraversal.Current);
                }
            }
            else
            {
                nodes.Add(traversal.Current);
            }
        }

        return [..nodes];
    }
}

public static class TreeFactory<TNode, TBoundCommand, TCommand>
where TNode : Renderable<TBoundCommand>, new()
where TBoundCommand : Command
where TCommand : TBoundCommand, new()
{
    private static void AddCommand(TNode node, TCommand command) =>
        RenderableAcessor<TBoundCommand>.AddCommand((Renderable<TBoundCommand>)(Renderable)node, (TBoundCommand)(Command)command);

    public static TNode Wide(int childrenCount, int commandsCount, CommandFilter commandFilter = CommandFilter.Color)
    {
        var node = new TNode();

        for (var i = 0; i < childrenCount; i++)
        {
            node.AppendChild(new TNode());

            for (var j = 0; j < commandsCount; j++)
            {
                AddCommand(node, new TCommand() { CommandFilter = commandFilter });
            }
        }

        return node;
    }

    public static TNode Linear(int depth, int childrenCount = 0, int commandsCount = 0, byte? commandSeparator = null, CommandFilter commandFilter = CommandFilter.Color, string? name = "$")
    {
        var node = new TNode() { Name = name };

        for (var j = 0; j < commandsCount; j++)
        {
            AddCommand(node, new TCommand() { CommandFilter = commandFilter, });
        }

        if (node is Element element)
        {
            commandSeparator ??= (byte)commandsCount;

            ElementAcessor.GetCommandsSeparator(element) = commandSeparator.Value;
        }

        if (depth > 0)
        {
            --depth;

            for (var i = 0; i < childrenCount; i++)
            {
                node.AppendChild(Linear(depth, childrenCount, commandsCount, commandSeparator, commandFilter, $"{name}.{i + 1}"));
            }
        }

        return node;
    }
}
