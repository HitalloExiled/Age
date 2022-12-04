using Age.Core.IO;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Age.Core.Math;

namespace Age.Drivers.GLES3;

internal class TextureStorage : Servers.Rendering.Storage.TextureStorage
{
    public static uint SystemFbo { get; private set; }
    public static TextureStorage Singleton { get; } = new();

    private readonly Dictionary<Guid, Texture>      textures = new();
    private readonly Dictionary<Guid, RenderTarget> renderTargets  = new();

    public override Color DefaultClearColor { get; set; }

    private static void WARN_PRINT(string message) => throw new NotImplementedException();

    private void ClearRenderTarget(RenderTarget renderTarget)
    {
        var gl = Wrapper.GL;

        if (renderTarget.DirectToScreen)
        {
            return;
        }

        if (renderTarget.Fbo > 0)
        {
            gl.DeleteFramebuffers(1, renderTarget.Fbo);
            renderTarget.Fbo = 0;
        }

        if (renderTarget.Overridden.Color == default)
        {
            gl.DeleteTextures(1, renderTarget.Color);
        }

        if (renderTarget.Overridden.Depth == default)
        {
            gl.DeleteTextures(1, renderTarget.Depth);
        }

        if (renderTarget.Texture != default)
        {
            var texture = this.GetTexture(renderTarget.Texture)!;

            texture.AllocHeight = 0;
            texture.AllocWidth  = 0;
            texture.Width       = 0;
            texture.Height      = 0;
            texture.Active      = false;
        }

        if (renderTarget.Overridden.Color != default)
        {
            var texture = this.GetTexture(renderTarget.Overridden.Color)!;

            texture.IsRenderTarget = false;
        }

        if (renderTarget.BackbufferFbo != 0)
        {
            gl.DeleteFramebuffers(1, renderTarget.BackbufferFbo);
            gl.DeleteTextures(1, renderTarget.Backbuffer);

            renderTarget.BackbufferFbo = 0;
            renderTarget.Backbuffer    = 0;
        }

        RenderTargetClearSdf(renderTarget);
    }

    private static void ClearRenderTargetOverriddenFboCache(RenderTarget renderTarget)
    {
        var gl = Wrapper.GL;
        foreach (var entry in renderTarget.Overridden.FboCache)
        {
            // OpenGL.Singleton.DeleteTextures((uint)entry.Value.AllocatedTextures.Count, entry.Value.AllocatedTextures.ToArray());
            gl.DeleteFramebuffers(1, entry.Value.Fbo);
        }
    }

    private Texture? GetTexture(Guid id)
    {
        this.textures.TryGetValue(id, out var texture);

        if (texture?.IsProxy ?? false)
        {
            return this.textures[texture.ProxyTo];
        }

        return null;
    }

    private static void RenderTargetClearSdf(RenderTarget renderTarget)
    {
        var gl = Wrapper.GL;
        if (renderTarget.SdfTextureWriteFb != 0)
        {
            gl.DeleteTextures(1, renderTarget.SdfTextureRead);
            gl.DeleteTextures(1, renderTarget.SdfTextureWrite);
            gl.DeleteTextures(2, renderTarget.SdfTextureProcess);
            gl.DeleteFramebuffers(1, renderTarget.SdfTextureWriteFb);

            renderTarget.SdfTextureRead       = 0;
            renderTarget.SdfTextureWrite      = 0;
            renderTarget.SdfTextureProcess[0] = 0;
            renderTarget.SdfTextureProcess[1] = 0;
            renderTarget.SdfTextureWriteFb    = 0;
        }
    }

