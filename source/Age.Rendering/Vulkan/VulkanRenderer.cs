using Age.Core.Extensions;
using Age.Core;
using Age.Numerics;
using Age.Rendering.Resources;
using System.Runtime.InteropServices;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan;

namespace Age.Rendering.Vulkan;

public sealed unsafe partial class VulkanRenderer : Disposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL        = 64;
    private const ushort FRAMES_BETWEEN_PENDING_DISPOSES = 2;

    public static VulkanRenderer Singleton { get; private set; } = null!;

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
                this.commandBuffers[vkCommandBuffer] = commandBuffer = new(vkCommandBuffer);
            }

            return commandBuffer;
        }
    }

    public VkQueue GraphicsQueue => this.Context.GraphicsQueue;

    public TextureFormat DepthBufferFormat    { get; private set; }
    public TextureFormat StencilBufferFormat  { get; private set; }
    public SampleCount   MaxUsableSampleCount { get; private set; }

    public VulkanRenderer()
    {
        Singleton = this;

        this.Context.DeviceInitialized += this.OnContextDeviceInitialized;
    }

    private static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT _1, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* _2)
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
        this.DepthBufferFormat   = (TextureFormat)this.FindSupportedFormat([VkFormat.D32Sfloat, VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint],      VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);
        this.StencilBufferFormat = (TextureFormat)this.FindSupportedFormat([VkFormat.D32SfloatS8Uint, VkFormat.D24UnormS8Uint, VkFormat.D16UnormS8Uint], VkImageTiling.Optimal, VkFormatFeatureFlags.DepthStencilAttachment);

        this.Context.Device.PhysicalDevice.GetProperties(out var physicalDeviceProperties);

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        this.MaxUsableSampleCount = (SampleCount)
        (
            counts.HasFlags(VkSampleCountFlags.N64)
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
                                : VkSampleCountFlags.N1
        );
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

    public void BeginFrame()
    {
        this.DisposePendingResources();

        this.Context.PrepareBuffers();

        if (this.Context.Frame.BufferPrepared)
        {
            this.Context.Frame.CommandBuffer.Begin();
        }
    }

    public Surface CreateSurface(nint handle, Size<uint> clientSize) =>
        this.Context.CreateSurface(handle, clientSize);

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

    public void DeferredDispose(ReadOnlySpan<IDisposable> disposables)
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

    public VkFormat FindSupportedFormat(ReadOnlySpan<VkFormat> candidates, VkImageTiling tiling, VkFormatFeatureFlags features) =>
        this.Context.FindSupportedFormat(candidates, tiling, features);

    public void UpdateDescriptorSets(ReadOnlySpan<VkWriteDescriptorSet> descriptorWrites, ReadOnlySpan<VkCopyDescriptorSet> descriptorCopies) =>
        this.Context.Device.UpdateDescriptorSets(descriptorWrites, descriptorCopies);

    public void WaitIdle() =>
        this.Context.Device.WaitIdle();
}
