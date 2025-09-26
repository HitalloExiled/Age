using Age.Core;
using Age.Core.Extensions;

namespace Age.Graphs;

public abstract class RenderGraphNode : Disposable
{
    private readonly List<RenderGraphEdge> inputEdges  = [];
    private readonly List<RenderGraphEdge> outputEdges = [];

    internal RenderGraph? RenderGraph
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;

                if (value != null)
                {
                    this.OnConnected();
                }
                else
                {
                    this.OnDisconnected();
                }
            }
        }
    }
    internal SortState    SortState   { get; set; }

    public ReadOnlySpan<RenderGraphEdge> InputEdges  => this.inputEdges.AsSpan();
    public ReadOnlySpan<RenderGraphEdge> OutputEdges => this.outputEdges.AsSpan();

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }
    protected virtual void OnConnected() { }
    protected virtual void OnDisconnected() { }

    protected abstract void Execute(RenderContext context);

    internal virtual void ClearInputs()
    {
        this.inputEdges.Clear();
    }

    internal virtual void ClearOutputs()
    {
        this.outputEdges.Clear();
    }

    internal void CallExecute(RenderContext context)
    {
        this.BeforeExecute();
        this.Execute(context);
        this.AfterExecute();

        foreach (var edge in this.OutputEdges)
        {
            edge.Pipe();
        }
    }

    internal void RemoveInput(RenderGraphEdge edge) =>
        this.inputEdges.Remove(edge);

    internal void RemoveOutput(RenderGraphEdge edge) =>
        this.outputEdges.Remove(edge);

    internal void SetInput(RenderGraphEdge edge) =>
        this.inputEdges.Add(edge);

    internal void SetOutput(RenderGraphEdge edge) =>
        this.outputEdges.Add(edge);

    public override string ToString() =>
        this.GetType().Name;
}

public abstract class RenderGraphNode<TOutput> : RenderGraphNode, IOutputable<TOutput>
{
    public abstract TOutput? Output { get; }
}

public abstract class RenderGraphNode<TInput, TOutput> : RenderGraphNode<TOutput>, IInputable<TInput>
{
    public abstract TInput? Input { get; set; }

    internal override void ClearInputs()
    {
        base.ClearInputs();

        this.Input = default;
    }
}
