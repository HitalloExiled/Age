using Age.Styling;
using Age.Elements;
using Age.Numerics;
using Age.Scene;

namespace Age.Playground;

public class EditorViewport3D : Element
{
    public override string NodeName => nameof(EditorViewport3D);

    public Viewport Viewport { get; }

    public EditorViewport3D()
    {
        this.Style = new()
        {
            Border  = new Border(1, 0, Color.Red),
            MinSize = new(Unit.Px(100)),
        };

        this.AppendChild(this.Viewport = new Viewport(new(400)));
    }
}
