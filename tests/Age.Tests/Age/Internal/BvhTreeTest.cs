using Age.Internal;
using Age.Numerics;
using Age.Rendering.Drawing;
using Age.Rendering.Drawing.Elements;
using Age.Rendering.Interfaces;

namespace Age.Tests.Age.Internal;

public class BvhTreeTest
{
    [Fact]
    public void Build()
    {
        var textureStorageMock   = new Mock<ITextureStorage>();
        var textServiceMock      = new Mock<ITextService>();

        using var container = new global::Age.Rendering.Container()
        {
            TextService      = textServiceMock.Object,
            TextureStorage   = textureStorageMock.Object,
        };

        var windowMock = new Mock<IWindow>();

        windowMock.SetupAdd(x => x.SizeChanged += It.IsAny<Action>());
        windowMock.SetupGet(x => x.ClientSize).Returns(new Size<uint>(800, 600));


        var window = windowMock.Object;

        var nodeTree = new NodeTree(window);
        var canvas   = new Canvas();

        var root   = new Span() { Name = "Root", Style = new() { Size = new(400, 200) } };
        var rootC1 = new Span() { Name = "Root->C1", Style = new() { Size = new(20) } };
        var rootC2 = new Span() { Name = "Root->C2", Style = new() { Size = new(40) } };
        var rootC3 = new Span() { Name = "Root->C3", Style = new() { Size = new(40) } };

        var rootC3C1 = new Span() { Name = "Root->C3->C1", Style = new() { Size = new(40) } };
        var rootC3C2 = new Span() { Name = "Root->C3->C2", Style = new() { Size = new(40), Position = new(0, -10) } };

        nodeTree.AppendChild(canvas);
        canvas.AppendChild(root);
        root.AppendChildren([rootC1, rootC2, rootC3]);
        rootC3C1.AppendChildren([rootC3C1, rootC3C2]);

        canvas.Update(0);

        var bvhTree = new BvhTree();

        bvhTree.Build(nodeTree);
    }
}
