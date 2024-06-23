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

        var editorViewport3D = new EditorViewport3D();
        var scene            = new DemoScene();

        scene.Camera!.RenderTarget = editorViewport3D.RenderTarget;

        this.canvas.AppendChild(editorViewport3D);
        this.canvas.AppendChild(scene);
    }
}
