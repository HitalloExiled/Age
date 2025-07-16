using System.Collections;

namespace Age.Elements;

internal struct SlotEnumerator(Slot target) : IEnumerator<Layoutable>
{
    private readonly Slot target = target;
    private int index = -1;

    public readonly Layoutable Current => this.target.Nodes[this.index];

    readonly object IEnumerator.Current => this.Current;

    public readonly void Dispose() { }
    public bool MoveNext() => ++this.index < this.target.Nodes.Count;
    public void Reset() => this.index = -1;
}
