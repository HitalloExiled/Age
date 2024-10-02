using Age.Commands;
using Age.Core.Extensions;
using Age.Elements;
using Age.Styling;
using Age.Numerics;
using Age.Scene;
using Age.Storage;
namespace Age.Internal;

public unsafe class BvhTree
{
    private const int MAX_ELEMENTS = 4;
    private readonly BvhNode<ContainerNode> root = new();

    private static AABB<float> GetBounding(Span<ContainerNode> nodes, Dictionary<ContainerNode, int> depths)
    {
        var aabb = new AABB<float>();

        foreach (var node in nodes)
        {
            aabb.Extends(new(node.Layout.Size, 1), new(node.Transform.Position.InvertedY, depths[node]));
        }

        return aabb;
    }

    private static void Split(BvhNode<ContainerNode> bvhNode, Span<ContainerNode> nodes, Dictionary<ContainerNode, int> depths)
    {
        var particion = bvhNode.AABB.Size.X > bvhNode.AABB.Size.Y
            ? new AABB<float>(
                bvhNode.AABB.Size.X / 2,
                bvhNode.AABB.Size.Y,
                bvhNode.AABB.Size.Z,
                bvhNode.AABB.Position.X,
                bvhNode.AABB.Position.Y,
                bvhNode.AABB.Position.Z
            )
            : new AABB<float>(
                bvhNode.AABB.Size.X,
                bvhNode.AABB.Size.Y / 2,
                bvhNode.AABB.Size.Z,
                bvhNode.AABB.Position.X,
                bvhNode.AABB.Position.Y,
                bvhNode.AABB.Position.Z
            );

        var leftNodes  = new List<ContainerNode>();
        var rightNodes = new List<ContainerNode>();
        var leftAABB   = new AABB<float>();
        var rightAABB  = new AABB<float>();

        foreach (var node in nodes)
        {
            var aabb = new AABB<float>(new(node.Layout.Size, 1), new(node.Transform.Position.InvertedY, depths[node]));

            var intersection = particion.Intersection(aabb);

            if (intersection.Volume > aabb.Volume / 2)
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
            var sortedElements = leftNodes
                .OrderByDescending(static x => x.Layout.Size.Area)
                .ThenBy(static x => x.Transform.Position.X)
                .ToArray()
                .AsSpan();

            var elements = sortedElements[..MAX_ELEMENTS];
            var remains  = sortedElements[MAX_ELEMENTS..];

            bvhNode.Left = new()
            {
                AABB = leftAABB,
                Left = new()
                {
                    AABB     = GetBounding(elements, depths),
                    Elements = elements.ToArray(),
                },
            };

            if (remains.Length > 0)
            {
                bvhNode.Left.Right = new()
                {
                    AABB = GetBounding(remains, depths),
                };

                Split(bvhNode.Left.Right, remains, depths);
            }
        }
        else if (leftNodes.Count > MAX_ELEMENTS)
        {
            bvhNode.Left = new()
            {
                AABB = leftAABB
            };

            Split(bvhNode.Left, leftNodes.AsSpan(), depths);
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
            var sortedElements = rightNodes
                .OrderByDescending(static x => x.Layout.Size.Area)
                .ThenByDescending(static x => x.Transform.Position.X)
                .ToArray()
                .AsSpan();

            var elements = sortedElements[..MAX_ELEMENTS];
            var remains  = sortedElements[MAX_ELEMENTS..];

            bvhNode.Right = new()
            {
                AABB  = rightAABB,
                Right = new()
                {
                    AABB     = GetBounding(elements, depths),
                    Elements = elements.ToArray(),
                }
            };

            if (remains.Length > 0)
            {
                bvhNode.Right.Left = new()
                {
                    AABB = GetBounding(remains, depths),
                };

                Split(bvhNode.Right.Left, remains, depths);
            }
        }
        else if (rightNodes.Count > MAX_ELEMENTS)
        {
            bvhNode.Right = new()
            {
                AABB = rightAABB
            };

            Split(bvhNode.Right, rightNodes.AsSpan(), depths);
        }

#if DEBUG
        if (bvhNode.Left != null && bvhNode.Right != null && bvhNode.Elements.Length > 0 || bvhNode.Elements.Length > MAX_ELEMENTS)
        {
            throw new Exception();
        }
#endif
    }

    private static IEnumerable<(ContainerNode, int)> Traverse(Node node, int depth = 0)
    {
        foreach (var child in node)
        {
            if (child is ContainerNode containerNode)
            {
                yield return (containerNode, depth);
            }

            foreach (var pair in Traverse(child, depth + 1))
            {
                yield return pair;
            }
        }
    }

    internal static BvhDebugNode Draw(BvhNode<ContainerNode> bvhNode, Color color)
    {
        var node = new BvhDebugNode
        {
            Commands =
            [
                new RectCommand
                {
                    Border         = new Border(2, 0, color * new Color(1, 1, 1, 1)),
                    Flags          = Rendering.Shaders.Canvas.CanvasShader.Flags.ColorAsBackground,
                    Rect           = new(
                        bvhNode.AABB.Size.X,
                        bvhNode.AABB.Size.Y,
                        bvhNode.AABB.Position.X,
                        -bvhNode.AABB.Position.Y
                    ),
                    SampledTexture = new(
                        TextureStorage.Singleton.DefaultTexture,
                        TextureStorage.Singleton.DefaultSampler,
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
        var depths = new Dictionary<ContainerNode, int>();

        var aabb  = new AABB<float>();
        var nodes = new List<ContainerNode>();

        foreach (var (node, depth) in Traverse(tree.Root))
        {
            if (node is not Element element || element.Style.Border != null)
            {
                depths[node] = depth;
                nodes.Add(node);

                aabb.Extends(new(node.Layout.Size, 1), new(node.Transform.Position.InvertedY, depth));
            }
        }

        nodes.TrimExcess();

        this.root.Left  = null;
        this.root.Right = null;

        this.root.AABB = aabb;

        if (nodes.Count > 0)
        {
            Split(this.root, nodes.AsSpan(), depths);
        }
    }
}
