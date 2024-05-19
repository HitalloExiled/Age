using Age.Rendering.Resources;

namespace Age.Rendering.Interfaces;

internal interface IRenderingService : IDisposable
{
    Sampler CreateSampler();
    uint[] GetObjectIdBuffer(IWindow window);
    void DestroySampler(Sampler sampler);
    void Render(IWindow window);
    void Render(IEnumerable<IWindow> windows);
    void RequestDraw();
    void UpdateTexture(Texture texture, byte[] data);
}
