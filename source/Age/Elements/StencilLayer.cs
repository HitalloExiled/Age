using System.Collections;
using Age.Elements.Enumerators;
using Age.Numerics;
using Age.Shaders;

namespace Age.Elements;

internal class StencilLayer(Element owner) : IEnumerable<StencilLayer>
{
    public Element Owner { get; } = owner;

    public uint          Depth           { get; private set; }
    public StencilLayer? FirstChild      { get; private set; }
    public StencilLayer? LastChild       { get; private set; }
    public StencilLayer? NextSibling     { get; private set; }
    public StencilLayer? Parent          { get; private set; }
    public StencilLayer? PreviousSibling { get; private set; }

    public CanvasShader.Border Border    => this.Owner.ComputedStyle.Border ?? default(CanvasShader.Border);
    public Size<uint>          Size      => this.Owner.Boundings;
    public Transform2D         Transform => this.Owner.CachedTransformWithOffset;


    private void InvokeConnected()
    {
        this.OnConnected();

        var enumerator = new StencilLayerTraverseEnumerator(this);

        while (enumerator.MoveNext())
        {
            enumerator.Current.OnConnected();
        }
    }

    private void InvokeDisconnected()
    {
        this.OnDisconnected();

        var enumerator = new StencilLayerTraverseEnumerator(this);

        while (enumerator.MoveNext())
        {
            enumerator.Current.OnDisconnected();
        }
    }

    private void OnConnected() =>
        this.Depth = this.Parent!.Depth + 1;

    private void OnDisconnected() =>
        this.Depth = this.Parent != null ? this.Parent.Depth + 1 : 0;

    IEnumerator IEnumerable.GetEnumerator() =>
        this.GetEnumerator();

    IEnumerator<StencilLayer> IEnumerable<StencilLayer>.GetEnumerator() =>
        this.GetEnumerator();

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
                layer.PreviousSibling = this.LastChild;

                this.LastChild = layer;
            }
            else
            {
                this.FirstChild = this.LastChild = layer;
            }

            layer.Parent = this;

            layer.InvokeConnected();
        }
    }

    public void Detach() =>
        this.Parent?.RemoveChild(this);

    public void DelegateChildren()
    {
        if (this.Parent != null)
        {
            foreach (var node in this)
            {
                this.Parent.AppendChild(node);
            }
        }
    }

    public StencilLayerEnumerator GetEnumerator() =>
        new(this);

    public bool IsSibling(StencilLayer other) =>
        this.PreviousSibling == other || this.NextSibling == other;

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

        layer.NextSibling     = null;
        layer.Parent          = null;
        layer.PreviousSibling = null;

        layer.InvokeDisconnected();
    }

    public override string ToString() =>
        $"{{ Owner: {this.Owner} }}";
}
