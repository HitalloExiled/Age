using Age.Scene;

namespace Age.Elements;

public sealed class Slot : Node
{
    public override string NodeName { get; } = nameof(Slot);

    internal List<Node> Nodes { get; } = [];

    public Node[] AssignedNodes => [.. this.Nodes];

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
                    shadowTree.Host.Slots[value ?? ""] = this;
                }

                field = value;
            }
        }
    }

    internal void Add(Node node)    => this.Nodes.Add(node);
    internal void Remove(Node node) => this.Nodes.Remove(node);
}
