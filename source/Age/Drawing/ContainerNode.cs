using Age.Numerics;
using Age.Scene;

namespace Age.Drawing;

public abstract class ContainerNode : Node2D
{
    private Size<uint> size;

    internal float Baseline { get; set; } = 1;

    public Size<uint> ContentSize { get; internal set; }

    public Size<uint> Size
    {
        get => this.size;
        internal set => this.Set(ref this.size, value);
    }
}