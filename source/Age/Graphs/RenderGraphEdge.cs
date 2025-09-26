
namespace Age.Graphs;

public abstract class RenderGraphEdge
{
    public abstract RenderGraphNode From { get; }
    public abstract RenderGraphNode To   { get; }

    public abstract void Pipe();
    public abstract void Disconnect();

    public override string ToString() => $"{this.From} > {this.To}";
}

public sealed class RenderGraphEdge<TFrom, TTo, TValue>(TFrom from, TTo to, Func<TFrom, TValue> getter, Action<TTo, TValue?> setter) : RenderGraphEdge
where TFrom  : RenderGraphNode
where TTo    : RenderGraphNode
{
    public override TFrom From => from;
    public override TTo   To   => to;

    public override void Pipe() => setter.Invoke(this.To, getter.Invoke(this.From));
    public override void Disconnect()
    {
        this.From.RemoveOutput(this);
        this.To.RemoveInput(this);

        setter.Invoke(this.To, default);
    }
}
