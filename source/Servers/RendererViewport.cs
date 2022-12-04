using Age.Core.Math;
using Age.Servers.Rendering;

using RSG = Age.Servers.RenderingServerGlobals;
using RS = Age.Servers.RenderingServer;
using RealT = System.Single;

namespace Age.Servers;

#pragma warning disable CA1822,IDE0059 // TODO - Remove

internal class RendererViewport
{
    private static void WARN_PRINT_ONCE(string message) => throw new NotImplementedException();

    private readonly List<Viewport> viewports = new();
    private readonly int occlusionRaysPerThread = 512;

    public static RendererViewport Singleton { get; } = new();

    private void Configure3dRenderBuffers(Viewport viewport)
    {
        if (viewport.RenderBuffers != null)
        {
            if (viewport.Size.X == 0 || viewport.Size.Y == 0)
            {
                viewport.RenderBuffers = null;
            }
            else
            {
                var scaling3dScale = viewport.Scaling3dScale;
                var scaling3dMode  = viewport.Scaling3dMode;
                var scalingEnabled = true;

                if (scaling3dMode == RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR && scaling3dScale > 1.0)
                {
                    scaling3dMode = RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR;
                }

                if ((scaling3dMode == RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR) && !viewport.FsrEnabled)
                {
                    WARN_PRINT_ONCE("FSR 1.0 3D resolution scaling is not available. Falling back to bilinear 3D resolution scaling.");
                    scaling3dMode = RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR;
                }

                if (scaling3dScale == 1.0)
                {
                    scalingEnabled = false;
                }

                int width;
                int height;
                int renderWidth;
                int renderHeight;

                if (scalingEnabled)
                {
                    switch (scaling3dMode)
                    {
                        case RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR:
                            width        = (int)Math.Clamp(viewport.Size.X * scaling3dScale, 1, 16384);
                            height       = (int)Math.Clamp(viewport.Size.Y * scaling3dScale, 1, 16384);
                            renderWidth  = width;
                            renderHeight = height;
                            break;
                        case RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR:
                            width        = viewport.Size.X;
                            height       = viewport.Size.Y;
                            renderWidth  = (int)Math.Max(width * scaling3dScale, 1.0);
                            renderHeight = (int)Math.Max(height * scaling3dScale, 1.0);
                            break;
                        default:
                            WARN_PRINT_ONCE($"Unknown scaling mode: {scaling3dMode}. Disabling 3D resolution scaling.");

                            width        = viewport.Size.X;
                            height       = viewport.Size.Y;
                            renderWidth  = width;
                            renderHeight = height;
                            break;
                    }
                }
                else
                {
                    width        = viewport.Size.X;
                    height       = viewport.Size.Y;
                    renderWidth  = width;
                    renderHeight = height;
                }

                viewport.InternalSize = new Vector2<int>(renderWidth, renderHeight);

                var textureMipmapBias = Math.Log2(Math.Min(scaling3dScale, 1.0)) + viewport.TextureMipmapBias;

                viewport.RenderBuffers.Configure(
                    viewport.RenderTarget,
                    viewport.InternalSize,
                    new Vector2<int>(width, height),
                    viewport.FsrSharpness,
                    textureMipmapBias,
                    viewport.MSAA3d,
                    viewport.ScreenSpaceAA,
                    viewport.UseTaa,
                    viewport.UseDebanding,
                    viewport.ViewCount
                );
            }
        }
    }

    private void DrawViewport(Viewport viewport)
    {
        var scenarioDrawCanvasBg = false;
        var scenarioCanvasMaxLayers = 0;

        for (var i = 0; i < RS.VIEWPORT_RENDER_INFO_TYPE_MAX; i++)
        {
            for (var j = 0; j < RS.VIEWPORT_RENDER_INFO_MAX; j++)
            {
                viewport.RenderInfo.Info[i, j] = 0;
            }
        }

        if (!viewport.Disable2D && viewport.DisableEnvironment && RSG.Scene.IsScenario(viewport.Scenario))
        {
            var environment = RSG.Scene.ScenarioGetEnvironment(viewport.Scenario);

            if (RSG.Scene.IsEnvironment(environment))
            {
                scenarioDrawCanvasBg    = RSG.Scene.EnvironmentGetBackground(environment) == RS.EnvironmentBG.ENV_BG_CANVAS;
                scenarioCanvasMaxLayers = RSG.Scene.EnvironmentGetCanvasMaxLayer(environment);
            }
        }

        var canDraw3D = RSG.Scene.IsCamera(viewport.Camera) && !viewport.Disable3D;

        if ((scenarioDrawCanvasBg || canDraw3D) && viewport.RenderBuffers == null)
        {
            viewport.RenderBuffers = RSG.Scene.RenderBuffersCreate();

            this.Configure3dRenderBuffers(viewport);
        }

        var color = viewport.TransparentBg ? default : RSG.TextureStorage.DefaultClearColor;

        if (viewport.ClearMode != RS.ViewportClearMode.VIEWPORT_CLEAR_NEVER)
        {
            RSG.TextureStorage.RenderTargetRequestClear(viewport.RenderTarget, color);

            if (viewport.ClearMode == RS.ViewportClearMode.VIEWPORT_CLEAR_ONLY_NEXT_FRAME)
            {
                viewport.ClearMode = RS.ViewportClearMode.VIEWPORT_CLEAR_NEVER;
            }
        }

        if (!scenarioDrawCanvasBg && canDraw3D)
        {
            this.Draw3D(viewport);
        }
    }

    private void Draw3D(Viewport viewport)
    {
        // TODO - godot\servers\rendering\renderer_viewport.cpp[190:193]

        if (viewport.UseOcclusionCulling && viewport.OcclusionBufferDirty)
        {
            var aspect  = viewport.Size.X / viewport.Size.Y;
            var maxSize = this.occlusionRaysPerThread * ThreadPool.ThreadCount;

            var viewportSize = viewport.Size.X * viewport.Size.Y;
            maxSize = Math.Clamp(maxSize, viewportSize / (32 * 32), viewportSize / (2 * 2));

            var height = Math.Sqrt(maxSize / aspect);
            var newSize = new Vector2<int>((int)height * aspect, (int)height);

            RendererSceneOcclusionCull.Singleton.BufferSetSize(viewport.Id, newSize);

            viewport.OcclusionBufferDirty = false;
        }

        var screenMeshLodThreshold = viewport.MeshLodThreshold / viewport.Size.X;
        RSG.Scene.RenderCamera(
            viewport.RenderBuffers!,
            viewport.Camera,
            viewport.Scenario,
            viewport.Id,
            viewport.InternalSize.As<RealT>(),
            viewport.UseTaa,
            screenMeshLodThreshold,
            viewport.ShadowAtlas,
            null, //TODO - xr_interface,
            viewport.RenderInfo
        );
    }

    public void DrawViewports()
    {
        // var verticesDrawn = 0;
	    // var objectsDrawn  = 0;
	    // var drawCallsUsed = 0;

        foreach (var viewport in this.viewports)
        {
            RSG.TextureStorage.RenderTargetSetAsUnused(viewport.RenderTarget);

            RSG.TextureStorage.RenderTargetSetOverride(viewport.RenderTarget, default, default, default);

            this.DrawViewport(viewport);
        }
    }
}
