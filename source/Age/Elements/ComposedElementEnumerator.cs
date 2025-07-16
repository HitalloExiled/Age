using System.Collections;

namespace Age.Elements;

internal struct ComposedElementEnumerator : IEnumerator<Layoutable>
{
    private ComposedTreeEnumerator composedTreeEnumerator;
    private SlotEnumerator         slotEnumerator;

    private readonly bool isSlot;

    public ComposedElementEnumerator(Element target)
    {
        if (target is Slot slot && slot.HasAssignedNodes)
        {
            this.slotEnumerator = new(slot);
            this.isSlot         = true;
        }
        else
        {
            this.composedTreeEnumerator = new(target);
        }
    }

    public readonly Layoutable Current => this.isSlot ? this.slotEnumerator.Current : this.composedTreeEnumerator.Current;

    readonly object IEnumerator.Current => this.Current;

    public readonly void Dispose() { }

    public bool MoveNext() =>
        this.isSlot
            ? this.slotEnumerator.MoveNext()
            : this.composedTreeEnumerator.MoveNext();

    public void Reset()
    {
        if (this.isSlot)
        {
            this.slotEnumerator.Reset();
        }
        else
        {
            this.composedTreeEnumerator.Reset();
        }
    }
}
