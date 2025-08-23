using Age.Core.Extensions;

namespace Age.Elements;

internal partial record struct ComposedPath(List<Element> LeftToAncestor, List<Element> RightToAncestor)
{
    public readonly ReadOnlySpan<Element> GetElements()
    {
        var elements = new List<Element>();

        var enumerator = this.GetEnumerator();

        while (enumerator.MoveNext())
        {
            elements.Add(enumerator.Current);
        }

        return elements.AsReadOnlySpan();
    }

    public readonly Enumerator GetEnumerator() => new(this.LeftToAncestor.AsSpan(), this.RightToAncestor.AsSpan(0, int.Max(1, this.RightToAncestor.Count - 1)));
}
