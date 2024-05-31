namespace Age.Rendering.Interfaces;

internal interface IRenderingService : IDisposable
{
    void Render(IEnumerable<IWindow> windows);
    void RequestDraw();
}
