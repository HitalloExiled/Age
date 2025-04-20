using System.Numerics;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Extensions;
using Age.Numerics;
using Age.Rendering.Resources;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;

using Buffer = Age.Rendering.Resources.Buffer;

namespace Age.Rendering.Vulkan;

public sealed unsafe partial class VulkanRenderer : Disposable
{
    public event Action? SwapchainRecreated;
    private const ushort MAX_DESCRIPTORS_PER_POOL        = 64;
    private const ushort FRAMES_BETWEEN_PENDING_DISPOSES = 2;

    private static VulkanRenderer? singleton;

    public static VulkanRenderer Singleton => singleton ?? throw new NullReferenceException();

    private readonly Lock                                       @lock           = new();
    private readonly Dictionary<VkCommandBuffer, CommandBuffer> commandBuffers  = [];
    private readonly List<PendingDisposable>                    pendingDisposes = [];

    private bool disposingPendingResources;

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

    private void AddPendingDisposes(IDisposable disposable) =>
        this.pendingDisposes.Add(new(disposable, FRAMES_BETWEEN_PENDING_DISPOSES));

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

    private void DisposePendingResources(bool immediate = false)
    {
        lock (this.@lock)
        {
            this.disposingPendingResources = true;

            if (immediate)
            {
                foreach (var item in this.pendingDisposes)
                {
                    item.Disposable.Dispose();
                }

                this.pendingDisposes.Clear();
            }
            else
            {
                var span = this.pendingDisposes.AsSpan();

                for (var i = 0; i < this.pendingDisposes.Count; i++)
                {
                    if (span[i].FramesRemaining == 0)
                    {
                        span[i].Disposable.Dispose();
                        this.pendingDisposes.RemoveAt(i);

                        i--;
                    }
                    else
                    {
                        span[i].FramesRemaining--;
                    }
                }
            }

            this.disposingPendingResources = false;
        }
    }

    private void OnContextDeviceInitialized()
    {
        this.DepthBufferFormat   = this.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint],      VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);
        this.StencilBufferFormat = this.FindSupportedFormat([VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint, VkFormat.D16UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);

        this.Context.Device.PhysicalDevice.GetProperties(out var physicalDeviceProperties);

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        this.MaxUsableSampleCount = counts.HasFlags(VkSampleCountFlags.N64)
            ? VkSampleCountFlags.N64
            : counts.HasFlags(VkSampleCountFlags.N32)
                ? VkSampleCountFlags.N32
                : counts.HasFlags(VkSampleCountFlags.N16)
                    ? VkSampleCountFlags.N16
                    : counts.HasFlags(VkSampleCountFlags.N8)
                        ? VkSampleCountFlags.N8
                        : counts.HasFlags(VkSampleCountFlags.N4)
                            ? VkSampleCountFlags.N4
                            : counts.HasFlags(VkSampleCountFlags.N2)
                                ? VkSampleCountFlags.N2
                                : VkSampleCountFlags.N1;
    }

    protected override void OnDisposed(bool disposing)
    {
        if (disposing)
        {
            this.Context.Device.WaitIdle();

            this.DisposePendingResources(true);

            DescriptorPool.Clear();

            this.Context.Dispose();
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

    public IndexBuffer CreateIndexBuffer(scoped ReadOnlySpan<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint16);

    public IndexBuffer CreateIndexBuffer(scoped ReadOnlySpan<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.Uint32);

    public IndexBuffer CreateIndexBuffer<T>(scoped ReadOnlySpan<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
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

    public Surface CreateSurface(nint handle, Size<uint> clientSize) =>
        this.Context.CreateSurface(handle, clientSize);
    public VertexBuffer CreateVertexBuffer<T>(scoped ReadOnlySpan<T> data) where T : unmanaged
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
        lock (this.@lock)
        {
            if (this.disposingPendingResources)
            {
                disposable.Dispose();
            }
            else
            {
                this.AddPendingDisposes(disposable);
            }
        }
    }

    public void DeferredDispose(scoped ReadOnlySpan<IDisposable> disposables)
    {
        lock (this.@lock)
        {
            if (this.disposingPendingResources)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                foreach (var disposable in disposables)
                {
                    this.AddPendingDisposes(disposable);
                }
            }
        }
    }

    public void DeferredDispose(IEnumerable<IDisposable> disposables)
    {
        lock (this.@lock)
        {
            if (this.disposingPendingResources)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                foreach (var disposable in disposables)
                {
                    this.AddPendingDisposes(disposable);
                }
            }
        }
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

    public VkFormat FindSupportedFormat(scoped ReadOnlySpan<VkFormat> candidates, VkImageTiling tiling, VkFormatFeatureFlags features) =>
        this.Context.FindSupportedFormat(candidates, tiling, features);

    public void UpdateDescriptorSets(scoped ReadOnlySpan<VkWriteDescriptorSet> descriptorWrites, scoped ReadOnlySpan<VkCopyDescriptorSet> descriptorCopies) =>
        this.Context.Device.UpdateDescriptorSets(descriptorWrites, descriptorCopies);

    public void WaitIdle() =>
        this.Context.Device.WaitIdle();
}
