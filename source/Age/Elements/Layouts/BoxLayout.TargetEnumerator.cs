using System.Collections;

namespace Age.Elements.Layouts;

internal partial class BoxLayout
{
    internal struct TargetEnumerator : IEnumerator<Layoutable>
    {
        private Element.ComposedTreeEnumerator composedTreeEnumerator;
        private Element.SlotEnumerator         slotEnumerator;

        private readonly bool isSlot;

        public TargetEnumerator(Element target)
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
}
