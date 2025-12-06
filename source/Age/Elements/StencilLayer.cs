using Age.Numerics;
using Age.Shaders;
using System.Runtime.CompilerServices;

namespace Age.Elements;

internal partial class StencilLayer(Element owner)
{
    public Element Owner { get; } = owner;

    public ushort        Depth           { get; private set; }
    public StencilLayer? FirstChild      { get; private set; }
    public StencilLayer? LastChild       { get; private set; }
    public StencilLayer? NextSibling     { get; private set; }
    public StencilLayer? Parent          { get; private set; }
    public StencilLayer? PreviousSibling { get; private set; }

    public Geometry2DShader.Border Border    => this.Owner.ComputedStyle.Border ?? new Geometry2DShader.Border();
    public Size<uint>              Size      => this.Owner.Boundings;
    public Matrix3x2<float>        Transform => this.Owner.CachedMatrix;

    public bool IsLeaf => this.FirstChild == null;

    public bool IsConnected { get; private set; }

    private TraversalEnumerator GetTraversalEnumerator() =>
        new(this);

    private void PropagateOnConnected()
    {
        apply(this);

        var enumerator = this.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(StencilLayer layer)
        {
            layer.Depth       = (ushort)((layer.Parent?.Depth ?? -1) + 1);
            layer.IsConnected = true;
        }
    }

    private void PropagateOnDisconnecting()
    {
        apply(this);

        var enumerator = this.GetTraversalEnumerator();

        while (enumerator.MoveNext())
        {
            apply(enumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void apply(StencilLayer layer)
        {
            layer.Depth       = default;
            layer.IsConnected = default;
        }
    }

    public void AppendChild(StencilLayer layer)
    {
        if (layer == this)
        {
            throw new InvalidOperationException("Cant add node to itself");
        }

        if (layer.Parent != this)
        {
            layer.Parent?.RemoveChild(layer);

            if (this.LastChild != null)
            {
                this.LastChild.NextSibling = layer;
                layer.PreviousSibling      = this.LastChild;

                this.LastChild = layer;
            }
            else
            {
                this.FirstChild = this.LastChild = layer;
            }

            layer.Parent = this;

            if (this.IsConnected)
            {
                layer.PropagateOnConnected();
            }
        }
    }

    public void Detach() =>
        this.Parent?.RemoveChild(this);

    public void RemoveChild(StencilLayer layer)
    {
        if (layer.Parent != this)
        {
            throw new InvalidOperationException("Layer is not child of this layer");
        }

        if (layer.PreviousSibling == null)
        {
            this.FirstChild = layer.NextSibling;
        }
        else
        {
            layer.PreviousSibling.NextSibling = layer.NextSibling;
        }

        if (layer.NextSibling == null)
        {
            this.LastChild = layer.PreviousSibling;
        }
        else
        {
            layer.NextSibling.PreviousSibling = layer.PreviousSibling;
        }

        if (this.IsConnected)
        {
            layer.PropagateOnDisconnecting();
        }

        layer.NextSibling     = null;
        layer.Parent          = null;
        layer.PreviousSibling = null;
    }

    public override string ToString() =>
        $"{{ Owner: {this.Owner} }}";

    public void TryConnect()
    {
        if (this.IsConnected || this.Parent != null)
        {
            return;
        }

        this.PropagateOnConnected();
    }

    public void TryDisconnect()
    {
        if (!this.IsConnected || this.Parent != null)
        {
            return;
        }

        this.PropagateOnDisconnecting();
    }
}
