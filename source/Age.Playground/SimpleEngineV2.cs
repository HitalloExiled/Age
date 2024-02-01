#define SIMPLE_ENGINE_V2
#if SIMPLE_ENGINE_V2
using System.Diagnostics;
using System.Runtime.InteropServices;
using Age.Core.Interop;
using Age.Numerics;
using Age.Platforms.Abstractions;
using SkiaSharp;
using ThirdParty.Vulkan;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Enums.KHR;
using ThirdParty.Vulkan.EXT;
using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Flags.EXT;
using ThirdParty.Vulkan.KHR;

using WavefrontLoader = Age.Resources.Loaders.Wavefront.Loader;
using Buffer          = ThirdParty.Vulkan.Buffer;
using Semaphore       = ThirdParty.Vulkan.Semaphore;
using ThirdParty.Vulkan.Extensions.EXT;
using ThirdParty.Vulkan.Extensions.KHR;
using Version = ThirdParty.Vulkan.Version;
using System.Runtime.CompilerServices;
using ThirdParty.Vulkan.Flags.KHR;

using PlatformWindow = Age.Platforms.Display.Window;

namespace Age.Playground;

public unsafe partial class SimpleEngineV2 : IDisposable
{
    private const int MAX_FRAMES_IN_FLIGHT = 2;

    private static readonly DateTime startTime = DateTime.UtcNow;

    private readonly HashSet<string>        deviceExtensions         = [SwapchainExtension.Name];
    private readonly bool                   enableValidationLayers   = Debugger.IsAttached;
    private readonly Semaphore[]            imageAvailableSemaphores = new Semaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly List<uint>             indices                  = [];
    private readonly Fence[]                inFlightFences           = new Fence[MAX_FRAMES_IN_FLIGHT];
    private readonly WavefrontLoader        wavefrontLoader          = new(new FileSystem());
    private readonly Semaphore[]            renderFinishedSemaphores = new Semaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly Buffer[]               uniformBuffers           = new Buffer[MAX_FRAMES_IN_FLIGHT];
    private readonly UniformBufferObject[]  uniformBuffersMapped     = new UniformBufferObject[MAX_FRAMES_IN_FLIGHT];
    private readonly DeviceMemory[]         uniformBuffersMemory     = new DeviceMemory[MAX_FRAMES_IN_FLIGHT];
    private readonly HashSet<string>        validationLayers         = ["VK_LAYER_KHRONOS_validation"];
    private readonly List<Vertex>           vertices                 = [];

    private Image                     colorImage = null!;
    private DeviceMemory              colorImageMemory = null!;
    private ImageView                 colorImageView = null!;
    private CommandBuffer[]           commandBuffers = [];
    private CommandPool               commandPool = null!;
    private uint                      currentFrame;
    private DebugUtilsMessenger       debugMessenger = null!;
    private Image                     depthImage = null!;
    private DeviceMemory              depthImageMemory = null!;
    private ImageView                 depthImageView = null!;
    private DescriptorPool            descriptorPool = null!;
    private DescriptorSetLayout       descriptorSetLayout = null!;
    private DescriptorSet[]           descriptorSets = [];
    private Device                    device = null!;
    private bool                      disposed;
    private bool                      framebufferResized;
    private Pipeline                  graphicsPipeline = null!;
    private Queue                     graphicsQueue = null!;
    private Buffer                    indexBuffer = null!;
    private DeviceMemory              indexBufferMemory = null!;
    private Instance                  instance = null!;
    private uint                      mipLevels;
    private SampleCountFlags          msaaSamples = SampleCountFlags.N1;
    private PhysicalDevice            physicalDevice = null!;
    private PipelineLayout            pipelineLayout = null!;
    private Queue                     presentQueue = null!;
    private RenderPass                renderPass = null!;
    private Surface                   surface = null!;
    private Swapchain                 swapChain = null!;
    private Extent2D                  swapChainExtent = new();
    private Framebuffer[]             swapChainFramebuffers = [];
    private Format                    swapChainImageFormat;
    private Image[]                   swapChainImages = [];
    private ImageView[]               swapChainImageViews = [];
    private Image                     textureImage = null!;
    private DeviceMemory              textureImageMemory = null!;
    private ImageView                 textureImageView = null!;
    private Sampler                   textureSampler = null!;
    private Buffer                    vertexBuffer = null!;
    private DeviceMemory              vertexBufferMemory = null!;
    private DebugUtilsExtension?      debugUtilsExtension;
    private SurfaceExtension          surfaceExtension = null!;
    private SwapchainExtension        swapchainExtension = null!;
    private PlatformWindow            window = null!;

    private static PresentMode ChooseSwapPresentMode(PresentMode[] availablePresentModes)
    {
        foreach (var availablePresentMode in availablePresentModes)
        {
            if (availablePresentMode == PresentMode.Mailbox)
            {
                return availablePresentMode;
            }
        }

        return PresentMode.Fifo;
    }

    private static SurfaceFormat ChooseSwapSurfaceFormat(SurfaceFormat[] availableFormats)
    {
        foreach (var availableFormat in availableFormats)
        {
            if (availableFormat.Format == Format.B8G8R8A8Srgb && availableFormat.ColorSpace == ColorSpace.SrgbNonlinear)
            {
                return availableFormat;
            }
        }

        return availableFormats[0];
    }

    private static bool DebugCallback(DebugUtilsMessageSeverityFlags messageSeverity, DebugUtilsMessageTypeFlags messageType, DebugUtilsMessenger.CallbackData callbackData)
    {
        Console.WriteLine("validation layer: " + callbackData.Message);

        return false;
    }

    private static void PopulateDebugMessengerCreateInfo(out DebugUtilsMessenger.CreateInfo createInfo) =>
        createInfo = new DebugUtilsMessenger.CreateInfo
        {
            MessageSeverity = DebugUtilsMessageSeverityFlags.Verbose | DebugUtilsMessageSeverityFlags.Warning | DebugUtilsMessageSeverityFlags.Error,
            MessageType     = DebugUtilsMessageTypeFlags.General | DebugUtilsMessageTypeFlags.Validation | DebugUtilsMessageTypeFlags.Performance,
            UserCallback    = DebugCallback,
        };

    private CommandBuffer BeginSingleTimeCommands()
    {
        var commandBuffer = this.commandPool.AllocateCommand(CommandBufferLevelFlags.Primary);

        commandBuffer.Begin();

        return commandBuffer;
    }

    private bool CheckDeviceExtensionSupport(PhysicalDevice physicalDevice)
    {
        var extensions = physicalDevice.EnumerateDeviceExtensionProperties();

        return this.deviceExtensions.Overlaps(extensions.Select(x => x.ExtensionName));
    }

    private bool CheckValidationLayerSupport()
    {
        var availableLayers = Instance.EnumerateLayerProperties();

        return this.validationLayers.Overlaps(availableLayers.Select(x => x.LayerName));
    }

