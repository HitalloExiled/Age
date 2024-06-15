using Age.Rendering.Drawing;
using Age.Rendering.Scene;

namespace Age.Editor;

public class Editor : Node
{
    private readonly Canvas canvas = new();

    public override string NodeName { get; } = nameof(Editor);

    public Editor()
    {
        this.AppendChild(this.canvas);
        this.canvas.AppendChild(new FrameStatus());

        var viewport = new EditorViewport3D();
        var scene    = new DemoScene();

        scene.Camera!.RenderTarget = viewport.RenderTarget;

        this.canvas.AppendChild(viewport);
        this.canvas.AppendChild(scene);
    }
}
