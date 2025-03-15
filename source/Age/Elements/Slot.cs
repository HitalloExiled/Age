using Age.Scene;

namespace Age.Elements;

public sealed class Slot : Element
{
    public override string NodeName { get; } = nameof(Slot);

    internal List<Layoutable> Nodes { get; } = [];

    public Layoutable[] AssignedNodes => [.. this.Nodes];

    public sealed override string? Name
    {
        get => field;
        set
        {
            if (field != value)
            {
                if (this.Root is ShadowTree shadowTree)
                {
                    shadowTree.Host.Slots.Remove(field ?? "");
                    shadowTree.Host.Slots.TryAdd(value ?? "", this);
                }

                field = value;
            }
        }
    }

    protected override void Connected(NodeTree tree)
    {
        base.Connected(tree);

        if (this.Root is ShadowTree shadowTree)
        {
            shadowTree.Host.Slots.TryAdd(this.Name ?? "", this);
        }
    }

    protected override void Disconnected(NodeTree tree)
    {
        base.Disconnected(tree);

        if (this.Root is ShadowTree shadowTree)
        {
            shadowTree.Host.Slots.Remove(this.Name ?? "");
        }
    }

    internal void Assign(Layoutable layoutable)
    {
        layoutable.AssignedSlot = this;
        this.Nodes.Add(layoutable);
    }

    internal void Unassign(Layoutable layoutable)
    {
        this.Nodes.Remove(layoutable);
        layoutable.AssignedSlot = null;
    }
}
