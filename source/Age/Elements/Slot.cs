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

    private protected override void OnConnectedInternal()
    {
        base.OnConnectedInternal();

        if (this.Root is ShadowTree shadowTree)
        {
            shadowTree.AddSlot(this, this.Name ?? "");
        }
    }

    private protected override void OnDetachingInternal()
    {
        base.OnDetachingInternal();

        if (this.Parent!.Root is ShadowTree shadowTree)
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
                    this.HandleLayoutableRemoved(layoutableChild);
                }
            }
        }

        layoutable.AssignedSlot = this;
        this.Nodes.Add(layoutable);
        this.Nodes.Sort();

        this.HandleLayoutableAppended(layoutable);
    }

    internal void Unassign(Layoutable layoutable)
    {
        this.Nodes.Remove(layoutable);
        layoutable.AssignedSlot = null;

        this.HandleLayoutableRemoved(layoutable);

        if (this.Nodes.Count == 0)
        {
            foreach (var child in this)
            {
                if (child is Layoutable layoutableChild)
                {
                    this.HandleLayoutableAppended(layoutableChild);
                }
            }
        }
    }
}
