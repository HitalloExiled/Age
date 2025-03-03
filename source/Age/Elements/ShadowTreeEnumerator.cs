using System.Collections;
using System.Runtime.CompilerServices;
using Age.Scene;

namespace Age.Elements;

public struct ShadowTreeEnumerator(Element target) : IEnumerator<Layoutable>
{
    private Node? current;
    private Slot? slot;

    private int slotIndex;
    private int state;

    public readonly Layoutable Current => (this.current as Layoutable)!;

    readonly object IEnumerator.Current => this.Current;

    public readonly void Dispose() { }

    public bool MoveNext()
    {
        switch (this.state)
        {
            case 0:
                if (target.ShadowTree != null)
                {
                    this.current = getNextLayoutableOrSlot(target.ShadowTree.FirstChild);
                    this.state   = 1;
                }
                else
                {
                    this.current = getNextLayoutable(target.FirstChild);
                    this.state   = 3;
                }

                break;
            case 1:
                this.current = getNextLayoutableOrSlot(this.current!.NextSibling);

                while (this.current != null && this.current is not (Layoutable or Slot))
                {
                    this.current = this.current.NextSibling;
                }

                if (this.current == null)
                {
                    this.current = target.FirstChild;
                    this.state   = 3;
                }
                else if (this.current is Slot slot && slot.Nodes.Count > 0)
                {
                    this.slotIndex = 0;
                    this.slot      = slot;
                    this.current   = slot.Nodes[this.slotIndex];
                    this.state     = 2;
                }

                break;
            case 2:
                if (this.current == null)
                {
                    this.current = this.slot!.NextSibling;
                    this.slot    = null;
                    this.state   = 1;
                }
                else
                {
                    for (++this.slotIndex; this.slotIndex < this.slot!.Nodes.Count; this.slotIndex++)
                    {
                        if (this.slot.Nodes[this.slotIndex] is Layoutable)
                        {
                            this.current = this.slot.Nodes[this.slotIndex];

                            break;
                        }
                    }
                }

                break;
            case 3:
                this.current = getNextLayoutable(this.current!.NextSibling);

                break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Node? getNextLayoutable(Node? node)
        {
            while (node != null && (node is not Layoutable layoutable || layoutable.AssignedSlot != null))
            {
                node = node.NextSibling;
            }

            return node;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Node? getNextLayoutableOrSlot(Node? node)
        {
            while (node != null && node is not (Slot or Layoutable))
            {
                node = node.NextSibling;
            }

            return node;
        }

        return this.current != null;
    }

    public void Reset()
    {
        this.slot    = null;
        this.current = null;
        this.state   = 0;
    }
}
