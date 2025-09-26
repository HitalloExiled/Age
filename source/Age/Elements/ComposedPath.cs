using Age.Core.Extensions;

namespace Age.Elements;

internal partial record struct ComposedPath(List<Layoutable> LeftToAncestor, List<Layoutable> RightToAncestor)
{
    public readonly ReadOnlySpan<Layoutable> GetElements()
    {
        var elements = new List<Layoutable>();

        var enumerator = this.GetEnumerator();

        while (enumerator.MoveNext())
        {
            elements.Add(enumerator.Current);
        }

        return elements.AsSpan();
    }

    public readonly Enumerator GetEnumerator() => new(this.LeftToAncestor.AsSpan(), this.RightToAncestor.AsSpan(0, int.Max(1, this.RightToAncestor.Count - 1)));
}
