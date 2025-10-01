using System.Diagnostics;
using Age.Rendering.Resources;
using Age.Scenes;

namespace Age.Elements;

public sealed class EmbeddedViewport : Element
{
    public override string NodeName => nameof(EmbeddedViewport);

    public SubViewport? Viewport
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            if (field != null)
            {
                field.Resized -= this.OnViewportResized;
            }

            field = value;

            if (value != null)
            {
                value.Resized += this.OnViewportResized;

                this.OnViewportResized();
            }
            else
            {
                this.ClearBackground();
            }
        }
    }

    public EmbeddedViewport()
    {
        this.Seal();
        this.ClearBackground();
    }

    private void ClearBackground() =>
        this.Style.BackgroundImage = new(Texture2D.Default);

    private void OnViewportResized()
    {
        Debug.Assert(this.Viewport != null);

        this.Style.Size            = this.Viewport.Size;
        this.Style.BackgroundImage = new(this.Viewport.Texture);
    }
}
