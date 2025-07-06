using Age.Scene;

namespace Age.Elements;

public sealed class Slot : Element
{
    public override string NodeName => nameof(Elements.Slot);
    internal List<Layoutable> Nodes { get; } = [];

    public Layoutable[] AssignedNodes    => [.. this.Nodes];
    public bool         HasAssignedNodes => this.Nodes.Count > 0;

    public override string? Name
    {
        get => field;
        set
        {
            if (field != value)
            {
                if (this.Root is ShadowTree shadowTree)
                {
                    shadowTree.RemoveSlot(this, field ?? "", true);
                    shadowTree.AddSlot(this, value ?? "");
                }

                field = value;
            }
        }
    }

    protected override void OnConnected(NodeTree tree)
    {
        base.OnConnected(tree);

        if (this.Root is ShadowTree shadowTree)
        {
            shadowTree.AddSlot(this, this.Name ?? "");
        }
    }

    protected override void OnRemoved(Node parent)
    {
        base.OnRemoved(parent);

        if (parent.Root is ShadowTree shadowTree)
        {
            shadowTree.RemoveSlot(this, this.Name ?? "");
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
                    this.Layout.HandleLayoutableRemoved(layoutableChild);
                }
            }
        }

        layoutable.AssignedSlot = this;
        this.Nodes.Add(layoutable);
        this.Nodes.Sort();

        this.Layout.HandleLayoutableAppended(layoutable);
    }

    internal void Unassign(Layoutable layoutable)
    {
        this.Nodes.Remove(layoutable);
        layoutable.AssignedSlot = null;

        this.Layout.HandleLayoutableRemoved(layoutable);

        if (this.Nodes.Count == 0)
        {
            foreach (var child in this)
            {
                if (child is Layoutable layoutableChild)
                {
                    this.Layout.HandleLayoutableAppended(layoutableChild);
                }
            }
        }
    }
}
