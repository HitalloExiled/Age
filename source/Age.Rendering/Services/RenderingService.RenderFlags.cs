namespace Age.Rendering.Services;

internal partial class RenderingService
{
    [Flags]
    private enum RenderFlags
    {
        None,
        Wireframe,
    }
}