    private Extent2D ChooseSwapExtent(SurfaceCapabilities capabilities)
    {
        if (capabilities.CurrentExtent.Width != uint.MaxValue)
        {
            return capabilities.CurrentExtent;
        }
        else
        {
            var actualExtent = new Extent2D
            {
                Width  = Math.Clamp(this.window.Size.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                Height = Math.Clamp(this.window.Size.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height),
            };

            return actualExtent;
        }
    }

    private void Cleanup()
    {
        this.CleanupSwapChain();

        this.textureSampler.Dispose();
        this.textureImageView.Dispose();
        this.textureImage.Dispose();
        this.textureImageMemory.Dispose();
        this.graphicsPipeline.Dispose();
        this.pipelineLayout.Dispose();
        this.renderPass.Dispose();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.uniformBuffers[i].Dispose();
            this.uniformBuffersMemory[i].Dispose();
        }

        this.descriptorPool.Dispose();
        this.descriptorSetLayout.Dispose();
        this.indexBuffer.Dispose();
        this.indexBufferMemory.Dispose();
        this.vertexBuffer.Dispose();
        this.vertexBufferMemory.Dispose();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.imageAvailableSemaphores[i].Dispose();
            this.renderFinishedSemaphores[i].Dispose();
            this.inFlightFences[i].Dispose();
        }

        this.commandPool.Dispose();
        this.device.Dispose();

        if (this.enableValidationLayers && this.debugUtilsExtension != null)
        {
            this.debugMessenger.Dispose();
        }

