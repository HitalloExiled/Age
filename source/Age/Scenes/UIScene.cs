using Age.Elements;

namespace Age.Scenes;

public class UIScene : Scene
{
    public override string NodeName => nameof(UIScene);

    public Canvas Canvas { get; } = new();

    public UIScene()
    {
        this.AppendChild(this.Canvas);

        this.Seal();
        this.SuspendUpdates();
        this.SuspendChildrenUpdates();
    }
}
