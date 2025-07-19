using Age.Scene;

namespace Age.Elements;

public sealed class ShadowTree(Element host, bool inheritsHostStyle) : Node
{
    public override string NodeName => nameof(ShadowTree);

    private readonly Dictionary<string, Slot> slots   = [];
    private readonly List<Layoutable>         waiting = [];

    internal bool InheritsHostStyle => inheritsHostStyle;

    public Element Host { get; } = host;

    protected override void OnChildAppended(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.HandleLayoutableAppended(layoutable);
        }
    }

    protected override void OnChildRemoved(Node child)
    {
        if (child is Layoutable layoutable)
        {
            this.Host.HandleLayoutableRemoved(layoutable);
        }
    }

    internal void AddSlot(Slot slot, string name)
    {
        if (this.slots.TryAdd(name, slot))
        {
            foreach (var node in this.waiting.ToArray())
            {
                if ((node.Slot ?? "") == name)
                {
                    slot.Assign(node);

                    this.waiting.Remove(node);
                }
            }
        }
    }

    internal void AssignSlot(string name, Layoutable layoutable)
    {
        if (this.slots.TryGetValue(name, out var slot))
        {
            this.waiting.Remove(layoutable);

            slot.Assign(layoutable);
        }
        else if (!this.waiting.Contains(layoutable))
        {
            this.waiting.Add(layoutable);
        }
    }

    internal void RemoveSlot(Slot slot, string name, bool preserveAssignedNodes = false)
    {
        if (this.slots.TryGetValue(name, out var stored) && stored == slot)
        {
            if (!preserveAssignedNodes && slot.Nodes.Count > 0)
            {
                foreach (var node in slot.Nodes.ToArray())
                {
                    slot.Unassign(node);

                    this.waiting.Add(node);
                }
            }

            this.slots.Remove(name);
        }
    }

    internal void UnassignSlot(string name, Layoutable layoutable)
    {
        if (this.slots.TryGetValue(name, out var slot))
        {
            slot.Unassign(layoutable);

            this.waiting.Add(layoutable);
        }
    }
}
