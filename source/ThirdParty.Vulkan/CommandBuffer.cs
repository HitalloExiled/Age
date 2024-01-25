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

    public void Begin(CommandBufferUsageFlags flags = default)
    {
        var commandBufferBeginInfo = new VkCommandBufferBeginInfo
        {
            flags = flags,
        };

        VulkanException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, &commandBufferBeginInfo));
    }

    public void Begin(BeginInfo beginInfo) =>
        VulkanException.Check(PInvoke.vkBeginCommandBuffer(this.Handle, beginInfo));

    public void BeginRenderPass(RenderPass.BeginInfo renderPassInfo, VkSubpassContents inline) => throw new NotImplementedException();

    /// <inheritdoc cref="PInvoke.vkCmdBindDescriptorSets" />
    public void BindDescriptorSets(VkPipelineBindPoint pipelineBindPoint, PipelineLayout layout, uint firstSet, DescriptorSet[] descriptorSets, uint[] dynamicOffsets) => throw new NotImplementedException();
    public void BindIndexBuffer(Buffer indexBuffer, int v, VkIndexType uint32) => throw new NotImplementedException();
    public void BindPipeline(VkPipelineBindPoint graphics, Pipeline graphicsPipeline) => throw new NotImplementedException();
    public void BindVertexBuffers(int v, Buffer[] vertexBuffers, ulong[] offsets) => throw new NotImplementedException();

    /// <inheritdoc cref="PInvoke.vkCmdBlitImage" />
    public void BlitImage(Image srcImage, VkImageLayout srcImageLayout, Image dstImage, VkImageLayout dstImageLayout, ImageBlit[] regions, Filter filter) => throw new NotImplementedException();

    public void CopyBuffer(Buffer source, Buffer destination, params BufferCopy[] regions)
    {
        fixed (VkBufferCopy* pRegions = regions.Select(x => (VkBufferCopy)x).ToArray())
        {
            PInvoke.vkCmdCopyBuffer(this.Handle, source, destination, (uint)regions.Length, pRegions);
        }
    }

    public void CopyBufferToImage(Buffer sourceBuffer, Image destinationImage, ImageLayout transferDstOptimal, params BufferImageCopy[] regions)
    {
        fixed (VkBufferImageCopy* pRegions = regions.Select(x => (VkBufferImageCopy)x).ToArray())
        {
            PInvoke.vkCmdCopyBufferToImage(this.Handle, sourceBuffer, destinationImage, transferDstOptimal, (uint)regions.Length, pRegions);
        }
    }

    public void End() =>
        VulkanException.Check(PInvoke.vkEndCommandBuffer(this.Handle));

    public void DrawIndexed(uint count, int v1, int v2, int v3, int v4) => throw new NotImplementedException();
    public void EndRenderPass(CommandBuffer commandBuffer) => throw new NotImplementedException();

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
        fixed (VkMemoryBarrier*       pMemoryBarriers       = memoryBarriers.CastToNative())
        fixed (VkBufferMemoryBarrier* pBufferMemoryBarriers = bufferMemoryBarriers.CastToNative())
        fixed (VkImageMemoryBarrier*  pImageMemoryBarriers  = imageMemoryBarriers.CastToNative())
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

    public void SetScissor(int v, Rect2D scissor) => throw new NotImplementedException();
    public void SetViewport(int v, Viewport viewport) => throw new NotImplementedException();

    public void Reset() => throw new NotImplementedException();
}
