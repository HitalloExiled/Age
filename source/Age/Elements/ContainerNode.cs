using Age.Elements.Layouts;
using Age.Scene;

namespace Age.Elements;

public abstract class ContainerNode : Node2D
{
    internal abstract Layout Layout { get; }
}
