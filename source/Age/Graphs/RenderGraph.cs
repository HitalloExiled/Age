using System.Diagnostics;

namespace Age.Graphs;

public sealed class RenderGraph
{
    private bool isDirty;

    private readonly Stack<RenderGraphEdge> edgeStack     = [];
    private readonly List<RenderGraphNode>  nodes         = [];
    private readonly List<RenderGraphNode>  nodeStack     = [];
    private readonly List<RenderGraphNode>  roots         = [];

    public IReadOnlyList<RenderGraphNode> Nodes         => this.nodes;

    private void AddNode(RenderGraphNode node)
    {
        if (node.RenderGraph == null)
        {
            this.nodes.Add(node);
            node.RenderGraph = this;
        }
        else if (node.RenderGraph != this)
        {
            throw new InvalidOperationException("Node already connected to another RenderGraph.");
        }

        this.isDirty = true;
    }

    private void SortExecution()
    {
        Debug.Assert(this.nodeStack.Count == 0);

        this.roots.Clear();

        foreach (var node in this.nodes)
        {
            if (node.InputEdges.IsEmpty)
            {
                this.roots.Add(node);
            }

            node.SortState = SortState.Unsorted;
        }

        this.nodes.Clear();

        foreach (var root in this.roots)
        {
            if (root.SortState == SortState.Sorted)
            {
                continue;
            }

            this.nodeStack.Add(root);

            while (this.nodeStack.Count > 0)
            {
                var node = this.nodeStack[^1];

                if (node.SortState == SortState.Sorted)
                {
                    this.nodeStack.RemoveAt(this.nodeStack.Count - 1);

                    continue;
                }

                var ready = true;

                if (node.SortState != SortState.Pending)
                {
                    var inputs = node.InputEdges;

                    for (var i = inputs.Length - 1; i >= 0; i--)
                    {
                        var edge = inputs[i];

                        if (edge.From.SortState != SortState.Sorted)
                        {
                            this.nodeStack.Remove(edge.From);
                            this.nodeStack.Add(edge.From);

                            ready = false;
                        }
                    }

                    node.SortState = SortState.Pending;
                }

                if (ready)
                {
                    this.nodeStack.RemoveAt(this.nodeStack.Count - 1);

                    Debug.Assert(!this.nodes.Contains(node));

                    this.nodes.Add(node);

                    node.SortState = SortState.Sorted;

                    var outputs = node.OutputEdges;

                    for (var i = outputs.Length - 1; i >= 0; i--)
                    {
                        var edge = outputs[i];

                        if (edge.To.SortState == SortState.Unsorted)
                        {
                            this.nodeStack.Add(edge.To);
                        }
                    }
                }
            }
        }

        this.nodeStack.Clear();
    }

    private void ThrowIfCiclic(RenderGraphEdge edge)
    {
        Debug.Assert(this.edgeStack.Count == 0);

        if (edge.To.OutputEdges.IsEmpty)
        {
            return;
        }

        var from = edge.From;

        this.edgeStack.Push(edge);

        while (this.edgeStack.Count > 0)
        {
            var current = this.edgeStack.Pop();

            if (current.To == from)
            {
                throw new RenderGraphCiclicException();
            }

            foreach (var item in current.To.OutputEdges)
            {
                this.edgeStack.Push(item);
            }
        }

        this.edgeStack.Clear();
    }

    public void Execute(RenderContext context)
    {
        if (this.isDirty)
        {
            this.SortExecution();

            this.isDirty = false;
        }

        foreach (var node in this.nodes)
        {
            node.CallExecute(context);
        }
    }

    public Connection<TFrom> Connect<TFrom>(TFrom node) where
    TFrom : RenderGraphNode
    {
        this.AddNode(node);

        return new(this, node);
    }

    public Connection<TFrom> Connect<TFrom, TTo, TValue>(TFrom from, TTo to)
    where TFrom  : RenderGraphNode<TValue>
    where TTo    : RenderGraphNode, IInputable<TValue> =>
        this.Connect(from, to, static x => x.Output, static (x, v) => x.Input = v);

    public Connection<TFrom> Connect<TFrom, TTo, TValue>(TFrom from, TTo to, Action<TTo, TValue?> setter)
    where TFrom  : RenderGraphNode<TValue>
    where TTo    : RenderGraphNode =>
        this.Connect(from, to, static x => x.Output, setter);

    public Connection<TFrom> Connect<TFrom, TTo, TValue>(TFrom from, TTo to, Func<TFrom, TValue> getter, Action<TTo, TValue?> setter)
    where TFrom  : RenderGraphNode
    where TTo    : RenderGraphNode
    {
        var edge = new RenderGraphEdge<TFrom, TTo, TValue>(from, to, getter, setter);

        this.ThrowIfCiclic(edge);

        this.AddNode(edge.From);
        this.AddNode(edge.To);

        edge.To.SetInput(edge);
        edge.From.SetOutput(edge);

        return new(this, from);
    }

    public void Disconnect(RenderGraphNode node)
    {
        this.nodes.Remove(node);

        foreach (var input in node.InputEdges)
        {
            input.Disconnect();
        }

        foreach (var output in node.OutputEdges)
        {
            output.Disconnect();
        }

        node.ClearInputs();
        node.ClearOutputs();
        node.RenderGraph = null;

        this.isDirty = true;
    }

    public void Disconnect(RenderGraphNode from, RenderGraphNode to)
    {
        var isDirty = this.isDirty;

        foreach (var edge in from.OutputEdges.ToArray())
        {
            if (edge.To == to)
            {
                edge.Disconnect();

                isDirty = true;
            }
        }

        this.isDirty = isDirty;
    }
}
