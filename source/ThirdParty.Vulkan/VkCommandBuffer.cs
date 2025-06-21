using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

namespace ThirdParty.Vulkan;

public sealed unsafe class VkCommandBuffer : DisposableManagedHandle<VkCommandBuffer>
{
    private readonly VkDevice      device;
    private readonly VkCommandPool commandPool;

    internal VkCommandBuffer(VkHandle<VkCommandBuffer> handle, VkDevice device, VkCommandPool commandPool) : base(handle)
    {
        this.device      = device;
        this.commandPool = commandPool;
    }

    protected override void Disposed()
    {
        fixed (VkHandle<VkCommandBuffer>* pHandle = &this.handle)
        {
            PInvoke.vkFreeCommandBuffers(this.device.Handle, this.commandPool.Handle, 1, pHandle);
        }
    }

    /// <inheritdoc cref="PInvoke.vkBeginCommandBuffer" />
    public void Begin(VkCommandBufferUsageFlags flags = default)
    {
        var commandBufferBeginInfo = new VkCommandBufferBeginInfo
        {
            Flags = flags,
        };

        VkException.Check(PInvoke.vkBeginCommandBuffer(this.handle, &commandBufferBeginInfo));
    }

    /// <inheritdoc cref="PInvoke.vkBeginCommandBuffer" />
    public void Begin(in VkCommandBufferBeginInfo beginInfo)
    {
        fixed (VkCommandBufferBeginInfo* pBeginInfo = &beginInfo)
        {
            VkException.Check(PInvoke.vkBeginCommandBuffer(this.handle, pBeginInfo));
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBeginRenderPass" />
    public void BeginRenderPass(in VkRenderPassBeginInfo beginInfo, VkSubpassContents contents)
    {
        fixed (VkRenderPassBeginInfo* pBeginInfo = &beginInfo)
        {
            PInvoke.vkCmdBeginRenderPass(this.handle, pBeginInfo, contents);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet descriptorSet, uint[] dynamicOffsets)
    {
        fixed (uint* pDynamicOffsets = dynamicOffsets)
        {
            var handle = descriptorSet.Handle;

            PInvoke.vkCmdBindDescriptorSets(this.handle, pipelineBindPoint, layout.Handle, firstSet, 1, &handle, (uint)dynamicOffsets.Length, pDynamicOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, scoped ReadOnlySpan<VkDescriptorSet> descriptorSets, scoped ReadOnlySpan<uint> dynamicOffsets)
    {
        fixed (VkHandle<VkDescriptorSet>* pDescriptorSets = VkHandle.GetHandles(descriptorSets))
        fixed (uint*                      pDynamicOffsets = dynamicOffsets)
        {
            PInvoke.vkCmdBindDescriptorSets(this.handle, pipelineBindPoint, layout.Handle, firstSet, (uint)descriptorSets.Length, pDescriptorSets, (uint)dynamicOffsets.Length, pDynamicOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindIndexBuffer" />
    public void BindIndexBuffer(VkBuffer indexBuffer, ulong offset, VkIndexType indexType) =>
        PInvoke.vkCmdBindIndexBuffer(this.handle, indexBuffer.Handle, offset, indexType);

    /// <inheritdoc cref="PInvoke.vkCmdBindPipeline" />
    public void BindPipeline(VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline) =>
        PInvoke.vkCmdBindPipeline(this.handle, pipelineBindPoint, pipeline.Handle);

    /// <inheritdoc cref="PInvoke.vkCmdBindVertexBuffers" />
    public void BindVertexBuffers(uint firstBinding, uint bindingCount, VkBuffer vertexBuffer, ulong offset)
    {
        var handle = vertexBuffer.Handle;

        PInvoke.vkCmdBindVertexBuffers(this.handle, firstBinding, bindingCount, &handle, &offset);
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindVertexBuffers" />
    public void BindVertexBuffers(uint firstBinding, uint bindingCount, scoped ReadOnlySpan<VkBuffer> vertexBuffers, scoped ReadOnlySpan<ulong> offsets)
    {
        fixed (VkHandle<VkBuffer>* pVertexBuffers = VkHandle.GetHandles(vertexBuffers))
        fixed (ulong*              pOffsets       = offsets)
        {
            PInvoke.vkCmdBindVertexBuffers(this.handle, firstBinding, bindingCount, pVertexBuffers, pOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBlitImage" />
    public void BlitImage(VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, VkImageBlit[] regions, VkFilter filter)
    {
        fixed (VkImageBlit* pRegions = regions)
        {
            PInvoke.vkCmdBlitImage(this.handle, srcImage.Handle, srcImageLayout, dstImage.Handle, dstImageLayout, (uint)regions.Length, pRegions, filter);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdClearAttachments" />
    public void ClearAttachments(scoped ReadOnlySpan<VkClearAttachment> attachments, scoped ReadOnlySpan<VkClearRect> rects)
    {
        fixed (VkClearAttachment* pAttachments = attachments)
        fixed (VkClearRect*       pRects       = rects)
        {
            PInvoke.vkCmdClearAttachments(this.handle, (uint)attachments.Length, pAttachments, (uint)rects.Length, pRects);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdClearColorImage" />
    public void ClearColorImage(VkImage image, VkImageLayout imageLayout, in VkClearColorValue color, scoped ReadOnlySpan<VkImageSubresourceRange> ranges)
    {
        fixed (VkClearColorValue*       pColor  = &color)
        fixed (VkImageSubresourceRange* pRanges = ranges)
        {
            PInvoke.vkCmdClearColorImage(this.handle, image.Handle, imageLayout, pColor, (uint)ranges.Length, pRanges);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBuffer" />
    public void CopyBuffer(VkBuffer source, VkBuffer destination, params VkBufferCopy[] regions)
    {
        fixed (VkBufferCopy* pRegions = regions)
        {
            PInvoke.vkCmdCopyBuffer(this.handle, source.Handle, destination.Handle, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBufferToImage" />
    public void CopyBufferToImage(VkBuffer sourceBuffer, VkImage destinationImage, VkImageLayout transferDstOptimal, scoped ReadOnlySpan<VkBufferImageCopy> regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions)
        {
            PInvoke.vkCmdCopyBufferToImage(this.handle, sourceBuffer.Handle, destinationImage.Handle, transferDstOptimal, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBufferToImage" />
    public void CopyImageToBuffer(VkImage srcImage, VkImageLayout srcImageLayout, VkBuffer dstBuffer, scoped ReadOnlySpan<VkBufferImageCopy> regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions)
        {
            PInvoke.vkCmdCopyImageToBuffer(this.handle, srcImage.Handle, srcImageLayout, dstBuffer.Handle, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkEndCommandBuffer" />
    public void End() =>
        VkException.Check(PInvoke.vkEndCommandBuffer(this.handle));

    /// <inheritdoc cref="PInvoke.vkCmdDraw" />
    public void Draw(uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance) =>
        PInvoke.vkCmdDraw(this.handle, vertexCount, instanceCount, firstVertex, firstInstance);

    /// <inheritdoc cref="PInvoke.vkCmdDrawIndexed" />
    public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance) =>
        PInvoke.vkCmdDrawIndexed(this.handle, indexCount, instanceCount, firstIndex, vertexOffset, firstInstance);

    /// <inheritdoc cref="PInvoke.vkCmdEndRenderPass" />
    public void EndRenderPass() =>
        PInvoke.vkCmdEndRenderPass(this.handle);

    /// <inheritdoc cref="PInvoke.vkCmdNextSubpass" />
    public void NextSubPass(in VkSubpassContents contents) =>
        PInvoke.vkCmdNextSubpass(this.handle, contents);

    /// <inheritdoc cref="PInvoke.vkCmdPipelineBarrier" />
    public void PipelineBarrier(
        VkPipelineStageFlags        srcStageMask,
        VkPipelineStageFlags        dstStageMask,
        VkDependencyFlags           dependencyFlags,
        Span<VkMemoryBarrier>       memoryBarriers,
        Span<VkBufferMemoryBarrier> bufferMemoryBarriers,
        Span<VkImageMemoryBarrier>  imageMemoryBarriers
    )
    {
        fixed (VkMemoryBarrier*       pMemoryBarriers       = memoryBarriers)
        fixed (VkBufferMemoryBarrier* pBufferMemoryBarriers = bufferMemoryBarriers)
        fixed (VkImageMemoryBarrier*  pImageMemoryBarriers  = imageMemoryBarriers)
        {
            PInvoke.vkCmdPipelineBarrier(
                this.handle,
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

    /// <inheritdoc cref="PInvoke.vkCmdPushConstants" />
    public void PushConstants<T>(VkPipelineLayout layout, VkShaderStageFlags stageFlags, in T value) where T : unmanaged
    {
        fixed (T* pValues = &value)
        {
            PInvoke.vkCmdPushConstants(this.handle, layout.Handle, stageFlags, 0, (uint)sizeof(T), pValues);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdSetScissor" />
    public void SetScissor(uint firstScissor, params VkRect2D[] scissors)
    {
        fixed (VkRect2D* pScissors = scissors)
        {
            PInvoke.vkCmdSetScissor(this.handle, firstScissor, (uint)scissors.Length, pScissors);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdSetViewport" />
    public void SetViewport(uint firstViewport, params VkViewport[] viewports)
    {
        fixed (VkViewport* pViewports = viewports)
        {
            PInvoke.vkCmdSetViewport(this.handle, firstViewport, (uint)viewports.Length, pViewports);
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetCommandBuffer" />
    public void Reset(VkCommandBufferResetFlags flags = default) =>
        PInvoke.vkResetCommandBuffer(this.handle, flags);
}
