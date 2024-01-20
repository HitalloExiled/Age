namespace Age.Rendering.Services;

public static class Singleton
{
    public static RenderingService RenderingService { get; set; } = null!;
    public static TextService      TextService      { get; set; } = null!;
}
