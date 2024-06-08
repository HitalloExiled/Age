using Age.Numerics;
using Age.Rendering.Interfaces;
using Age.Rendering.Vulkan;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

public class CommandBuffer : Resource<VkCommandBuffer>
{
    private readonly bool disposable;

    internal CommandBuffer(VulkanRenderer renderer, VkCommandBuffer value, bool disposable) : base(renderer, value) =>
        this.disposable = disposable;

    protected override void OnDispose()
    {
        if (this.disposable)
        {
            this.Value.Dispose();
        }
    }

    public void Begin(VkCommandBufferUsageFlags oneTimeSubmit) =>
        this.Value.Begin(oneTimeSubmit);

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, Color clearColor)
    {
        var clearValues = new VkClearValue[2];

        clearValues[0].Color.Float32[0] = clearColor.R;
        clearValues[0].Color.Float32[1] = clearColor.G;
        clearValues[0].Color.Float32[2] = clearColor.B;
        clearValues[0].Color.Float32[3] = clearColor.A;

        clearValues[1].DepthStencil = new()
        {
            Depth   = 1.0f,
            Stencil = 1
        };

        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                ClearValueCount = (uint)clearValues.Length,
                Framebuffer     = framebuffer.Value.Handle,
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
                RenderPass = renderPass.Value.Handle,
            };

            this.Value.BeginRenderPass(renderPassBeginInfo, VkSubpassContents.Inline);
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer) =>
        this.Value.BindIndexBuffer(indexBuffer.Buffer.Value, 0, indexBuffer.Type);

    public void BindPipeline(Shader shader) =>
        this.Value.BindPipeline(shader.PipelineBindPoint, shader.Pipeline);

    public void BindVertexBuffers(VertexBuffer vertexBuffer) =>
        this.Value.BindVertexBuffers(0, 1, [vertexBuffer.Buffer.Value], [0]);

    public void BindVertexBuffer(Span<VertexBuffer> vertexBuffers)
    {
        var handles = new VkBuffer[vertexBuffers.Length];

        for (var i = 0; i < vertexBuffers.Length; ++i)
         {
             handles[i] = vertexBuffers[i].Buffer.Value;
         }

        this.Value.BindVertexBuffers(0, 1, handles, new ulong[vertexBuffers.Length]);
    }

    public void BindUniformSet(UniformSet uniformSet) =>
        this.Value.BindDescriptorSets(uniformSet.Shader.PipelineBindPoint, uniformSet.Shader.PipelineLayout, 0, uniformSet.DescriptorSets, []);

    public void Draw(VertexBuffer vertexBuffer, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) =>
        this.Value.Draw(vertexBuffer.Size, instanceCount, firstVertex, firstInstance);

    public void DrawIndexed(IndexBuffer indexBuffer, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0) =>
        this.Value.DrawIndexed(indexBuffer.Size, instanceCount, firstIndex, vertexOffset, firstInstance);

    public void End() =>
        this.Value.End();

    public void EndRenderPass() =>
        this.Value.EndRenderPass();

    public void Reset() =>
        this.Value.Reset();

    public void PushConstant<T>(Shader shader, in T constant) where T : unmanaged, IPushConstant =>
        this.Value.PushConstants(shader.PipelineLayout, T.Stages, constant);

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

        this.Value.SetViewport(0, viewport);

        var scissor = new VkRect2D
        {
            Offset = new()
            {
                X = 0,
                Y = 0
            },
            Extent = extent,
        };

        this.Value.SetScissor(0, scissor);
    }
}
