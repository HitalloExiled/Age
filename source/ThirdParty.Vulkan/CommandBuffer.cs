using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;
using ThirdParty.Vulkan.TypeExtensions;

namespace ThirdParty.Vulkan;

public unsafe partial class CommandBuffer : DisposableNativeHandle
{
    private readonly Device      device;
    private readonly CommandPool commandPool;

    internal CommandBuffer(VkCommandBuffer handle, Device device, CommandPool commandPool) : base(handle)
    {
        this.device      = device;
        this.commandPool = commandPool;
    }

    protected override void OnDispose()
    {
        fixed (nint* pHandle = &this.Handle)
        {
            PInvoke.vkFreeCommandBuffers(this.device, this.commandPool, 1, pHandle);
        }
    }

    /// <inheritdoc cref="PInvoke.vkBeginCommandBuffer" />
    public void Begin(CommandBufferUsageFlags flags = default)
    {
        var commandBufferBeginInfo = new VkCommandBufferBeginInfo
        {
            flags = flags,
        };

        VulkanException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, &commandBufferBeginInfo));
    }

    /// <inheritdoc cref="PInvoke.vkBeginCommandBuffer" />
    public void Begin(BeginInfo beginInfo) =>
        VulkanException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, beginInfo));

    /// <inheritdoc cref="PInvoke.vkCmdBeginRenderPass" />
    public void BeginRenderPass(RenderPass.BeginInfo beginInfo, SubpassContents contents) =>
        PInvoke.vkCmdBeginRenderPass(this, beginInfo, contents);

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(PipelineBindPoint pipelineBindPoint, PipelineLayout layout, uint firstSet, DescriptorSet[] descriptorSets, uint[] dynamicOffsets)
    {
        fixed (VkDescriptorSet* pDescriptorSets = descriptorSets.ToHandlers())
        fixed (uint*            pDynamicOffsets = dynamicOffsets)
        {
            PInvoke.vkCmdBindDescriptorSets(this, pipelineBindPoint, layout, firstSet, (uint)descriptorSets.Length, pDescriptorSets, (uint)dynamicOffsets.Length, pDynamicOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindIndexBuffer" />
    public void BindIndexBuffer(Buffer indexBuffer, ulong offset, IndexType indexType) =>
        PInvoke.vkCmdBindIndexBuffer(this, indexBuffer, offset, indexType);

    /// <inheritdoc cref="PInvoke.vkCmdBindPipeline" />
    public void BindPipeline(PipelineBindPoint pipelineBindPoint, Pipeline pipeline) =>
        PInvoke.vkCmdBindPipeline(this, pipelineBindPoint, pipeline);

    /// <inheritdoc cref="PInvoke.vkCmdBindVertexBuffers" />
    public void BindVertexBuffers(uint firstBinding, uint bindingCount, Buffer[] vertexBuffers, ulong[] offsets)
    {
        fixed (VkBuffer* pVertexBuffers = vertexBuffers.ToHandlers())
        fixed (ulong*    pOffsets       = offsets)
        {
            PInvoke.vkCmdBindVertexBuffers(this, firstBinding, bindingCount, pVertexBuffers, pOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBlitImage" />
    public void BlitImage(Image srcImage, ImageLayout srcImageLayout, Image dstImage, ImageLayout dstImageLayout, ImageBlit[] regions, Filter filter)
    {
        fixed (VkImageBlit* pRegions = regions.ToNatives())
        {
            PInvoke.vkCmdBlitImage(this, srcImage, srcImageLayout, dstImage, dstImageLayout, (uint)regions.Length, pRegions, filter);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBuffer" />
    public void CopyBuffer(Buffer source, Buffer destination, params BufferCopy[] regions)
    {
        fixed (VkBufferCopy* pRegions = regions.ToNatives())
        {
            PInvoke.vkCmdCopyBuffer(this.Handle, source, destination, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBufferToImage" />
    public void CopyBufferToImage(Buffer sourceBuffer, Image destinationImage, ImageLayout transferDstOptimal, params BufferImageCopy[] regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions.ToNatives())
        {
            PInvoke.vkCmdCopyBufferToImage(this.Handle, sourceBuffer, destinationImage, transferDstOptimal, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkEndCommandBuffer" />
    public void End() =>
        VulkanException.Check(PInvoke.vkEndCommandBuffer(this.Handle));

    /// <inheritdoc cref="PInvoke.vkCmdDrawIndexed" />
    public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance) =>
        PInvoke.vkCmdDrawIndexed(this, indexCount, instanceCount, firstIndex, vertexOffset, firstInstance);

    /// <inheritdoc cref="PInvoke.vkCmdEndRenderPass" />
    public void EndRenderPass() =>
        PInvoke.vkCmdEndRenderPass(this);

    /// <inheritdoc cref="PInvoke.vkCmdPipelineBarrier" />
    public void PipelineBarrier(
        PipelineStageFlags    srcStageMask,
        PipelineStageFlags    dstStageMask,
        DependencyFlags       dependencyFlags,
        MemoryBarrier[]       memoryBarriers,
        BufferMemoryBarrier[] bufferMemoryBarriers,
        ImageMemoryBarrier[]  imageMemoryBarriers
    )
    {
        fixed (VkMemoryBarrier*       pMemoryBarriers       = memoryBarriers.ToNatives())
        fixed (VkBufferMemoryBarrier* pBufferMemoryBarriers = bufferMemoryBarriers.ToNatives())
        fixed (VkImageMemoryBarrier*  pImageMemoryBarriers  = imageMemoryBarriers.ToNatives())
        {
            PInvoke.vkCmdPipelineBarrier(
                this,
                srcStageMask,
                dstStageMask,
                dependencyFlags,
                (uint)bufferMemoryBarriers.Length,
                pMemoryBarriers,
                (uint)bufferMemoryBarriers.Length,
                pBufferMemoryBarriers,
                (uint)imageMemoryBarriers.Length,
                pImageMemoryBarriers
            );
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdSetScissor" />
    public void SetScissor(uint firstScissor, params Rect2D[] scissors)
    {
        fixed (VkRect2D* pScissors = scissors.ToNatives())
        {
            PInvoke.vkCmdSetScissor(this, firstScissor, (uint)scissors.Length, pScissors);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdSetViewport" />
    public void SetViewport(uint firstViewport, params Viewport[] viewports)
    {
        fixed (VkViewport* pViewports = viewports.ToNatives())
        {
            PInvoke.vkCmdSetViewport(this, firstViewport, (uint)viewports.Length, pViewports);
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetCommandBuffer" />
    public void Reset(CommandBufferResetFlags flags = default) =>
        PInvoke.vkResetCommandBuffer(this, flags);
}
