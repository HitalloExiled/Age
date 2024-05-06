using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;

namespace Age.Internal;

public unsafe class BvhTree
{
    private const int MAX_ELEMENTS = 2;
    private readonly BvhNode<ContainerNode> root = new();

    private static Rect<float> GetIntersection(ContainerNode node, in Rect<float> rect) =>
        rect.Intersection(new(node.Size.Cast<float>(), node.Transform.Position));

    private static AABB<float> GetBounding(Span<ContainerNode> nodes)
    {
        var aabb = new AABB<float>();

        foreach (var node in nodes)
        {
            aabb.Extends(new(node.Size, 1), new(node.Transform.Position.InvertedY, 0));
        }

        return aabb;
    }

    private static void Split(BvhNode<ContainerNode> bvhNode, Span<ContainerNode> nodes)
    {
        var rect = bvhNode.AABB.Size.X > bvhNode.AABB.Size.Y
            ? new Rect<float>(
                bvhNode.AABB.Size.X / 2,
                bvhNode.AABB.Size.Y,
                bvhNode.AABB.Position.X,
                bvhNode.AABB.Position.Y
            )
            : new Rect<float>(
                bvhNode.AABB.Size.X,
                bvhNode.AABB.Size.Y / 2,
                bvhNode.AABB.Position.X,
                bvhNode.AABB.Position.Y
            );

        var leftNodes  = new List<ContainerNode>();
        var rightNodes = new List<ContainerNode>();
        var leftAABB   = new AABB<float>();
        var rightAABB  = new AABB<float>();

        foreach (var node in nodes)
        {
            var intersection = GetIntersection(node, rect);

            var aabb = new AABB<float>(new(node.Size, 1), new(node.Transform.Position.InvertedY, 0));

            if (intersection.Area > aabb.Volume / 2)
            {
                leftNodes.Add(node);
                leftAABB.Extends(aabb);
            }
            else
            {
                rightNodes.Add(node);
                rightAABB.Extends(aabb);
            }
        }

        leftNodes.TrimExcess();
        rightNodes.TrimExcess();

        if (leftNodes.Count is > 0 and <= MAX_ELEMENTS)
        {
            bvhNode.Left = new()
            {
                AABB     = leftAABB,
                Elements = [..leftNodes]
            };
        }
        else if (leftAABB == bvhNode.AABB)
        {
            var sortedElements = leftNodes.OrderBy(x => x.Transform.Position.X).ToArray().AsSpan();

            var size = Math.Min(MAX_ELEMENTS, sortedElements.Length);

            var elements = sortedElements[..size];
            var remains  = sortedElements[size..];

            bvhNode.Left = new()
            {
                AABB = leftAABB,
                Left = new()
                {
                    AABB     = GetBounding(elements),
                    Elements = elements.ToArray(),
                },
            };

            if (remains.Length > 0)
            {
                bvhNode.Left.Right = new()
                {
                    AABB = GetBounding(remains),
                };

                Split(bvhNode.Left.Right, remains);
            }
        }
        else if (leftNodes.Count > MAX_ELEMENTS)
        {
            bvhNode.Left = new()
            {
                AABB = leftAABB
            };

            Split(bvhNode.Left, leftNodes.AsSpan());
        }


        if (rightNodes.Count is > 0 and <= MAX_ELEMENTS)
        {
            bvhNode.Right = new()
            {
                AABB     = rightAABB,
                Elements = [..rightNodes]
            };
        }
        else if (rightAABB == bvhNode.AABB)
        {
            var sortedElements = rightNodes.OrderByDescending(x => x.Transform.Position.X).ToArray().AsSpan();

            var size = Math.Min(MAX_ELEMENTS, sortedElements.Length);

            var elements = sortedElements[..size];
            var remains  = sortedElements[size..];

            bvhNode.Right = new()
            {
                AABB  = rightAABB,
                Right = new()
                {
                    AABB     = GetBounding(elements),
                    Elements = elements.ToArray(),
                }
            };

            if (remains.Length > 0)
            {
                bvhNode.Right.Left = new()
                {
                    AABB = GetBounding(remains),
                };

                Split(bvhNode.Right.Left, remains);
            }
        }
        else if (rightNodes.Count > MAX_ELEMENTS)
        {
            bvhNode.Right = new()
            {
                AABB = rightAABB
            };

            Split(bvhNode.Right, rightNodes.AsSpan());
        }
    }

    internal static BvhDebugNode Draw(BvhNode<ContainerNode> bvhNode, Color color)
    {
        if (bvhNode.Left != null && bvhNode.Right != null && bvhNode.Elements.Length > 0)
        {
            throw new Exception();
        }

        var node = new BvhDebugNode
        {
            Commands =
            [
                new RectDrawCommand
                {
                    Border         = new(2, 0, color * new Color(1, 1, 1, 1)),
                    Flags          = Rendering.Shaders.CanvasShader.Flags.ColorAsBackground,
                    Rect           = new(
                        bvhNode.AABB.Size.X,
                        bvhNode.AABB.Size.Y,
                        bvhNode.AABB.Position.X,
                        -bvhNode.AABB.Position.Y
                    ),
                    SampledTexture = new(
                        Container.Singleton.TextureStorage.DefaultTexture,
                        Container.Singleton.TextureStorage.DefaultSampler,
                        UVRect.Normalized
                    ),
                }
            ],
        };

        if (bvhNode.Left != null)
        {
            node.AppendChild(Draw(bvhNode.Left, Color.Red));
        }

        if (bvhNode.Right != null)
        {
            node.AppendChild(Draw(bvhNode.Right, Color.Blue));
        }

        return node;
    }

    internal BvhDebugNode Draw() =>
        Draw(this.root, Color.Green);

    public void Build(NodeTree tree)
    {
        var aabb  = new AABB<float>();
        var nodes = new List<ContainerNode>();

        foreach (var node in tree.Traverse<ContainerNode>(true))
        {
            if (node is not Element element || element.Style.Border != null)
            {
                nodes.Add(node);
            }

            aabb.Extends(new(node.Size, 1), new(node.Transform.Position.InvertedY, 0));
        }

        nodes.TrimExcess();

        this.root.Left  = null;
        this.root.Right = null;

        this.root.AABB = aabb;

        Split(this.root, nodes.AsSpan());
    }
}
