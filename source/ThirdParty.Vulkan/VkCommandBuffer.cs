using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class VkCommandBuffer : DisposableManagedHandle<VkCommandBuffer>
{
    private readonly VkDevice      device;
    private readonly VkCommandPool commandPool;

    internal VkCommandBuffer(VkHandle<VkCommandBuffer> handle, VkDevice device, VkCommandPool commandPool) : base(handle)
    {
        this.device      = device;
        this.commandPool = commandPool;
    }

    protected override void OnDispose()
    {
        fixed (VkHandle<VkCommandBuffer>* pHandle = &this.Handle)
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

        VkException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, &commandBufferBeginInfo));
    }

    /// <inheritdoc cref="PInvoke.vkBeginCommandBuffer" />
    public void Begin(in VkCommandBufferBeginInfo beginInfo)
    {
        fixed (VkCommandBufferBeginInfo* pBeginInfo = &beginInfo)
        {
            VkException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, pBeginInfo));
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBeginRenderPass" />
    public void BeginRenderPass(in VkRenderPassBeginInfo beginInfo, VkSubpassContents contents)
    {
        fixed (VkRenderPassBeginInfo* pBeginInfo = &beginInfo)
        {
            PInvoke.vkCmdBeginRenderPass(this.Handle, pBeginInfo, contents);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet descriptorSet, uint[] dynamicOffsets)
    {
        fixed (VkHandle<VkDescriptorSet>* pDescriptorSets = &descriptorSet.Handle)
        fixed (uint*                      pDynamicOffsets = dynamicOffsets)
        {
            PInvoke.vkCmdBindDescriptorSets(this.Handle, pipelineBindPoint, layout.Handle, firstSet, 1, pDescriptorSets, (uint)dynamicOffsets.Length, pDynamicOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(VkPipelineBindPoint pipelineBindPoint, VkPipelineLayout layout, uint firstSet, VkDescriptorSet[] descriptorSets, uint[] dynamicOffsets)
    {
        fixed (VkHandle<VkDescriptorSet>* pDescriptorSets = ToHandlers(descriptorSets))
        fixed (uint*                      pDynamicOffsets = dynamicOffsets)
        {
            PInvoke.vkCmdBindDescriptorSets(this.Handle, pipelineBindPoint, layout.Handle, firstSet, (uint)descriptorSets.Length, pDescriptorSets, (uint)dynamicOffsets.Length, pDynamicOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindIndexBuffer" />
    public void BindIndexBuffer(VkBuffer indexBuffer, ulong offset, VkIndexType indexType) =>
        PInvoke.vkCmdBindIndexBuffer(this.Handle, indexBuffer.Handle, offset, indexType);

    /// <inheritdoc cref="PInvoke.vkCmdBindPipeline" />
    public void BindPipeline(VkPipelineBindPoint pipelineBindPoint, VkPipeline pipeline) =>
        PInvoke.vkCmdBindPipeline(this.Handle, pipelineBindPoint, pipeline.Handle);

    /// <inheritdoc cref="PInvoke.vkCmdBindVertexBuffers" />
    public void BindVertexBuffers(uint firstBinding, uint bindingCount, VkBuffer vertexBuffer, ulong offset)
    {
        fixed (VkHandle<VkBuffer>* pBuffers = &vertexBuffer.Handle)
        {
            PInvoke.vkCmdBindVertexBuffers(this.Handle, firstBinding, bindingCount, pBuffers, &offset);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBindVertexBuffers" />
    public void BindVertexBuffers(uint firstBinding, uint bindingCount, VkBuffer[] vertexBuffers, ulong[] offsets)
    {
        fixed (VkHandle<VkBuffer>* pVertexBuffers = ToHandlers(vertexBuffers))
        fixed (ulong*              pOffsets       = offsets)
        {
            PInvoke.vkCmdBindVertexBuffers(this.Handle, firstBinding, bindingCount, pVertexBuffers, pOffsets);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdBlitImage" />
    public void BlitImage(VkImage srcImage, VkImageLayout srcImageLayout, VkImage dstImage, VkImageLayout dstImageLayout, VkImageBlit[] regions, VkFilter filter)
    {
        fixed (VkImageBlit* pRegions = regions)
        {
            PInvoke.vkCmdBlitImage(this.Handle, srcImage.Handle, srcImageLayout, dstImage.Handle, dstImageLayout, (uint)regions.Length, pRegions, filter);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBuffer" />
    public void CopyBuffer(VkBuffer source, VkBuffer destination, params VkBufferCopy[] regions)
    {
        fixed (VkBufferCopy* pRegions = regions)
        {
            PInvoke.vkCmdCopyBuffer(this.Handle, source.Handle, destination.Handle, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdCopyBufferToImage" />
    public void CopyBufferToImage(VkBuffer sourceBuffer, VkImage destinationImage, VkImageLayout transferDstOptimal, params VkBufferImageCopy[] regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions)
        {
            PInvoke.vkCmdCopyBufferToImage(this.Handle, sourceBuffer.Handle, destinationImage.Handle, transferDstOptimal, (uint)regions.Length, pRegions);
        }
    }

    /// <inheritdoc cref="PInvoke.vkEndCommandBuffer" />
    public void End() =>
        VkException.Check(PInvoke.vkEndCommandBuffer(this.Handle));

    /// <inheritdoc cref="PInvoke.vkCmdDrawIndexed" />
    public void DrawIndexed(uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance) =>
        PInvoke.vkCmdDrawIndexed(this.Handle, indexCount, instanceCount, firstIndex, vertexOffset, firstInstance);

    /// <inheritdoc cref="PInvoke.vkCmdEndRenderPass" />
    public void EndRenderPass() =>
        PInvoke.vkCmdEndRenderPass(this.Handle);

    /// <inheritdoc cref="PInvoke.vkCmdPipelineBarrier" />
    public void PipelineBarrier(
        VkPipelineStageFlags    srcStageMask,
        VkPipelineStageFlags    dstStageMask,
        VkDependencyFlags       dependencyFlags,
        VkMemoryBarrier[]       memoryBarriers,
        VkBufferMemoryBarrier[] bufferMemoryBarriers,
        VkImageMemoryBarrier[]  imageMemoryBarriers
    )
    {
        fixed (VkMemoryBarrier*       pMemoryBarriers       = memoryBarriers)
        fixed (VkBufferMemoryBarrier* pBufferMemoryBarriers = bufferMemoryBarriers)
        fixed (VkImageMemoryBarrier*  pImageMemoryBarriers  = imageMemoryBarriers)
        {
            PInvoke.vkCmdPipelineBarrier(
                this.Handle,
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
    public void SetScissor(uint firstScissor, params VkRect2D[] scissors)
    {
        fixed (VkRect2D* pScissors = scissors)
        {
            PInvoke.vkCmdSetScissor(this.Handle, firstScissor, (uint)scissors.Length, pScissors);
        }
    }

    /// <inheritdoc cref="PInvoke.vkCmdSetViewport" />
    public void SetViewport(uint firstViewport, params VkViewport[] viewports)
    {
        fixed (VkViewport* pViewports = viewports)
        {
            PInvoke.vkCmdSetViewport(this.Handle, firstViewport, (uint)viewports.Length, pViewports);
        }
    }

    /// <inheritdoc cref="PInvoke.vkResetCommandBuffer" />
    public void Reset(VkCommandBufferResetFlags flags = default) =>
        PInvoke.vkResetCommandBuffer(this.Handle, flags);
}
