using Age.Scene;

namespace Age.Elements;

public sealed class Slot : Element
{
    public override string NodeName => nameof(Slot);
    internal List<Layoutable> Nodes { get; } = [];

    public Layoutable[] AssignedNodes    => [.. this.Nodes];
    public bool         HasAssignedNodes => this.Nodes.Count > 0;

    public sealed override string? Name
    {
        get => field;
        set
        {
            if (field != value)
            {
                if (this.Root is ShadowTree shadowTree)
                {
                    shadowTree.Host.RemoveSlot(this, field ?? "", true);
                    shadowTree.Host.AddSlot(this, value ?? "");
                }

                field = value;
            }
        }
    }

    protected override void Adopted(Node parent)
    {
        base.Adopted(parent);

        if (parent is ShadowTree shadowTree)
        {
            shadowTree.Host.AddSlot(this, this.Name ?? "");
        }
    }

    protected override void Removed(Node parent)
    {
        base.Removed(parent);

        if (parent is ShadowTree shadowTree)
        {
            shadowTree.Host.RemoveSlot(this, this.Name ?? "");
        }
    }

    internal void Assign(Layoutable layoutable)
    {
        if (this.Nodes.Count == 0)
        {
            foreach (var child in this)
            {
                if (child is Layoutable layoutableChild)
                {
                    this.Layout.LayoutableRemoved(layoutableChild);
                }
            }
        }

        layoutable.AssignedSlot = this;
        this.Nodes.Add(layoutable);
        this.Nodes.Sort();

        this.Layout.LayoutableAppended(layoutable);
    }

    internal void Unassign(Layoutable layoutable)
    {
        this.Nodes.Remove(layoutable);
        layoutable.AssignedSlot = null;

        this.Layout.LayoutableRemoved(layoutable);

        if (this.Nodes.Count == 0)
        {
            foreach (var child in this)
            {
                if (child is Layoutable layoutableChild)
                {
                    this.Layout.LayoutableAppended(layoutableChild);
                }
            }
        }
    }
}
