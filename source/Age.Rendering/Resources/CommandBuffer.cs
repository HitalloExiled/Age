using Age.Numerics;
using Age.Rendering.Interfaces;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering.Resources;

public class CommandBuffer : Resource<VkCommandBuffer>
{
    private readonly bool disposable;
    private readonly VkCommandBuffer instance;

    public override VkCommandBuffer Instance => this.instance;

    internal CommandBuffer(VkCommandBuffer instance, bool disposable)
    {
        this.instance = instance;
        this.disposable = disposable;
    }

    protected override void Disposed()
    {
        if (this.disposable)
        {
            this.Instance.Dispose();
        }
    }

    public void Begin(VkCommandBufferUsageFlags oneTimeSubmit) =>
        this.Instance.Begin(oneTimeSubmit);

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, Color clearColor)
    {
        var clearValue = new VkClearValue();

        clearValue.Color.Float32[0] = clearColor.R;
        clearValue.Color.Float32[1] = clearColor.G;
        clearValue.Color.Float32[2] = clearColor.B;
        clearValue.Color.Float32[3] = clearColor.A;
        clearValue.DepthStencil = new() { Depth = 1, Stencil = 0 };

        this.BeginRenderPass(renderPass, framebuffer, [clearValue]);
    }

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, Span<Color> clearColors)
    {
        Span<VkClearValue> clearValues = stackalloc VkClearValue[clearColors.Length];

        for (var i = 0; i < clearColors.Length; i++)
        {
            clearValues[i].Color.Float32[0] = clearColors[i].R;
            clearValues[i].Color.Float32[1] = clearColors[i].G;
            clearValues[i].Color.Float32[2] = clearColors[i].B;
            clearValues[i].Color.Float32[3] = clearColors[i].A;
        }

        this.BeginRenderPass(renderPass, framebuffer, clearValues);
    }

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, Span<VkClearValue> clearValues)
    {
        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                ClearValueCount = (uint)clearValues.Length,
                Framebuffer     = framebuffer.Instance.Handle,
                PClearValues    = pClearValues,
                RenderArea      = new()
                {
                    Offset = new()
                    {
                        X = 0,
                        Y = 0
                    },
                    Extent = framebuffer.Extent,
                },
                RenderPass = renderPass.Instance.Handle,
            };

            this.Instance.BeginRenderPass(renderPassBeginInfo, VkSubpassContents.Inline);
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer) =>
        this.Instance.BindIndexBuffer(indexBuffer.Buffer.Instance, 0, indexBuffer.Type);

    public void BindShader(Shader shader) =>
        this.Instance.BindPipeline(shader.BindPoint, shader.Pipeline);

    public void BindVertexBuffer(VertexBuffer vertexBuffer) =>
        this.Instance.BindVertexBuffers(0, 1, [vertexBuffer.Buffer.Instance], [0]);

    public void BindVertexBuffer(Span<VertexBuffer> vertexBuffers)
    {
        var handles = new VkBuffer[vertexBuffers.Length];

        for (var i = 0; i < vertexBuffers.Length; ++i)
         {
             handles[i] = vertexBuffers[i].Buffer.Instance;
         }

        this.Instance.BindVertexBuffers(0, 1, handles, new ulong[vertexBuffers.Length]);
    }

    public void BindUniformSet(UniformSet uniformSet) =>
        this.Instance.BindDescriptorSets(uniformSet.shader.BindPoint, uniformSet.shader.PipelineLayout, 0, uniformSet.DescriptorSets, []);

    public void ClearImageColor(Image image, VkImageLayout imageLayout, VkClearColorValue color, Span<VkImageSubresourceRange> ranges) =>
        this.Instance.ClearColorImage(image, imageLayout, color, ranges);

    public void Draw(VertexBuffer vertexBuffer, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) =>
        this.Instance.Draw(vertexBuffer.Size, instanceCount, firstVertex, firstInstance);

    public void DrawIndexed(IndexBuffer indexBuffer, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0) =>
        this.Instance.DrawIndexed(indexBuffer.Size, instanceCount, firstIndex, vertexOffset, firstInstance);

    public void End() =>
        this.Instance.End();

    public void EndRenderPass() =>
        this.Instance.EndRenderPass();

    public void Reset() =>
        this.Instance.Reset();

    public void PushConstant<T>(Shader shader, in T constant) where T : unmanaged, IPushConstant =>
        this.Instance.PushConstants(shader.PipelineLayout, T.Stages, constant);

    public void SetViewport(in VkExtent2D extent)
    {
        var viewport = new VkViewport
        {
            X        = 0,
            Y        = 0,
            Width    = extent.Width,
            Height   = extent.Height,
            MinDepth = 0,
            MaxDepth = 1
        };

        this.Instance.SetViewport(0, viewport);

        var scissor = new VkRect2D
        {
            Offset = new()
            {
                X = 0,
                Y = 0
            },
            Extent = extent,
        };

        this.Instance.SetScissor(0, scissor);
    }
}
