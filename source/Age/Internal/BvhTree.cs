using Age.Numerics;
using Age.Rendering;
using Age.Rendering.Commands;
using Age.Rendering.Drawing;

namespace Age.Internal;

public unsafe class BvhTree
{
    private readonly BvhNode<ContainerNode> root = new();

    private static Rect<float> GetIntersection(ContainerNode node, in Rect<float> rect) =>
        rect.Intersection(new(node.Size.Cast<float>(), node.Transform.Position));

    private static void Split(BvhNode<ContainerNode> bvhNode, IList<ContainerNode> nodes)
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

        ContainerNode? largestNode = null;

        foreach (var node in nodes)
        {
            if (node.Size.Area > (largestNode?.Size.Area ?? 0))
            {
                largestNode = node;
            }

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

        if (leftAABB == bvhNode.AABB || rightAABB == bvhNode.AABB)
        {
            bvhNode.Elements = [..bvhNode.Elements, ..nodes];

            return;
        }

        if (largestNode?.Size.Area == leftAABB.Volume)
        {
            leftNodes.Remove(largestNode);

            bvhNode.Left = new()
            {
                Elements = [largestNode]
            };
        }
        else if (largestNode?.Size.Area == rightAABB.Volume)
        {
            rightNodes.Remove(largestNode);

            bvhNode.Right = new()
            {
                Elements = [largestNode]
            };
        }

        leftNodes.TrimExcess();
        rightNodes.TrimExcess();

        if (leftNodes.Count > 0)
        {
            bvhNode.Left ??= new();

            bvhNode.Left.AABB = leftAABB;

            Split(bvhNode.Left, leftNodes);
        }

        if (rightNodes.Count > 0)
        {
            bvhNode.Right ??= new();

            bvhNode.Right.AABB = rightAABB;

            Split(bvhNode.Right, rightNodes);
        }
    }

    public void Build(NodeTree tree)
    {
        var aabb  = new AABB<float>();
        var nodes = new List<ContainerNode>();

        ContainerNode? largest = null;

        foreach (var node in tree.Traverse<ContainerNode>(true))
        {
            if (node.Size.Area > (largest?.Size.Area ?? 0))
            {
                largest = node;
            }

            nodes.Add(node);

            aabb.Extends(new(node.Size, 1), new(node.Transform.Position.InvertedY, 0));
        }

        if (largest?.Size.Area == aabb.Volume)
        {
            nodes.Remove(largest);

            this.root.Elements = [largest];
        }

        nodes.TrimExcess();

        this.root.Left  = null;
        this.root.Right = null;

        this.root.AABB = aabb;

        Split(this.root, nodes);
    }

    internal static BvhDebugNode Draw(BvhNode<ContainerNode> bvhNode, Color color)
    {
        var node = new BvhDebugNode
        {
            Commands =
            [
                new RectDrawCommand
                {
                    Color          = color * new Color(1, 1, 1, 0.25f),
                    SampledTexture = new(Container.Singleton.TextureStorage.DefaultTexture, Container.Singleton.TextureStorage.DefaultSampler),
                    // Border         = new(2, 0, Color.Green),
                    Flags          = Rendering.Shaders.CanvasShader.Flags.ColorAsBackground,
                    Rect           = new(
                        bvhNode.AABB.Size.X,
                        bvhNode.AABB.Size.Y,
                        bvhNode.AABB.Position.X,
                        -bvhNode.AABB.Position.Y
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
}
