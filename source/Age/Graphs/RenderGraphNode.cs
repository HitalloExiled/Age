using Age.Core;
using Age.Core.Extensions;
using Age.Scenes;

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
            if (field == value)
            {
                return;
            }

            if (value != null)
            {
                field = value;

                this.OnConnected();
            }
            else
            {
                this.OnDisconnecting();

                field = value;
            }
        }
    }

    internal SortState SortState { get; set; }

    public bool IsConnected => this.RenderGraph != null;

    public ReadOnlySpan<RenderGraphEdge> InputEdges  => this.inputEdges.AsSpan();
    public ReadOnlySpan<RenderGraphEdge> OutputEdges => this.outputEdges.AsSpan();

    public Viewport? Viewport => this.RenderGraph?.Viewport;

    public abstract string Name { get; }

    protected virtual void AfterExecute() { }
    protected virtual void BeforeExecute() { }
    protected virtual void OnConnected() { }
    protected virtual void OnDisconnecting() { }

    protected abstract void Execute();

    internal virtual void ClearInputs() =>
        this.inputEdges.Clear();

    internal virtual void ClearOutputs() =>
        this.outputEdges.Clear();

    internal void CallExecute()
    {
        this.BeforeExecute();
        this.Execute();
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
