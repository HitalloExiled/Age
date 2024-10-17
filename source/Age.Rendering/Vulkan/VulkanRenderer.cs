using System.Numerics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Core.Interop;
using Age.Numerics;
using Age.Rendering.Interfaces;
using Age.Rendering.Resources;
using Age.Rendering.Shaders;
using ThirdParty.SpirvCross;
using ThirdParty.SpirvCross.Enums;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Vulkan;

#pragma warning disable CA1822 // TODO Remove;

public unsafe partial class VulkanRenderer : IDisposable
{
    public event Action? SwapchainRecreated;
    private const ushort MAX_DESCRIPTORS_PER_POOL        = 64;
    private const ushort FRAMES_BETWEEN_PENDING_DISPOSES = 2;

    private static VulkanRenderer? singleton;

    public static VulkanRenderer Singleton => singleton ?? throw new NullReferenceException();

    private readonly object padlock = new();

    private readonly Dictionary<VkCommandBuffer, CommandBuffer> commandBuffers = [];
    private readonly Queue<IDisposable>                         pendingDisposes = new();

    private ushort framesUntilPendingDispose;
    private bool   disposed;

    internal VulkanContext Context { get; } = new();

    public uint CurrentFrame => this.Context.Frame.Index;

    public CommandBuffer CurrentCommandBuffer
    {
        get
        {
            var vkCommandBuffer = this.Context.Frame.CommandBuffer;

            if (!this.commandBuffers.TryGetValue(vkCommandBuffer, out var commandBuffer))
            {
                this.commandBuffers[vkCommandBuffer] = commandBuffer = new(vkCommandBuffer, false);
            }

            return commandBuffer;
        }
    }

    public VkQueue GraphicsQueue => this.Context.GraphicsQueue;

    public VkFormat           DepthBufferFormat    { get; private set; }
    public VkFormat           StencilBufferFormat  { get; private set; }
    public VkSampleCountFlags MaxUsableSampleCount { get; private set; }

