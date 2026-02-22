using Age.Core.Extensions;

namespace Age.Scenes;

internal partial record struct ComposedPath(List<Node> LeftToAncestor, List<Node> RightToAncestor)
{
    public readonly ReadOnlySpan<Node> GetElements()
    {
        var elements = new List<Node>();

        var enumerator = this.GetEnumerator();

        while (enumerator.MoveNext())
        {
            elements.Add(enumerator.Current);
        }

        return elements.AsSpan();
    }

    public readonly Enumerator GetEnumerator() => new(this.LeftToAncestor.AsSpan(), this.RightToAncestor.AsSpan(0, int.Max(1, this.RightToAncestor.Count - 1)));
}