        this.surface.Dispose();
        this.instance.Dispose();
    }

    private void CleanupSwapChain()
    {
        this.depthImageView.Dispose();
        this.depthImage.Dispose();
        this.depthImageMemory.Dispose();

        this.colorImageView.Dispose();
        this.colorImage.Dispose();
        this.colorImageMemory.Dispose();

        for (var i = 0; i < this.swapChainFramebuffers.Length; i++)
        {
            this.swapChainFramebuffers[i].Dispose();
        }

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            this.swapChainImageViews[i].Dispose();
        }

        this.swapChain.Dispose();
    }

    private void CopyBuffer(Buffer srcBuffer, Buffer dstBuffer, ulong size)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var copyRegion = new BufferCopy
        {
            Size = size
        };

        commandBuffer.CopyBuffer(srcBuffer, dstBuffer, copyRegion);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(Buffer buffer, Image image, int width, int height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var region = new BufferImageCopy
        {
            BufferOffset      = 0,
            BufferRowLength   = 0,
            BufferImageHeight = 0,
            ImageSubresource  = new()
            {
                AspectMask     = ImageAspectFlags.Color,
                MipLevel       = 0,
                BaseArrayLayer = 0,
                LayerCount     = 1,
            },
            ImageOffset = new(),
            ImageExtent = new(){ Width = (uint)width, Height = (uint)height },
        };

        commandBuffer.CopyBufferToImage(buffer, image, ImageLayout.TransferDstOptimal, region);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CreateBuffer(ulong size, BufferUsageFlags usage, MemoryPropertyFlags properties, out Buffer buffer, out DeviceMemory bufferMemory)
    {
        var createInfo = new Buffer.CreateInfo
        {
            Size        = size,
            Usage       = usage,
            SharingMode = SharingMode.Exclusive
        };

        buffer = this.device.CreateBuffer(createInfo);

        var memRequirements = buffer.GetMemoryRequirements();

        var allocInfo = new DeviceMemory.AllocateInfo
        {
            AllocationSize = memRequirements.Size,
            MemoryTypeIndex = this.FindMemoryType(memRequirements.MemoryTypeBits, properties)
        };

        bufferMemory = this.device.AllocateMemory(allocInfo);

        buffer.BindMemory(bufferMemory, 0);
    }

    private void CreateCommandBuffers() =>
        this.commandBuffers = this.commandPool.AllocateCommands(MAX_FRAMES_IN_FLIGHT, CommandBufferLevelFlags.Primary);

    private void CreateCommandPool()
    {
        var queueFamilyIndices = this.FindQueueFamilies(this.physicalDevice);

        this.commandPool = this.device.CreateCommandPool(queueFamilyIndices.GraphicsFamily!.Value, CommandPoolCreateFlags.ResetCommandBuffer);
    }

    private void CreateColorResources()
    {
        var colorFormat = this.swapChainImageFormat;

        this.CreateImage(
            this.swapChainExtent.Width,
            this.swapChainExtent.Height,
            1,
            this.msaaSamples,
            colorFormat,
            ImageTiling.Optimal,
            ImageUsageFlags.TransientAttachment | ImageUsageFlags.ColorAttachment,
            MemoryPropertyFlags.DeviceLocal,
            out this.colorImage,
            out this.colorImageMemory
        );
        this.colorImageView = this.CreateImageView(this.colorImage, colorFormat, ImageAspectFlags.Color, 1);
    }

    private void CreateDepthResources()
    {
        var depthFormat = this.FindDepthFormat();

        this.CreateImage(
            this.swapChainExtent.Width,
            this.swapChainExtent.Height,
            1,
            this.msaaSamples,
            depthFormat,
            ImageTiling.Optimal,
            ImageUsageFlags.DepthStencilAttachment,
            MemoryPropertyFlags.DeviceLocal,
            out this.depthImage,
            out this.depthImageMemory
        );

        this.depthImageView = this.CreateImageView(this.depthImage, depthFormat, ImageAspectFlags.Depth, 1);
    }

    private void CreateDescriptorPool()
    {
        var createInfo = new DescriptorPool.CreateInfo
        {
            PoolSizes =
            [
                new()
                {
                    Type            = DescriptorType.UniformBuffer,
                    DescriptorCount = MAX_FRAMES_IN_FLIGHT,
                },
                new()
                {
                    Type            = DescriptorType.CombinedImageSampler,
                    DescriptorCount = MAX_FRAMES_IN_FLIGHT,
                },
            ],
            MaxSets = MAX_FRAMES_IN_FLIGHT,
        };

        this.descriptorPool = this.device.CreateDescriptorPool(createInfo);
    }

    private void CreateDescriptorSets()
    {
        var layouts = new DescriptorSetLayout[MAX_FRAMES_IN_FLIGHT]
        {
            this.descriptorSetLayout,
            this.descriptorSetLayout,
        };

        var allocInfo = new DescriptorSet.AllocateInfo
        {
            SetLayouts = layouts
        };

        this.descriptorSets = this.descriptorPool.AllocateDescriptorSets(allocInfo);

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            var bufferInfo = new DescriptorBufferInfo
            {
                Buffer = this.uniformBuffers[i],
                Offset = 0,
                Range  = (uint)Marshal.SizeOf<UniformBufferObject>()
            };

            var imageInfo = new DescriptorImageInfo
            {
                ImageLayout = ImageLayout.ShaderReadOnlyOptimal,
                ImageView   = this.textureImageView,
                Sampler     = this.textureSampler
            };

            var descriptorWrites = new WriteDescriptorSet[]
            {
                new()
                {
                    BufferInfo      = [bufferInfo],
                    DescriptorCount = 1,
                    DescriptorType  = DescriptorType.UniformBuffer,
                    DstArrayElement = 0,
                    DstBinding      = 0,
                    DstSet          = this.descriptorSets[i],
                },
                new()
                {
                    DescriptorCount = 1,
                    DescriptorType  = DescriptorType.CombinedImageSampler,
                    DstArrayElement = 0,
                    DstBinding      = 1,
                    DstSet          = this.descriptorSets[i],
                    ImageInfo       = [imageInfo],
                }
            };

            this.device.UpdateDescriptorSets(descriptorWrites, []);
        }
    }

    private void CreateDescriptorSetLayout()
    {
        var uboLayoutBinding = new DescriptorSetLayoutBinding
        {
            Binding         = 0,
            DescriptorType  = DescriptorType.UniformBuffer,
            DescriptorCount = 1,
            StageFlags      = ShaderStageFlags.Vertex,
        };

        var samplerLayoutBinding = new DescriptorSetLayoutBinding
        {
            Binding         = 1,
            DescriptorCount = 1,
            DescriptorType  = DescriptorType.CombinedImageSampler,
            StageFlags      = ShaderStageFlags.Fragment
        };

        var bindings = new[]
        {
            uboLayoutBinding,
            samplerLayoutBinding
        };

        var createInfo = new DescriptorSetLayout.CreateInfo
        {
            Bindings = bindings
        };

        this.descriptorSetLayout = this.device.CreateDescriptorSetLayout(createInfo);
    }

    private void CreateFramebuffers()
    {
        this.swapChainFramebuffers = new Framebuffer[this.swapChainImageViews.Length];

        for (var i = 0; i < this.swapChainImageViews.Length; i++)
        {
            var attachments = new[]
            {
                this.colorImageView,
                this.depthImageView,
                this.swapChainImageViews[i]
            };

            var createInfo = new Framebuffer.CreateInfo
            {
                RenderPass  = this.renderPass,
                Attachments = attachments,
                Width       = this.swapChainExtent.Width,
                Height      = this.swapChainExtent.Height,
                Layers      = 1
            };

            this.swapChainFramebuffers[i] = this.device.CreateFramebuffer(createInfo);
        }
    }

    private void CreateGraphicsPipeline()
    {
        var vertShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/vert.spv"))!;
        var fragShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/frag.spv"))!;

        var vertShaderModule = this.CreateShaderModule(vertShaderCode);
        var fragShaderModule = this.CreateShaderModule(fragShaderCode);

        var name = "main";
        var vertShaderStageInfo = new PipelineShaderStage.CreateInfo
        {
            Module = vertShaderModule,
            Name   = name,
            Stage  = ShaderStageFlags.Vertex,
        };

        var fragShaderStageInfo = new PipelineShaderStage.CreateInfo
        {
            Module = fragShaderModule,
            Name   = name,
            Stage  = ShaderStageFlags.Fragment,
        };

        var shaderStages = new[]
        {
            vertShaderStageInfo,
            fragShaderStageInfo
        };

        var bindingDescription    = Vertex.GetBindingDescription();
        var attributeDescriptions = Vertex.GetAttributeDescriptions();

        var vertexInputInfo = new PipelineVertexInputState.CreateInfo
        {
            VertexBindingDescriptions   = [bindingDescription],
            VertexAttributeDescriptions = attributeDescriptions
        };

        var inputAssembly = new PipelineInputAssemblyState.CreateInfo
        {
            Topology               = PrimitiveTopology.TriangleList,
            PrimitiveRestartEnable = false
        };

        var viewportState = new PipelineViewportState.CreateInfo
        {
            ViewportCount = 1,
            ScissorCount  = 1
        };

        var rasterizer = new PipelineRasterizationState.CreateInfo
        {
            DepthClampEnable        = false,
            RasterizerDiscardEnable = false,
            PolygonMode             = PolygonMode.Fill,
            LineWidth               = 1,
            CullMode                = CullModeFlags.Back,
            FrontFace               = FrontFace.CounterClockwise,
            DepthBiasEnable         = false,
        };

        var multisampling = new PipelineMultisampleState.CreateInfo
        {
            SampleShadingEnable  = true,
            RasterizationSamples = this.msaaSamples,
            MinSampleShading     = 1,
        };

        var depthStencil = new PipelineDepthStencilState.CreateInfo
        {
            DepthTestEnable       = true,
            DepthWriteEnable      = true,
            DepthCompareOp        = CompareOp.Less,
            DepthBoundsTestEnable = false,
            StencilTestEnable     = false
        };

        var colorBlendAttachment = new PipelineColorBlendAttachmentState
        {
            ColorWriteMask = ColorComponentFlags.R | ColorComponentFlags.G | ColorComponentFlags.B | ColorComponentFlags.A,
            BlendEnable    = false
        };

        var colorBlending = new PipelineColorBlendState.CreateInfo
        {
            LogicOpEnable = false,
            LogicOp       = LogicOp.Copy,
            Attachments   = [colorBlendAttachment]
        };

        var dynamicState = new PipelineDynamicState.CreateInfo
        {
            DynamicStates = [DynamicState.Viewport, DynamicState.Scissor]
        };

        var pipelineLayoutInfo = new PipelineLayout.CreateInfo
        {
            SetLayouts = [this.descriptorSetLayout]
        };

        this.pipelineLayout = this.device.CreatePipelineLayout(pipelineLayoutInfo);

        var pipelineInfo = new GraphicsPipeline.CreateInfo
        {
            Stages             = shaderStages,
            VertexInputState   = vertexInputInfo,
            InputAssemblyState = inputAssembly,
            ViewportState      = viewportState,
            RasterizationState = rasterizer,
            MultisampleState   = multisampling,
            ColorBlendState    = colorBlending,
            DynamicState       = dynamicState,
            Layout             = this.pipelineLayout,
            RenderPass         = this.renderPass,
            Subpass            = 0,
            BasePipelineHandle = default,
            DepthStencilState  = depthStencil,
        };

        this.graphicsPipeline =  this.device.CreateGraphicsPipelines(pipelineInfo);

        fragShaderModule.Dispose();
        vertShaderModule.Dispose();
    }

    private void CreateImage(uint width, uint height, uint mipLevels, SampleCountFlags numSamples, Format format, ImageTiling tiling, ImageUsageFlags usage, MemoryPropertyFlags properties, out Image image, out DeviceMemory imageMemory)
    {
        var createInfo = new Image.CreateInfo
        {
            ArrayLayers = 1,
            Extent      = new()
            {
                Width  = width,
                Height = height,
                Depth  = 1,
            },
            Format        = format,
            ImageType     = ImageType.N2D,
            InitialLayout = ImageLayout.Undefined,
            MipLevels     = mipLevels,
            Samples       = numSamples,
            SharingMode   = SharingMode.Exclusive,
            Tiling        = tiling,
            Usage         = usage,
        };

        image = this.device.CreateImage(createInfo);

        var memRequirements = image.GetMemoryRequirements();

        var allocInfo = new DeviceMemory.AllocateInfo
        {
            AllocationSize  = memRequirements.Size,
            MemoryTypeIndex = this.FindMemoryType(memRequirements.MemoryTypeBits, properties)
        };

        imageMemory = this.device.AllocateMemory(allocInfo);

        image.BindMemory(imageMemory, 0);
    }

    private ImageView CreateImageView(Image image, Format format, ImageAspectFlags aspectFlags, uint mipLevels)
    {
        var createInfo = new ImageView.CreateInfo
        {
            Image            = image,
            ViewType         = ImageViewType.N2D,
            Format           = format,
            SubresourceRange = new()
            {
                AspectMask     = aspectFlags,
                BaseMipLevel   = 0,
                LevelCount     = mipLevels,
                BaseArrayLayer = 0,
                LayerCount     = 1
            }
        };

        return this.device.CreateImageView(createInfo);
    }

    private void CreateImageViews()
    {
        this.swapChainImageViews = new ImageView[this.swapChainImages.Length];

        for (var i = 0; i < this.swapChainImages.Length; i++)
        {
            this.swapChainImageViews[i] = this.CreateImageView(this.swapChainImages[i], this.swapChainImageFormat, ImageAspectFlags.Color, 1);
        }
    }

    private void CreateIndexBuffer()
    {
        var bufferSize = (ulong)(sizeof(int) * this.indices.Count);

        this.CreateBuffer(
            bufferSize,
            BufferUsageFlags.TransferSrc,
            MemoryPropertyFlags.HostVisible | MemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Map(0, 0, [.. this.indices]);
        stagingBufferMemory.Unmap();

        this.CreateBuffer(
            bufferSize,
            BufferUsageFlags.TransferDst | BufferUsageFlags.IndexBuffer,
            MemoryPropertyFlags.DeviceLocal,
            out this.indexBuffer,
            out this.indexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.indexBuffer, bufferSize);

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();
    }

    private void CreateInstance()
    {
        if (this.enableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        var appInfo = new ApplicationInfo
        {
            ApplicationName    = "Hello Triangle",
            ApplicationVersion = new Version(0, 1, 0, 0),
            EngineName         = "No Engine",
            EngineVersion      = new Version(0, 1, 0, 0),
            ApiVersion         = Version.V1_0,
        };



        using var ppEnabledLayerNames = new StringArrayPtr(this.validationLayers.ToArray());



        DebugUtilsMessenger.CreateInfo? debugCreateInfo = null;
        string[]                        enabledLayerNames = [];

        if (this.enableValidationLayers)
        {
            enabledLayerNames = [.. this.validationLayers];
            PopulateDebugMessengerCreateInfo(out debugCreateInfo);
        }

        var createInfo = new Instance.CreateInfo
        {
            ApplicationInfo   = appInfo,
            EnabledExtensions = [.. this.GetRequiredExtensions()],
            EnabledLayers     = enabledLayerNames,
            Next              = debugCreateInfo,
        };

        this.instance = new Instance(createInfo);

        if (this.enableValidationLayers && !this.instance.TryGetExtension(out this.debugUtilsExtension))
        {
            throw new Exception($"Cannot found required extension {DebugUtilsExtension.Name}");
        }

        if (!this.instance.TryGetExtension<SurfaceExtension>(out var vkKhrSurface))
        {
            throw new Exception($"Cannot found required extension {SurfaceExtension.Name}");
        }

        this.surfaceExtension = vkKhrSurface;
    }

    private void CreateLogicalDevice()
    {
        var indices = this.FindQueueFamilies(this.physicalDevice);

        var queueCreateInfos    = new List<DeviceQueue.CreateInfo>();
        var uniqueQueueFamilies = new HashSet<uint>
        {
            indices.GraphicsFamily!.Value,
            indices.PresentFamily!.Value
        };

        foreach (var queueFamily in uniqueQueueFamilies)
        {
            var queueCreateInfo = new DeviceQueue.CreateInfo
            {
                QueueFamilyIndex = queueFamily,
                QueuePriorities  = [1],
            };

            queueCreateInfos.Add(queueCreateInfo);
        }

        var deviceFeatures = new PhysicalDevice.Features
        {
            SamplerAnisotropy = true,
            SampleRateShading = true,
        };

        using var ppEnabledLayerNames = new StringArrayPtr(this.validationLayers.ToArray());

        var createInfo = new Device.CreateInfo
        {
            QueueCreateInfos  = [.. queueCreateInfos],
            EnabledFeatures   = deviceFeatures,
            EnabledExtensions = [.. this.deviceExtensions],
        };

        this.device = this.physicalDevice.CreateDevice(createInfo);

        if (!this.device.TryGetExtension<SwapchainExtension>(out var vkKhrSwapchain))
        {
            throw new Exception($"Cannot found required extension {SwapchainExtension.Name}");
        }

        this.swapchainExtension = vkKhrSwapchain;
        this.graphicsQueue  = this.device.GetQueue(indices.GraphicsFamily.Value, 0);
        this.presentQueue   = this.device.GetQueue(indices.PresentFamily.Value, 0);

    }

    private void CreateRenderPass()
    {
        var colorAttachment = new AttachmentDescription
        {
            Format         = this.swapChainImageFormat,
            Samples        = this.msaaSamples,
            LoadOp         = AttachmentLoadOp.Clear,
            StoreOp        = AttachmentStoreOp.Store,
            StencilLoadOp  = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout  = ImageLayout.Undefined,
            FinalLayout    = ImageLayout.ColorAttachmentOptimal
        };

        var depthAttachment = new AttachmentDescription
        {
            Format         = this.FindDepthFormat(),
            Samples        = this.msaaSamples,
            LoadOp         = AttachmentLoadOp.Clear,
            StoreOp        = AttachmentStoreOp.DontCare,
            StencilLoadOp  = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout  = ImageLayout.Undefined,
            FinalLayout    = ImageLayout.DepthStencilAttachmentOptimal
        };

        var colorAttachmentResolve = new AttachmentDescription
        {
            Format         = this.swapChainImageFormat,
            Samples        = SampleCountFlags.N1,
            LoadOp         = AttachmentLoadOp.DontCare,
            StoreOp        = AttachmentStoreOp.Store,
            StencilLoadOp  = AttachmentLoadOp.DontCare,
            StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout  = ImageLayout.Undefined,
            FinalLayout    = ImageLayout.PresentSrcKhr
        };

        var colorAttachmentResolveRef = new AttachmentReference
        {
            Attachment = 2,
            Layout     = ImageLayout.ColorAttachmentOptimal
        };

        var colorAttachmentRef = new AttachmentReference
        {
            Attachment = 0,
            Layout     = ImageLayout.ColorAttachmentOptimal
        };

        var depthAttachmentRef = new AttachmentReference
        {
            Attachment = 1,
            Layout     = ImageLayout.DepthStencilAttachmentOptimal
        };

        var subpass = new SubpassDescription
        {
            PipelineBindPoint      = PipelineBindPoint.Graphics,
            ColorAttachments       = [colorAttachmentRef],
            DepthStencilAttachment = depthAttachmentRef,
            ResolveAttachments     = [colorAttachmentResolveRef],
        };

        _ = new SubpassDependency
        {
            SrcSubpass = Constants.VK_SUBPASS_EXTERNAL,
            DstSubpass = 0,
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutput | PipelineStageFlags.EarlyFragmentTests,
            SrcAccessMask = default,
            DstStageMask = PipelineStageFlags.ColorAttachmentOutput | PipelineStageFlags.EarlyFragmentTests,
            DstAccessMask = AccessFlags.ColorAttachmentWrite | AccessFlags.DepthStencilAttachmentWrite
        };

        var attachments = new[]
        {
            colorAttachment,
            depthAttachment,
            colorAttachmentResolve,
        };

        var renderPassInfo = new RenderPass.CreateInfo
        {
            Attachments = attachments,
            Subpasses   = [subpass],
        };

        this.renderPass = this.device.CreateRenderPass(renderPassInfo);
    }

    private ShaderModule CreateShaderModule(byte[] code)
    {
        var createInfo = new ShaderModule.CreateInfo
        {
            Code = Unsafe.As<uint[]>(code),
        };

        return this.device.CreateShaderModule(createInfo);
    }

    private void CreateSurface()
    {
        if (!this.instance.TryGetExtension<Win32SurfaceExtension>(out var vkKhrWin32Surface))
        {
            throw new Exception($"Cannot found required extension {Win32SurfaceExtension.Name}");
        }

        var createInfo = new Win32Surface.CreateInfo
        {
            Hwnd      = this.window.Handle,
            Hinstance = Process.GetCurrentProcess().Handle,
        };

        this.surface = vkKhrWin32Surface.CreateSurface(createInfo);
    }

    private void CreateSwapChain()
    {
        var swapChainSupport = this.QuerySwapChainSupport(this.physicalDevice);
        var surfaceFormat    = ChooseSwapSurfaceFormat(swapChainSupport.Formats);
        var presentMode      = ChooseSwapPresentMode(swapChainSupport.PresentModes);
        var extent           = this.ChooseSwapExtent(swapChainSupport.Capabilities);

        var imageCount = swapChainSupport.Capabilities.MinImageCount + 1;

        if (swapChainSupport.Capabilities.MaxImageCount > 0 && imageCount > swapChainSupport.Capabilities.MaxImageCount)
        {
            imageCount = swapChainSupport.Capabilities.MaxImageCount;
        }

        var indices = this.FindQueueFamilies(this.physicalDevice);

        uint[] queueFamilyIndices;
        SharingMode sharingMode;

        if (indices.GraphicsFamily != indices.PresentFamily)
        {
            sharingMode        = SharingMode.Concurrent;
            queueFamilyIndices =
            [
                indices.GraphicsFamily!.Value,
                indices.PresentFamily!.Value
            ];
        }
        else
        {
            sharingMode        = SharingMode.Exclusive;
            queueFamilyIndices = [];
        }

        var createInfo = new Swapchain.CreateInfo
        {
            Clipped            = true,
            CompositeAlpha     = CompositeAlphaFlags.Opaque,
            ImageArrayLayers   = 1,
            ImageColorSpace    = surfaceFormat.ColorSpace,
            ImageExtent        = extent,
            ImageFormat        = surfaceFormat.Format,
            ImageSharingMode   = sharingMode,
            ImageUsage         = ImageUsageFlags.ColorAttachment,
            MinImageCount      = imageCount,
            OldSwapchain       = default,
            PresentMode        = presentMode,
            PreTransform       = swapChainSupport.Capabilities.CurrentTransform,
            QueueFamilyIndices = queueFamilyIndices,
            Surface            = this.surface,
        };

        this.swapChain       = this.swapchainExtension.CreateSwapchain(createInfo);
        this.swapChainImages = this.swapChain.GetImages();

        this.swapChainImageFormat = surfaceFormat.Format;
        this.swapChainExtent      = extent;
    }

    private void CreateSyncObjects()
    {
        var semaphoreInfo = new Semaphore.CreateInfo();
        var fenceInfo     = new Fence.CreateInfo
        {
            Flags = FenceCreateFlags.Signaled
        };

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.imageAvailableSemaphores[i] = this.device.CreateSemaphore(semaphoreInfo);
            this.renderFinishedSemaphores[i] = this.device.CreateSemaphore(semaphoreInfo);
            this.inFlightFences[i]           = this.device.CreateFence(fenceInfo);
        }
    }

    private void CreateTextureImage()
    {
        using var stream = File.OpenRead(Path.Join(AppContext.BaseDirectory, "Textures/viking_room.png"));
        var bitmap = SKBitmap.Decode(stream);
        var pixels = bitmap.Pixels.Select(x => (uint)x).ToArray();

        this.mipLevels = (uint)Math.Floor(Math.Log2(Math.Max(bitmap.Width, bitmap.Height))) + 1;

        var imageSize = (ulong)(pixels.Length * 4);

        this.CreateBuffer(
            imageSize,
            BufferUsageFlags.TransferSrc,
            MemoryPropertyFlags.HostVisible | MemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Map(0, 0, pixels);
        stagingBufferMemory.Unmap();

        this.CreateImage(
            (uint)bitmap.Width,
            (uint)bitmap.Height,
            this.mipLevels,
            SampleCountFlags.N1,
            Format.R8G8B8A8Srgb,
            ImageTiling.Optimal,
            ImageUsageFlags.TransferSrc | ImageUsageFlags.TransferDst | ImageUsageFlags.Sampled,
            MemoryPropertyFlags.DeviceLocal,
            out this.textureImage,
            out this.textureImageMemory
        );

        this.TransitionImageLayout(this.textureImage, Format.R8G8B8A8Srgb, ImageLayout.Undefined, ImageLayout.TransferDstOptimal, this.mipLevels);
        this.CopyBufferToImage(stagingBuffer, this.textureImage, bitmap.Width, bitmap.Height);
        //transitioned to VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL while generating mipmaps

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();

        this.GenerateMipmaps(this.textureImage, Format.R8G8B8A8Srgb, bitmap.Width, bitmap.Height, this.mipLevels);
    }

    private void CreateTextureSampler()
    {
        var properties = this.physicalDevice.GetProperties();

        var samplerInfo = new Sampler.CreateInfo
        {
            AddressModeU            = SamplerAddressMode.Repeat,
            AddressModeV            = SamplerAddressMode.Repeat,
            AddressModeW            = SamplerAddressMode.Repeat,
            AnisotropyEnable        = true,
            BorderColor             = BorderColor.IntOpaqueBlack,
            CompareEnable           = false,
            CompareOp               = CompareOp.Always,
            MagFilter               = Filter.Linear,
            MaxAnisotropy           = properties.Limits.MaxSamplerAnisotropy,
            MaxLod                  = this.mipLevels,
            MinFilter               = Filter.Linear,
            MinLod                  = 0,
            MipLodBias              = 0,
            MipmapMode              = SamplerMipmapMode.Linear,
            UnnormalizedCoordinates = false,
        };

        this.textureSampler = this.device.CreateSampler(samplerInfo);
    }

    private void CreateTextureImageView() =>
        this.textureImageView = this.CreateImageView(this.textureImage, Format.R8G8B8A8Srgb, ImageAspectFlags.Color, this.mipLevels);

    private void CreateUniformBuffers()
    {
        var bufferSize = (ulong)Marshal.SizeOf<UniformBufferObject>();

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.CreateBuffer(
                bufferSize,
                BufferUsageFlags.UniformBuffer,
                MemoryPropertyFlags.HostVisible | MemoryPropertyFlags.HostCoherent,
                out this.uniformBuffers[i],
                out this.uniformBuffersMemory[i]
            );

            this.uniformBuffersMemory[i].Map(0, 0, this.uniformBuffersMapped[i]);
        }
    }

    private void CreateVertexBuffer()
    {
        var bufferSize = (ulong)(Marshal.SizeOf<Vertex>() * this.vertices.Count);

        this.CreateBuffer(
            bufferSize,
            BufferUsageFlags.TransferSrc,
            MemoryPropertyFlags.HostVisible | MemoryPropertyFlags.HostCoherent,
            out var stagingBuffer,
            out var stagingBufferMemory
        );

        stagingBufferMemory.Map(0, 0, this.vertices.ToArray());
        stagingBufferMemory.Unmap();

        this.CreateBuffer(
            bufferSize,
            BufferUsageFlags.TransferDst | BufferUsageFlags.VertexBuffer,
            MemoryPropertyFlags.DeviceLocal,
            out this.vertexBuffer,
            out this.vertexBufferMemory
        );

        this.CopyBuffer(stagingBuffer, this.vertexBuffer, bufferSize);

        stagingBuffer.Dispose();
        stagingBufferMemory.Dispose();
    }

    private void DrawFrame()
    {
        this.inFlightFences[this.currentFrame].Wait(true, ulong.MaxValue);

        var imageIndex = 0u;

        try
        {
            imageIndex = this.swapChain.AcquireNextImage(ulong.MaxValue, this.imageAvailableSemaphores[this.currentFrame], default);
        }
        catch (VulkanException exception)
        {
            if (exception.Result == Result.ErrorOutOfDateKhr)
            {
                this.RecreateSwapChain();

                return;
            }
            else if (exception.Result != Result.SuboptimalKhr)
            {
                throw new Exception("failed to acquire swap chain image!");
            }
        }

        this.UpdateUniformBuffer(this.currentFrame);

        this.inFlightFences[this.currentFrame].Reset();
        this.commandBuffers[this.currentFrame].Reset();

        this.RecordCommandBuffer(this.commandBuffers[this.currentFrame], imageIndex);

        var waitSemaphores = new[]
        {
            this.imageAvailableSemaphores[this.currentFrame]
        };

        var waitStages = new PipelineStageFlags[]
        {
            PipelineStageFlags.ColorAttachmentOutput
        };

        var commandBuffers = new[]
        {
            this.commandBuffers[this.currentFrame]
        };

        var submitInfo = new SubmitInfo
        {
            CommandBuffers   = commandBuffers,
            SignalSemaphores = [this.renderFinishedSemaphores[this.currentFrame]],
            WaitDstStageMask = waitStages,
            WaitSemaphores   = waitSemaphores,
        };

        this.graphicsQueue.Submit(submitInfo, this.inFlightFences[this.currentFrame]);

        _ = new SubpassDependency
        {
            SrcSubpass = Constants.VK_SUBPASS_EXTERNAL,
            DstSubpass = 0,
            SrcStageMask = PipelineStageFlags.ColorAttachmentOutput,
            SrcAccessMask = default,
            DstStageMask = PipelineStageFlags.ColorAttachmentOutput,
            DstAccessMask = AccessFlags.ColorAttachmentWrite
        };

        var presentInfo = new PresentInfo
        {
            WaitSemaphores = [this.renderFinishedSemaphores[this.currentFrame]],
            Swapchains     = [this.swapChain],
            ImageIndices   = [imageIndex],
        };

        try
        {
            this.swapchainExtension.QueuePresent(this.presentQueue, presentInfo);
        }
        catch (VulkanException exception)
        {
            if (exception.Result is Result.ErrorOutOfDateKhr or Result.SuboptimalKhr || this.framebufferResized)
            {
                this.framebufferResized = false;

                this.RecreateSwapChain();
            }
            else
            {
                throw new Exception("failed to present swap chain image!");
            }
        }


        this.currentFrame = (this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT;
    }

    private void EndSingleTimeCommands(CommandBuffer commandBuffer)
    {
        commandBuffer.End();

        var submitInfo = new SubmitInfo
        {
            CommandBuffers = [commandBuffer]
        };

        this.graphicsQueue.Submit([submitInfo]);
        this.graphicsQueue.WaitIdle();

        commandBuffer.Dispose();
    }

    private SampleCountFlags GetMaxUsableSampleCount()
    {
        var physicalDeviceProperties = this.physicalDevice.GetProperties();

        var counts = physicalDeviceProperties.Limits.FramebufferColorSampleCounts & physicalDeviceProperties.Limits.FramebufferDepthSampleCounts;

        return counts.HasFlag(SampleCountFlags.N64)
            ? SampleCountFlags.N64
            : counts.HasFlag(SampleCountFlags.N32)
                ? SampleCountFlags.N32
                : counts.HasFlag(SampleCountFlags.N16)
                    ? SampleCountFlags.N16
                    : counts.HasFlag(SampleCountFlags.N8)
                        ? SampleCountFlags.N8
                        : counts.HasFlag(SampleCountFlags.N4)
                            ? SampleCountFlags.N4
                            : counts.HasFlag(SampleCountFlags.N2)
                                ? SampleCountFlags.N2
                                : SampleCountFlags.N1;
    }

    private List<string> GetRequiredExtensions()
    {
        var extensions = new List<string>();

        if (this.enableValidationLayers)
        {
            extensions.Add(DebugUtilsExtension.Name);
        }

        extensions.Add(SurfaceExtension.Name);
        extensions.Add(Win32SurfaceExtension.Name);

        return extensions;
    }

    private Format FindDepthFormat() =>
        this.FindSupportedFormat(
            [Format.D32Sfloat, Format.D32SfloatS8Uint, Format.D24UnormS8Uint],
            ImageTiling.Optimal,
            FormatFeatureFlags.DepthStencilAttachment
        );

    private uint FindMemoryType(uint typeFilter, MemoryPropertyFlags properties)
    {
        var memProperties = this.physicalDevice.GetMemoryProperties();

        for (var i = 0u; i < memProperties.MemoryTypes.Length; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && memProperties.MemoryTypes[i].PropertyFlags.HasFlag(properties))
            {
                return i;
            }
        }

        throw new Exception("failed to find suitable memory type!");
    }

    private QueueFamilyIndices FindQueueFamilies(PhysicalDevice physicalDevice)
    {
        var indices = new QueueFamilyIndices();

        var queueFamilies = physicalDevice.GetQueueFamilyProperties();

        var i = 0u;

        foreach (var queueFamily in queueFamilies)
        {
            if (queueFamily.QueueFlags.HasFlag(QueueFlags.Graphics))
            {
                indices.GraphicsFamily = i;
            }

            var presentSupport = this.surfaceExtension.GetPhysicalDeviceSurfaceSupport(physicalDevice, i, this.surface);

            if (presentSupport)
            {
                indices.PresentFamily = i;
            }

            if (indices.IsComplete)
            {
                break;
            }

            i++;
        }

        return indices;
    }

    private Format FindSupportedFormat(Format[] candidates, ImageTiling tiling, FormatFeatureFlags features)
    {
        foreach (var format in candidates)
        {
            var props = this.physicalDevice.GetFormatProperties(format);

            if (tiling == ImageTiling.Linear && props.LinearTilingFeatures.HasFlag(features))
            {
                return format;
            }
            else if (tiling == ImageTiling.Optimal && props.OptimalTilingFeatures.HasFlag(features))
            {
                return format;
            }
        }

        throw new Exception("failed to find supported format!");
    }

    private void GenerateMipmaps(Image image, Format imageFormat, int texWidth, int texHeight, uint mipLevels)
    {
        // Check if image format supports linear blitting
        var formatProperties = this.physicalDevice.GetFormatProperties(imageFormat);

        if (!formatProperties.OptimalTilingFeatures.HasFlag(FormatFeatureFlags.SampledImageFilterLinear))
        {
            throw new Exception("texture image format does not support linear blitting!");
        }

        var commandBuffer = this.BeginSingleTimeCommands();

        static ImageMemoryBarrier clone(
            ImageMemoryBarrier imageMemoryBarrier,
            uint?              baseArrayLayer = default,
            ImageLayout?       oldLayout      = default,
            ImageLayout?       newLayout      = default,
            AccessFlags?       srcAccessMask  = default,
            AccessFlags?       dstAccessMask  = default
        ) =>
            new()
            {
                Image               = imageMemoryBarrier.Image,
                SrcQueueFamilyIndex = imageMemoryBarrier.SrcQueueFamilyIndex,
                DstQueueFamilyIndex = imageMemoryBarrier.DstQueueFamilyIndex,
                SubresourceRange    = new()
                {
                    AspectMask     = imageMemoryBarrier.SubresourceRange!.AspectMask,
                    BaseArrayLayer = baseArrayLayer ?? imageMemoryBarrier.SubresourceRange!.BaseArrayLayer,
                    LayerCount     = imageMemoryBarrier.SubresourceRange!.LayerCount,
                    LevelCount     = imageMemoryBarrier.SubresourceRange!.LevelCount,
                },
                OldLayout     = oldLayout     ?? imageMemoryBarrier.OldLayout,
                NewLayout     = newLayout     ?? imageMemoryBarrier.NewLayout,
                SrcAccessMask = srcAccessMask ?? imageMemoryBarrier.SrcAccessMask,
                DstAccessMask = dstAccessMask ?? imageMemoryBarrier.DstAccessMask,
            };

        var barrier = new ImageMemoryBarrier
        {
            Image               = image,
            SrcQueueFamilyIndex = Constants.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = Constants.VK_QUEUE_FAMILY_IGNORED,
            SubresourceRange    = new()
            {
                AspectMask     = ImageAspectFlags.Color,
                BaseArrayLayer = 0,
                LayerCount     = 1,
                LevelCount     = 1,
            }
        };

        var mipWidth  = texWidth;
        var mipHeight = texHeight;

        for (var i = 1u; i < mipLevels; i++)
        {
            barrier = clone(
                barrier,
                baseArrayLayer: i - 1,
                oldLayout:      ImageLayout.TransferDstOptimal,
                newLayout:      ImageLayout.TransferSrcOptimal,
                srcAccessMask:  AccessFlags.TransferWrite,
                dstAccessMask:  AccessFlags.TransferRead
            );

            commandBuffer.PipelineBarrier(
                PipelineStageFlags.Transfer,
                PipelineStageFlags.Transfer,
                default,
                [],
                [],
                [barrier]
            );

            var blit = new ImageBlit
            {
                SrcSubresource = new()
                {
                    AspectMask     = ImageAspectFlags.Color,
                    MipLevel       = i - 1,
                    BaseArrayLayer = 0,
                    LayerCount     = 1
                },
                SrcOffsets =
                [
                    new(),
                    new() { X = mipWidth, Y = mipHeight, Z = 1 }
                ],
                DstOffsets =
                [
                    new(),
                    new() { X = mipWidth > 1 ? mipWidth / 2 : 1, Y = mipHeight > 1 ? mipHeight / 2 : 1, Z = 1 },
                ],
                DstSubresource = new()
                {
                    AspectMask     = ImageAspectFlags.Color,
                    MipLevel       = i,
                    BaseArrayLayer = 0,
                    LayerCount     = 1
                }
            };

            commandBuffer.BlitImage(
                image,
                ImageLayout.TransferSrcOptimal,
                image,
                ImageLayout.TransferDstOptimal,
                [blit],
                Filter.Linear
            );

            barrier = clone(
                barrier,
                oldLayout:     ImageLayout.TransferSrcOptimal,
                newLayout:     ImageLayout.ShaderReadOnlyOptimal,
                srcAccessMask: AccessFlags.TransferRead,
                dstAccessMask: AccessFlags.ShaderRead
            );

            commandBuffer.PipelineBarrier(
                PipelineStageFlags.Transfer,
                PipelineStageFlags.FragmentShader,
                default,
                [],
                [],
                [barrier]
            );

            if (mipWidth > 1)
            {
                mipWidth /= 2;
            }

            if (mipHeight > 1)
            {
                mipHeight /= 2;
            }
        }

        barrier = clone(
            barrier,
            baseArrayLayer: mipLevels - 1,
            oldLayout:      ImageLayout.TransferDstOptimal,
            newLayout:      ImageLayout.ShaderReadOnlyOptimal,
            srcAccessMask:  AccessFlags.TransferWrite,
            dstAccessMask:  AccessFlags.ShaderRead
        );

        commandBuffer.PipelineBarrier(
            PipelineStageFlags.Transfer,
            PipelineStageFlags.FragmentShader,
            default,
            [],
            [],
            [barrier]
        );

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void InitVulkan()
    {
        this.CreateInstance();
        this.SetupDebugMessenger();
        this.CreateSurface();
        this.PickPhysicalDevice();
        this.CreateLogicalDevice();
        this.CreateSwapChain();
        this.CreateImageViews();
        this.CreateRenderPass();
        this.CreateDescriptorSetLayout();
        this.CreateGraphicsPipeline();
        this.CreateCommandPool();
        this.CreateColorResources();
        this.CreateDepthResources();
        this.CreateFramebuffers();
        this.CreateTextureImage();
        this.CreateTextureImageView();
        this.CreateTextureSampler();
        this.LoadModel();
        this.CreateVertexBuffer();
        this.CreateIndexBuffer();
        this.CreateUniformBuffers();
        this.CreateDescriptorPool();
        this.CreateDescriptorSets();
        this.CreateCommandBuffers();
        this.CreateSyncObjects();
    }

    private void InitWindow()
    {
        this.window = new PlatformWindow("Age*", new(600, 400), new());

        this.window.SizeChanged += () => this.framebufferResized = true;
    }

    private bool IsDeviceSuitable(PhysicalDevice device)
    {
        var indices             = this.FindQueueFamilies(device);
        var extensionsSupported = this.CheckDeviceExtensionSupport(device);
        var swapChainAdequate   = false;

        if (extensionsSupported)
        {
            var swapChainSupport = this.QuerySwapChainSupport(device);

            swapChainAdequate = swapChainSupport.Formats.Length != 0 && swapChainSupport.PresentModes.Length != 0;
        }

        var supportedFeatures = this.physicalDevice.GetDeviceFeatures();

        return indices.IsComplete && extensionsSupported && swapChainAdequate && supportedFeatures.SamplerAnisotropy;
    }

    private void LoadModel()
    {
        var data = this.wavefrontLoader.Load(Path.Join(AppContext.BaseDirectory, "Models", "viking_room.obj"));

        var uniqueVertices = new Dictionary<Vertex, uint>();

        foreach (var obj in data.Objects)
        {
            foreach (var item in obj.Mesh.Faces.SelectMany(x => x.Indices))
            {
                var pos      = data.Attributes.Vertices[item.Index];
                var color    = item.ColorIndex > -1 ? data.Attributes.Colors[item.ColorIndex] : new(1, 0, 0);
                var texCoord = data.Attributes.TexCoords[item.TexCoordIndex];

                texCoord.Y = 1 - texCoord.Y;

                var vertex = new Vertex(pos, color, texCoord);

                if (!uniqueVertices.TryGetValue(vertex, out var index))
                {
                    uniqueVertices[vertex] = index = (uint)this.vertices.Count;
                    this.vertices.Add(vertex);
                }

                this.indices.Add(index);
            }
        }
    }

    private void MainLoop()
    {
        this.window.Show();

        do
        {
            this.window.DoEvents();

            if (!this.window.Closed)
            {
                this.DrawFrame();
            }
        }
        while (!this.window.Closed);

        this.device.WaitIdle();
    }

    private void PickPhysicalDevice()
    {
        var physicalDevices = this.instance.EnumeratePhysicalDevices();

        foreach (var physicalDevice in physicalDevices)
        {
            if (this.IsDeviceSuitable(physicalDevice))
            {
                this.physicalDevice = physicalDevice;
                this.msaaSamples    = this.GetMaxUsableSampleCount();

                break;
            }
        }

        if (this.physicalDevice == default)
        {
            throw new Exception("failed to find a suitable GPU!");
        }
    }

    private SwapChainSupportDetails QuerySwapChainSupport(PhysicalDevice physicalDevice)
    {
        var capabilities = this.surface.GetCapabilities(physicalDevice);
        var formats      = this.surface.GetFormats(physicalDevice);
        var presentModes = this.surface.GetPresentModes(physicalDevice);

        return new SwapChainSupportDetails
        {
            Capabilities = capabilities,
            Formats      = formats,
            PresentModes = presentModes
        };
    }

    private void RecordCommandBuffer(CommandBuffer commandBuffer, uint imageIndex)
    {
        commandBuffer.Begin();

        var renderPassInfo = new RenderPass.BeginInfo
        {
            RenderPass  = this.renderPass,
            Framebuffer = this.swapChainFramebuffers[imageIndex],
            RenderArea  = new()
            {
                Offset = new()
                {
                    X = 0,
                    Y = 0
                },
                Extent = this.swapChainExtent,
            },
            ClearValues =
            [
                new(),
                new()
                {
                    DepthStencil = new()
                    {
                        Depth = 1,
                    }
                }
            ]
        };

        commandBuffer.BeginRenderPass(renderPassInfo, SubpassContents.Inline);

        #region RenderPass
        commandBuffer.BindPipeline(PipelineBindPoint.Graphics, this.graphicsPipeline);

        var viewport = new Viewport
        {
            X        = 0,
            Y        = 0,
            Width    = this.swapChainExtent.Width,
            Height   = this.swapChainExtent.Height,
            MinDepth = 0,
            MaxDepth = 1
        };

        commandBuffer.SetViewport(0, viewport);

        var scissor = new Rect2D
        {
            Offset = new()
            {
                X = 0,
                Y = 0
            },
            Extent = this.swapChainExtent
        };

        commandBuffer.SetScissor(0, scissor);

        var vertexBuffers = new[]
        {
            this.vertexBuffer
        };

        var offsets = new ulong[] { 0 };

        commandBuffer.BindVertexBuffers(0, vertexBuffers, offsets);
        commandBuffer.BindIndexBuffer(this.indexBuffer, 0, IndexType.Uint32);
        commandBuffer.BindDescriptorSets(PipelineBindPoint.Graphics, this.pipelineLayout, 0, [this.descriptorSets[this.currentFrame]], []);
        commandBuffer.DrawIndexed((uint)this.indices.Count, 1, 0, 0, 0);
        #endregion RenderPass

        commandBuffer.EndRenderPass(commandBuffer);

        commandBuffer.End();
    }

    private void RecreateSwapChain()
    {
        while (this.window.Minimized)
        {
            this.window.DoEvents();
        }

        this.device.WaitIdle();

        this.CleanupSwapChain();

        this.CreateSwapChain();
        this.CreateImageViews();
        this.CreateColorResources();
        this.CreateDepthResources();
        this.CreateFramebuffers();
    }

    private void SetupDebugMessenger()
    {
        if (!this.enableValidationLayers)
        {
            return;
        }

        PopulateDebugMessengerCreateInfo(out var createInfo);

        this.debugMessenger = this.debugUtilsExtension!.CreateDebugUtilsMessenger(createInfo);
    }

    private void TransitionImageLayout(Image image, Format _, ImageLayout oldLayout, ImageLayout newLayout, uint mipLevels)
    {
        var commandBuffer = this.BeginSingleTimeCommands();



        PipelineStageFlags sourceStage;
        PipelineStageFlags destinationStage;
        AccessFlags        srcAccessMask;
        AccessFlags        dstAccessMask;

        if (oldLayout == ImageLayout.Undefined && newLayout ==  ImageLayout.TransferDstOptimal)
        {
            srcAccessMask = default;
            dstAccessMask = AccessFlags.TransferWrite;

            sourceStage      = PipelineStageFlags.TopOfPipe;
            destinationStage = PipelineStageFlags.Transfer;
        }
        else if (oldLayout ==  ImageLayout.TransferDstOptimal && newLayout ==  ImageLayout.ShaderReadOnlyOptimal)
        {
            srcAccessMask = AccessFlags.TransferWrite;
            dstAccessMask = AccessFlags.ShaderRead;

            sourceStage      = PipelineStageFlags.Transfer;
            destinationStage = PipelineStageFlags.FragmentShader;
        }
        else
        {
            throw new Exception("unsupported layout transition!");
        }

        var barrier = new ImageMemoryBarrier
        {
            OldLayout           = oldLayout,
            NewLayout           = newLayout,
            SrcQueueFamilyIndex = Constants.VK_QUEUE_FAMILY_IGNORED,
            DstQueueFamilyIndex = Constants.VK_QUEUE_FAMILY_IGNORED,
            Image               = image,
            SrcAccessMask       = srcAccessMask,
            DstAccessMask       = dstAccessMask,
            SubresourceRange    = new()
            {
                AspectMask     = ImageAspectFlags.Color,
                BaseMipLevel   = 0,
                LevelCount     = mipLevels,
                BaseArrayLayer = 0,
                LayerCount     = 1,
            }
        };

        commandBuffer.PipelineBarrier(
            sourceStage,
            destinationStage,
            default,
            [],
            [],
            [barrier]
        );

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void UpdateUniformBuffer(uint currentImage)
    {
        const double RADIANS = 0.017453292519943295;

        var now = DateTime.UtcNow;

        var time = Math.Max(0, (float)(now - startTime).TotalMilliseconds / 1000);

        var ubo = new UniformBufferObject
        {
            Model = Matrix4x4<float>.Rotate(new(0, 0, 1), time * (float)(90 * RADIANS)),
            View  = Matrix4x4<float>.LookAt(new(2), new(0), new(0, 0, 1)),
            Proj  = Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), this.swapChainExtent.Width / (float)this.swapChainExtent.Height, 0.1f, 10)
        };

        ubo.Proj[1, 1] *= -1;

        this.uniformBuffersMapped[currentImage] = ubo;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        this.InitWindow();
        this.InitVulkan();
        this.MainLoop();
        this.Cleanup();
    }
}
#endif