    public VulkanRenderer()
    {
        singleton = this;

        this.Context.SwapchainRecreated += () => this.SwapchainRecreated?.Invoke();
        this.Context.DeviceInitialized  += this.OnContextDeviceInitialized;
    }

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var loglevel = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagsEXT.Error   => LogLevel.Error,
            VkDebugUtilsMessageSeverityFlagsEXT.Warning => LogLevel.Warning,
            VkDebugUtilsMessageSeverityFlagsEXT.Info    => LogLevel.Info,
            _ => LogLevel.None
        };

        Logger.Log(Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage)!, loglevel);

        return true;
    }

    private VkDescriptorSet[] CreateDescriptorSets(VkDescriptorPool descriptorPool, VkDescriptorSetLayout descriptorSetLayout)
    {
        var descriptorSetLayouts = new VkHandle<VkDescriptorSetLayout>[VulkanContext.MAX_FRAMES_IN_FLIGHT]
        {
            descriptorSetLayout.Handle,
            descriptorSetLayout.Handle,
        };

        fixed (VkHandle<VkDescriptorSetLayout>* pSetLayouts = descriptorSetLayouts)
        {
            var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
            {
                DescriptorSetCount = VulkanContext.MAX_FRAMES_IN_FLIGHT,
                PSetLayouts        = pSetLayouts,
            };

            return descriptorPool.AllocateDescriptorSets(descriptorSetAllocateInfo);
        }
    }

    private VkImageView CreateImageView(Image image, VkImageAspectFlags aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            Format           = image.Format,
            Image            = image.Instance.Handle,
            SubresourceRange = new()
            {
                AspectMask = aspect,
                LayerCount = 1,
                LevelCount = 1,
            },
            ViewType = VkImageViewType.N2D,
        };

        return this.Context.Device.CreateImageView(imageViewCreateInfo);
    }

    private VkShaderModule CreateShaderModule(byte[] buffer)
    {
        fixed (byte* pCode = buffer)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                CodeSize = (uint)buffer.Length,
                PCode    = (uint*)pCode,
            };

            return this.Context.Device.CreateShaderModule(createInfo);
        }
    }

    private void DisposePendingResources(bool immediate = false)
    {
        if (this.pendingDisposes.Count > 0)
        {
            if (immediate || this.framesUntilPendingDispose == 0)
            {
                while (this.pendingDisposes.Count > 0)
                {
                    this.pendingDisposes.Dequeue().Dispose();
                }
            }
            else
            {
                this.framesUntilPendingDispose--;
            }
        }
    }

    private void OnContextDeviceInitialized()
    {
        this.DepthBufferFormat   = this.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint],      VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);
        this.StencilBufferFormat = this.FindSupportedFormat([VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint, VkFormat.D16UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);

        this.Context.Device.PhysicalDevice.GetProperties(out var physicalDeviceProperties);

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        this.MaxUsableSampleCount = counts.HasFlag(VkSampleCountFlags.N64)
            ? VkSampleCountFlags.N64
            : counts.HasFlag(VkSampleCountFlags.N32)
                ? VkSampleCountFlags.N32
                : counts.HasFlag(VkSampleCountFlags.N16)
                    ? VkSampleCountFlags.N16
                    : counts.HasFlag(VkSampleCountFlags.N8)
                        ? VkSampleCountFlags.N8
                        : counts.HasFlag(VkSampleCountFlags.N4)
                            ? VkSampleCountFlags.N4
                            : counts.HasFlag(VkSampleCountFlags.N2)
                                ? VkSampleCountFlags.N2
                                : VkSampleCountFlags.N1;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Context.Device.WaitIdle();

            this.DisposePendingResources(true);

            DescriptorPool.Clear();

            this.Context.Dispose();

            this.disposed = true;
        }
    }

    public CommandBuffer AllocateCommand(VkCommandBufferLevel commandBufferLevel) =>
        new(this.Context.AllocateCommand(commandBufferLevel), true);

    public void BeginFrame()
    {
        this.DisposePendingResources();

        this.Context.PrepareBuffers();

        if (this.Context.Frame.BufferPrepared)
        {
            this.Context.Frame.CommandBuffer.Begin();
        }
    }

    public CommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = this.Context.Frame.CommandPool.AllocateCommand(VkCommandBufferLevel.Primary);

        commandBuffer.Begin(VkCommandBufferUsageFlags.OneTimeSubmit);

        return new(commandBuffer, true);
    }

    public Buffer CreateBuffer(ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags properties)
    {
        var bufferCreateInfo = new VkBufferCreateInfo
        {
            Size  = size,
            Usage = usage,
        };

        var device = this.Context.Device;

        var buffer = device.CreateBuffer(bufferCreateInfo);

        buffer.GetMemoryRequirements(out var memRequirements);

        var memoryType = this.Context.FindMemoryType(memRequirements.MemoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = memoryType
        };

        var memory = device.AllocateMemory(memoryAllocateInfo);

        buffer.BindMemory(memory, 0);

        return new(buffer)
        {
            Allocation = new()
            {
                Alignment  = memRequirements.Alignment,
                Memory     = memory,
                Memorytype = memoryType,
                Offset     = 0,
                Size       = size,
            },
            Usage = usage,
        };
    }

    public IndexBuffer CreateIndexBuffer(Span<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint16);

    public IndexBuffer CreateIndexBuffer(Span<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint32);

    public IndexBuffer CreateIndexBuffer<T>(Span<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
    {
        var bufferSize = (ulong)(sizeof(T) * indices.Length);

        var buffer = this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.IndexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update([..indices]);

        return new()
        {
            Buffer = buffer,
            Type   = indexType,
            Size   = (uint)indices.Length,
        };
    }

    public Framebuffer CreateFramebuffer(in FramebufferCreateInfo createInfo)
    {
        var imageViews = new VkImageView[createInfo.Attachments.Length];

        for (var i = 0; i < createInfo.Attachments.Length; i++)
        {
            imageViews[i] = this.CreateImageView(createInfo.Attachments[i].Image, createInfo.Attachments[i].ImageAspect);
        }

        var extent = new VkExtent2D
        {
            Width  = createInfo.Attachments[0].Image.Extent.Width,
            Height = createInfo.Attachments[0].Image.Extent.Height,
        };

        var framebuffer = this.Context.CreateFrameBuffer(createInfo.RenderPass.Instance, imageViews.AsSpan(), extent);

        return new(framebuffer)
        {
            ImageViews = imageViews,
            Extent     = extent,
        };
    }

    // public RenderPipeline CreateRenderPipeline(in RenderPipelineCreateInfo createInfo)
    // {
    //     var subPasses   = new RenderPassCreateInfo.SubPass[createInfo.SubPasses.Length];
    //     var attachments = new List<FramebufferCreateInfo.Attachment>();

    //     for (var subPassIndex = 0; subPassIndex < createInfo.SubPasses.Length; subPassIndex++)
    //     {
    //         ref var subPass      = ref createInfo.SubPasses[subPassIndex];
    //         var colorAttachments = new RenderPassCreateInfo.ColorAttachment[subPass.ColorAttachments.Length];

    //         ref var renderPassSubPass = ref subPasses[subPassIndex];

    //         for (var i = 0; i < subPass.ColorAttachments.Length; i++)
    //         {
    //             ref var colorAttachment = ref subPass.ColorAttachments[i];

    //             attachments.Add(new(this.CreateImage(colorAttachment.Color.Image), VkImageAspectFlags.Color));

    //             if (colorAttachment.Resolve.HasValue)
    //             {
    //                 attachments.Add(new(this.CreateImage(colorAttachment.Resolve.Value.Image), VkImageAspectFlags.Color));
    //             }

    //             colorAttachments[i] = new()
    //             {
    //                 Color   = colorAttachment.Color.Description,
    //                 Resolve = colorAttachment.Resolve?.Description,
    //             };
    //         }

    //         if (subPass.DepthStencilAttachment.HasValue)
    //         {
    //             attachments.Add(new(this.CreateImage(subPass.DepthStencilAttachment.Value.Image), VkImageAspectFlags.Depth));
    //         }

    //         subPasses[subPassIndex] = new()
    //         {
    //             PipelineBindPoint      = subPass.PipelineBindPoint,
    //             ColorAttachments       = colorAttachments,
    //             DepthStencilAttachment = subPass.DepthStencilAttachment?.Description,
    //         };
    //     }

    //     var renderPassCreateInfo = new RenderPassCreateInfo
    //     {
    //         SubpassDependencies = createInfo.SubpassDependencies,
    //         SubPasses           = subPasses,
    //     };

    //     var renderPass = this.CreateRenderPass(renderPassCreateInfo);

    //     var framebufferCreateInfo = new FramebufferCreateInfo
    //     {
    //         RenderPass  = renderPass,
    //         Attachments = [..attachments],
    //     };

    //     var framebuffer = this.CreateFramebuffer(framebufferCreateInfo);

    //     return new()
    //     {
    //         RenderPass  = renderPass,
    //         Framebuffer = framebuffer,
    //     };
    // }

    public Surface CreateSurface(nint handle, Size<uint> clientSize) =>
        this.Context.CreateSurface(handle, clientSize);

    public Texture CreateTexture(in TextureCreateInfo textureCreateInfo)
    {
        var samples = VkSampleCountFlags.N1;
        var tiling  = VkImageTiling.Optimal;
        var usage   = VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst | VkImageUsageFlags.Sampled;

        var imageCreateInfo = new VkImageCreateInfo
        {
            ArrayLayers   = 1,
            Extent        = new()
            {
                Width  = textureCreateInfo.Width,
                Height = textureCreateInfo.Height,
                Depth  = textureCreateInfo.Depth
            },
            Format        = textureCreateInfo.Format,
            ImageType     = textureCreateInfo.ImageType,
            InitialLayout = VkImageLayout.Undefined,
            MipLevels     = 1,
            Samples       = samples,
            Tiling        = tiling,
            Usage         = usage,
        };

        var image = new Image(imageCreateInfo, VkImageLayout.TransferDstOptimal);

        var imageView = this.CreateImageView(image, VkImageAspectFlags.Color);

        var texture = new Texture(true)
        {
            Image     = image,
            ImageView = imageView,
        };

        return texture;
    }

    public Texture CreateTexture(Image image, bool owner)
    {
        var imageView = this.CreateImageView(image, VkImageAspectFlags.Color);

        var texture = new Texture(owner)
        {
            Image     = image,
            ImageView = imageView,
        };

        return texture;
    }

    public Texture CreateTexture(in TextureCreateInfo textureCreate, Span<byte> data)
    {
        var texture = this.CreateTexture(textureCreate);

        texture.Update(data);

        return texture;
    }

    public VertexBuffer CreateVertexBuffer<T>(Span<T> data) where T : unmanaged
    {
        var size = (ulong)(data.Length * sizeof(T));
        var buffer = this.CreateBuffer(
            size,
            VkBufferUsageFlags.TransferDst | VkBufferUsageFlags.VertexBuffer,
            VkMemoryPropertyFlags.DeviceLocal
        );

        buffer.Update(data);

        return new()
        {
            Buffer = buffer,
            Size   = (uint)data.Length,
        };
    }

    public void DeferredDispose(IDisposable disposable)
    {
        this.framesUntilPendingDispose = FRAMES_BETWEEN_PENDING_DISPOSES;

        this.pendingDisposes.Enqueue(disposable);
    }

    public void DeferredDispose(Span<IDisposable> disposables)
    {
        this.framesUntilPendingDispose = FRAMES_BETWEEN_PENDING_DISPOSES;

        foreach (var disposable in disposables)
        {
            this.pendingDisposes.Enqueue(disposable);
        }
    }

    public void DeferredDispose(IEnumerable<IDisposable> disposables)
    {
        this.framesUntilPendingDispose = FRAMES_BETWEEN_PENDING_DISPOSES;

        foreach (var disposable in disposables)
        {
            this.pendingDisposes.Enqueue(disposable);
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void EndFrame()
    {
        if (this.Context.Frame.BufferPrepared)
        {
            this.Context.Frame.CommandBuffer.End();
        }

        this.Context.SwapBuffers();
    }

    public void EndSingleTimeCommands(CommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var commandBufferHandle = commandBuffer.Instance.Handle;

        var submitInfo = new VkSubmitInfo
        {
            CommandBufferCount = 1,
            PCommandBuffers    = &commandBufferHandle
        };

        this.Context.GraphicsQueue.Submit(submitInfo);
        this.Context.GraphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    public VkFormat FindSupportedFormat(Span<VkFormat> candidates, VkImageTiling tiling, VkFormatFeatureFlags features) =>
        this.Context.FindSupportedFormat(candidates, tiling, features);

    public void UpdateDescriptorSets(Span<VkWriteDescriptorSet> descriptorWrites, Span<VkCopyDescriptorSet> descriptorCopies) =>
        this.Context.Device.UpdateDescriptorSets(descriptorWrites, descriptorCopies);

    public void WaitIdle() =>
        this.Context.Device.WaitIdle();
}