    private void UpdateRenderTarget(RenderTarget renderTarget)
    {
        var gl = Wrapper.GL;

        if (renderTarget.Size.X <= 0 || renderTarget.Size.Y <= 0)
        {
            return;
        }

        if (renderTarget.DirectToScreen)
        {
            renderTarget.Fbo = SystemFbo;
            return;
        }

        renderTarget.ColorInternalFormat = renderTarget.IsTransparent
            ? GLEnum.UnsignedByte
            : GLEnum.UnsignedInt2101010Rev;

        renderTarget.ImageFormat = Image.Format.FORMAT_RGBA8;

        gl.Disable(GLEnum.ScissorTest);
        gl.ColorMask(true, true, true, true);
        gl.DepthMask(false);

        var useMultiView = false; // renderTarget.ViewCount > 1 && config.multiview_supported;
        var textureTarget = useMultiView ? GLEnum.Texture2DArray : GLEnum.Texture2D;

        gl.GenFramebuffers(1, new [] { renderTarget.Fbo });
        gl.BindFramebuffer(GLEnum.Framebuffer, renderTarget.Fbo);

        Texture texture;

        if (renderTarget.Overridden.Color != default)
        {
            texture = this.GetTexture(renderTarget.Overridden.Color)!;

            renderTarget.Color = texture.TextId;
            renderTarget.Size  = new Vector2D<int>(texture.Width, texture.Height);
        }
        else
        {
            texture = this.GetTexture(renderTarget.Texture)!;

            gl.GenTextures(1, new[] { renderTarget.Color });
            gl.BindTexture(textureTarget, renderTarget.Color);

            unsafe
            {
                if (useMultiView)
                {
                    gl.TexImage3D(
                        textureTarget,
                        0,
                        (int)renderTarget.ColorInternalFormat,
                        (uint)renderTarget.Size.X,
                        (uint)renderTarget.Size.Y,
                        renderTarget.ViewCount,
                        0,
                        renderTarget.ColorFormat,
                        renderTarget.ColorType,
                        null
                    );
                }
                else
                {
                    gl.TexImage2D(
                        textureTarget,
                        0,
                        (int)renderTarget.ColorInternalFormat,
                        (uint)renderTarget.Size.X,
                        (uint)renderTarget.Size.Y,
                        0,
                        renderTarget.ColorFormat,
                        renderTarget.ColorType,
                        null
                    );
                }
            }

            gl.TexParameterI(textureTarget, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
            gl.TexParameterI(textureTarget, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameterI(textureTarget, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
            gl.TexParameterI(textureTarget, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);
        }

        if (useMultiView)
        {
            Wrapper.OVR.FramebufferTextureMultiview(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                renderTarget.Color,
                0,
                0,
                renderTarget.ViewCount
            );
        }
        else
        {
            gl.FramebufferTexture2D(
                GLEnum.Framebuffer,
                GLEnum.ColorAttachment0,
                GLEnum.Texture2D,
                renderTarget.Color,
                0
            );
        }

        if (renderTarget.Overridden.Depth != default)
        {
            texture = this.GetTexture(renderTarget.Overridden.Depth)!;

            renderTarget.Depth = texture.TextId;
        }
        else
        {
            gl.GenTextures(1, new[] { renderTarget.Depth });
            gl.BindTexture(textureTarget, renderTarget.Depth);

            unsafe
            {
                if (useMultiView)
                {
                    gl.TexImage3D(
                        textureTarget,
                        0,
                        (int)GLEnum.DepthComponent24,
                        (uint)renderTarget.Size.X,
                        (uint)renderTarget.Size.Y,
                        renderTarget.ViewCount,
                        0,
                        GLEnum.DepthComponent,
                        GLEnum.UnsignedInt,
                        null
                    );
                }
                else
                {
                    gl.TexImage2D(
                        textureTarget,
                        0,
                        (int)GLEnum.DepthComponent24,
                        (uint)renderTarget.Size.X,
                        (uint)renderTarget.Size.Y,
                        0,
                        GLEnum.DepthComponent,
                        GLEnum.UnsignedInt,
                        null
                    );
                }
            }

            gl.TexParameterI(textureTarget, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
            gl.TexParameterI(textureTarget, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameterI(textureTarget, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
            gl.TexParameterI(textureTarget, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);
        }

        if (useMultiView)
        {
            Wrapper.OVR.FramebufferTextureMultiview(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                renderTarget.Depth,
                0,
                0,
                renderTarget.ViewCount
            );
        }
        else
        {
            gl.FramebufferTexture2D(
                GLEnum.Framebuffer,
                GLEnum.ColorAttachment0,
                GLEnum.Texture2D,
                renderTarget.Depth,
                0
            );
        }

        var status = gl.CheckFramebufferStatus(GLEnum.Framebuffer);

        if (status != GLEnum.FramebufferComplete)
        {
            gl.DeleteBuffers(1, renderTarget.Fbo);
            gl.DeleteTextures(1, renderTarget.Color);

            renderTarget.Fbo   = 0;
            renderTarget.Size  = new Vector2D<int>();
            renderTarget.Color = 0;
            renderTarget.Depth = 0;

            if (renderTarget.Overridden.Color != default)
            {
                texture.TextId = 0;
                texture.Active = false;
            }

            WARN_PRINT($"Could not create render target, status: {status}");
        }

        if (renderTarget.Overridden.Color != default)
        {
            texture.IsRenderTarget = true;
        }
        else
        {
            texture.Format     = renderTarget.ImageFormat;
            texture.RealFormat = renderTarget.ImageFormat;
            texture.Target     = textureTarget;

            if (renderTarget.ViewCount > 1 /*  && config->multiview_supported */)
            {
                texture.Type   = Texture.TypeEnum.LAYERED;
                texture.Layers = renderTarget.ViewCount;
            }
            else
            {
                texture.Type   = Texture.TypeEnum.T2D;
                texture.Layers = 1;
            }

            texture.FormatCache         = renderTarget.ColorFormat;
            texture.TypeCache           = GLEnum.UnsignedByte;
            texture.InternalFormatCache = renderTarget.ColorInternalFormat;
            texture.TextId              = renderTarget.Color;
            texture.Width               = renderTarget.Size.X;
            texture.Height              = renderTarget.Size.Y;
            texture.AllocHeight         = renderTarget.Size.Y;
            texture.Active              = true;
        }

        gl.ClearColor(0, 0, 0, 0);
        gl.Clear((uint)GLEnum.ColorBufferBit);
        gl.BindFramebuffer(GLEnum.Framebuffer, SystemFbo);
    }

    public RenderTarget GetRenderTarget(Guid guid) =>
        this.renderTargets[guid];

    public void RenderTargetClearUsed(Guid renderTargetId)
    {
        var renderTarget = this.renderTargets[renderTargetId];

        renderTarget.UsedInFrame = false;
    }

    public override void RenderTargetRequestClear(Guid renderTargetId, Color color)
    {
        var renderTarget = this.renderTargets[renderTargetId];

        renderTarget.ClearRequest = true;
        renderTarget.ClearColor   = color;
    }

    public override void RenderTargetSetAsUnused(Guid renderTargetId) => this.RenderTargetClearUsed(renderTargetId);

    public override void RenderTargetSetOverride(Guid renderTargetId, Guid colorTexture, Guid depthTexture, Guid velocityTexture)
    {
        var renderTarget = this.renderTargets[renderTargetId];

        renderTarget.Overridden.Velocity = velocityTexture;

        if (renderTarget.Overridden.Color == colorTexture && renderTarget.Overridden.Depth == depthTexture)
        {
            return;
        }

        if (colorTexture == default && depthTexture == default)
        {
            this.ClearRenderTarget(renderTarget);

            renderTarget.Overridden.IsOverridden = false;

            renderTarget.Color = default;
            renderTarget.Depth = default;
            renderTarget.Size = default;

            ClearRenderTargetOverriddenFboCache(renderTarget);

            return;
        }

        if (!renderTarget.Overridden.IsOverridden)
        {
            this.ClearRenderTarget(renderTarget);
        }

        renderTarget.Overridden.Color = colorTexture;
        renderTarget.Overridden.Depth = depthTexture;
        renderTarget.Overridden.IsOverridden = true;

        var hash = (uint)colorTexture.GetHashCode() ^ (uint)depthTexture.GetHashCode();

        if (renderTarget.Overridden.FboCache.TryGetValue(hash, out var cache))
        {
            renderTarget.Fbo     = cache.Fbo;
            renderTarget.Size    = cache.Size;
            renderTarget.Texture = colorTexture;

            return;
        }

        this.UpdateRenderTarget(renderTarget);
    }

    public override void UpdateTextureAtlas() =>  // TODO - drivers\gles3\storage\texture_storage.cpp[1275]
        throw new NotImplementedException();
}
