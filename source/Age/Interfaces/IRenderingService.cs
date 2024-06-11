namespace Age.Interfaces;

internal interface IRenderingService : IDisposable
{
    void Render(IEnumerable<Window> windows);
    void RequestDraw();
}
