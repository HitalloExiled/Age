using System.Diagnostics;
using Age.Numerics;

namespace Age.Internal;

[DebuggerDisplay("\\{ AABB: AABB, Left: {Left?.AABB}, Right: {Right?.AABB} \\}")]
public record BvhNode<T>
{
    public AABB<float> AABB     { get; set; }
    public BvhNode<T>? Root     { get; set; }
    public BvhNode<T>? Left     { get; set; }
    public BvhNode<T>? Right    { get; set; }
    public T[]         Elements { get; set; } = [];
}
