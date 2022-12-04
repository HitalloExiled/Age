using Age.Servers.Rendering;

namespace Age.Servers;

internal abstract class RenderingServer
{
    #region enums
    public enum EnvironmentBG
    {
        ENV_BG_CLEAR_COLOR,
        ENV_BG_COLOR,
        ENV_BG_SKY,
        ENV_BG_CANVAS,
        ENV_BG_KEEP,
        ENV_BG_CAMERA_FEED,
        ENV_BG_MAX
    };

    public enum ViewportMSAA
    {
        VIEWPORT_MSAA_DISABLED,
        VIEWPORT_MSAA_2X,
        VIEWPORT_MSAA_4X,
        VIEWPORT_MSAA_8X,
        VIEWPORT_MSAA_MAX,
    };

    public enum ViewportClearMode
    {
        VIEWPORT_CLEAR_ALWAYS,
        VIEWPORT_CLEAR_NEVER,
        VIEWPORT_CLEAR_ONLY_NEXT_FRAME
    };

    public enum ViewportScaling3DMode
    {
        VIEWPORT_SCALING_3D_MODE_BILINEAR,
        VIEWPORT_SCALING_3D_MODE_FSR,
        VIEWPORT_SCALING_3D_MODE_MAX
    };

    public enum ViewportScreenSpaceAA
    {
        VIEWPORT_SCREEN_SPACE_AA_DISABLED,
        VIEWPORT_SCREEN_SPACE_AA_FXAA,
        VIEWPORT_SCREEN_SPACE_AA_MAX,
    };
    #endregion enums

    public const int VIEWPORT_RENDER_INFO_MAX = 3;
    public const int VIEWPORT_RENDER_INFO_TYPE_MAX = 2;

    public static RenderingServer Singleton => RenderingServerDefault.Singleton;

    public abstract bool IsRenderLoopEnabled { get; }
    public abstract bool HasChanged          { get; }

    public bool PrintGpuProfile   { get; set; }
    public bool RenderLoopEnabled { get; set; }

    public abstract void Draw(bool swapBuffers, double frameStep);
    public abstract void Sync();

    public virtual void Init() => throw new NotImplementedException();
}
