namespace Age.Graphs;

public readonly struct Connection<TFrom> where TFrom : RenderGraphNode
{
    private readonly RenderGraphPipeline pipeline;
    public TFrom From { get; }

    internal Connection(RenderGraphPipeline pipeline, TFrom from)
    {
        this.pipeline = pipeline;
        this.From     = from;
    }

    public Connection<TTo> Connect<TTo, TValue>(TTo to)
    where TTo    : RenderGraphNode, IInputable<TValue>
    where TValue : new()
    {
        this.pipeline.Connect<RenderGraphNode<TValue>, TTo, TValue>((RenderGraphNode<TValue>)(RenderGraphNode)this.From, to);

        return new(this.pipeline, to);
    }

    public Connection<TTo> Connect<TTo, TValue>(TTo to, Action<TTo, TValue?> setter)
    where TTo    : RenderGraphNode
    where TValue : new()
    {
        this.pipeline.Connect((RenderGraphNode<TValue>)(RenderGraphNode)this.From, to, setter);

        return new(this.pipeline, to);
    }

    public Connection<TTo> Connect<TTo, TValue>(TTo to, Func<TFrom, TValue> getter, Action<TTo, TValue?> setter)
    where TTo    : RenderGraphNode
    where TValue : new()
    {
        this.pipeline.Connect(this.From, to, getter, setter);

        return new(this.pipeline, to);
    }
}
