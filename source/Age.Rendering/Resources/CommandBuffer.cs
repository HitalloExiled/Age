using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Extensions;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;
using Age.Rendering.Vulkan;

namespace Age.Rendering.Resources;

public sealed class CommandBuffer : Resource<VkCommandBuffer>
{
    private readonly bool owner;

    internal override VkCommandBuffer Instance { get; }

    internal CommandBuffer(VkCommandBuffer instance)
    {
        this.Instance = instance;
        this.owner    = false;
    }

    internal CommandBuffer(VkCommandBufferLevel commandBufferLevel)
    {
        this.Instance = VulkanRenderer.Singleton.Context.AllocateCommand(commandBufferLevel);
        this.owner    = true;
    }

    protected override void OnDisposed()
    {
        if (this.owner)
        {
            VulkanRenderer.Singleton.DeferredDispose(this.Instance);
        }
    }

    public static CommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = new CommandBuffer(VkCommandBufferLevel.Primary);

        commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);

        return commandBuffer;
    }

    public static unsafe void EndSingleTimeCommands(CommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var commandBufferHandle = commandBuffer.Instance.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        VulkanRenderer.Singleton.Context.GraphicsQueue.Submit(submitInfo);
        VulkanRenderer.Singleton.Context.GraphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    public void Begin(VkCommandBufferUsageFlags oneTimeSubmit) =>
        this.Instance.Begin(oneTimeSubmit);

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, in Color clearColor)
    {
        var clearValue = new VkClearValue();

        clearValue.Color.Float32[0] = clearColor.R;
        clearValue.Color.Float32[1] = clearColor.G;
        clearValue.Color.Float32[2] = clearColor.B;
        clearValue.Color.Float32[3] = clearColor.A;
        clearValue.DepthStencil = new() { Depth = 1, Stencil = 0 };

        this.BeginRenderPass(framebuffer.Extent, renderPass, framebuffer, [clearValue]);
    }

    public unsafe void BeginRenderPass(RenderPass renderPass, Framebuffer framebuffer, ReadOnlySpan<Color> clearColors)
    {
        Span<VkClearValue> clearValues = stackalloc VkClearValue[clearColors.Length];

        for (var i = 0; i < clearColors.Length; i++)
        {
            clearValues[i].Color.Float32[0] = clearColors[i].R;
            clearValues[i].Color.Float32[1] = clearColors[i].G;
            clearValues[i].Color.Float32[2] = clearColors[i].B;
            clearValues[i].Color.Float32[3] = clearColors[i].A;
        }

        this.BeginRenderPass(framebuffer.Extent, renderPass, framebuffer, clearValues);
    }

    public void BeginRenderPass(RenderTarget renderTarget, in ClearValue clearColor) =>
        this.BeginRenderPass(renderTarget, [clearColor]);

    public void BeginRenderPass(RenderTarget renderTarget, ReadOnlySpan<ClearValue> clearValues)
    {
        var vkClearValues = clearValues.Cast<ClearValue, VkClearValue>();

        this.BeginRenderPass(renderTarget.Size.ToExtent2D(), renderTarget.RenderPass, renderTarget.Framebuffer, vkClearValues);
    }

    public unsafe void BeginRenderPass(VkExtent2D extent, VkRenderPass renderPass, VkFramebuffer framebuffer, ReadOnlySpan<VkClearValue> clearValues)
    {
        fixed (VkClearValue* pClearValues = clearValues)
        {
            var renderPassBeginInfo = new VkRenderPassBeginInfo
            {
                ClearValueCount = (uint)clearValues.Length,
                Framebuffer     = framebuffer.Handle,
                PClearValues    = pClearValues,
                RenderArea      =
                {
                    Extent = extent,
                },
                RenderPass = renderPass.Handle,
            };

            this.Instance.BeginRenderPass(renderPassBeginInfo, VkSubpassContents.Inline);
        }
    }

    public void BindIndexBuffer(IndexBuffer indexBuffer) =>
        this.Instance.BindIndexBuffer(indexBuffer.Buffer.Instance, 0, indexBuffer.Type);

    public void BindShader(Shader shader) =>
        this.Instance.BindPipeline(shader.BindPoint, shader.Pipeline!);

    public void BindVertexBuffer(VertexBuffer vertexBuffer) =>
        this.Instance.BindVertexBuffers(0, 1, [vertexBuffer.Buffer.Instance], [0]);

    public void BindVertexBuffer(ReadOnlySpan<VertexBuffer> vertexBuffers)
    {
        var handles = new VkBuffer[vertexBuffers.Length];

        for (var i = 0; i < vertexBuffers.Length; ++i)
         {
             handles[i] = vertexBuffers[i].Buffer.Instance;
         }

        this.Instance.BindVertexBuffers(0, 1, handles, new ulong[vertexBuffers.Length]);
    }

    public void BindUniformSet(UniformSet uniformSet) =>
        this.Instance.BindDescriptorSets(uniformSet.Shader.BindPoint, uniformSet.Shader.PipelineLayout!, 0, uniformSet.DescriptorSets, []);

    public void ClearAttachments(ReadOnlySpan<VkClearAttachment> attachments, ReadOnlySpan<VkClearRect> rects) =>
        this.Instance.ClearAttachments(attachments, rects);

    public void ClearImageColor(Image image, VkImageLayout imageLayout, VkClearColorValue color, ReadOnlySpan<VkImageSubresourceRange> ranges) =>
        this.Instance.ClearColorImage(image, imageLayout, color, ranges);

    public void Draw(VertexBuffer vertexBuffer, uint instanceCount = 1, uint firstVertex = 0, uint firstInstance = 0) =>
        this.Instance.Draw(vertexBuffer.Size, instanceCount, firstVertex, firstInstance);

    public void DrawIndexed(IndexBuffer indexBuffer, uint instanceCount = 1, uint firstIndex = 0, int vertexOffset = 0, uint firstInstance = 0) =>
        this.Instance.DrawIndexed(indexBuffer.Size, instanceCount, firstIndex, vertexOffset, firstInstance);

    public void End() =>
        this.Instance.End();

    public void EndRenderPass() =>
        this.Instance.EndRenderPass();

    public void NextSubPass() =>
        this.Instance.NextSubPass(VkSubpassContents.Inline);

    public void Reset() =>
        this.Instance.Reset();

    public void PushConstant<T>(Shader shader, in T constant) where T : unmanaged =>
        this.Instance.PushConstants(shader.PipelineLayout!, shader.PushConstantStages, constant);

    public void SetViewport(in Size<uint> size)
    {
        var viewport = new VkViewport
        {
            X        = 0,
            Y        = 0,
            Width    = size.Width,
            Height   = size.Height,
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
            Extent = size.ToExtent2D(),
        };

        this.Instance.SetScissor(0, scissor);
    }

    public void SetStencilReference(VkStencilFaceFlags faceMask, uint reference) =>
        this.Instance.SetStencilReference(faceMask, reference);
}
