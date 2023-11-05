using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Age.Core;
using Age.Core.Unsafe;
using Age.Numerics;
using Age.Rendering.Enums;
using Age.Rendering.Vulkan.Handlers;
using Age.Rendering.Vulkan.Uniforms;
using Age.Vulkan.Native;
using Age.Vulkan.Native.Enums;
using Age.Vulkan.Native.Extensions.EXT;
using Age.Vulkan.Native.Extensions.KHR;
using Age.Vulkan.Native.Flags;
using Age.Vulkan.Native.Flags.EXT;
using Age.Vulkan.Native.Flags.KHR;
using Age.Vulkan.Native.Types;
using Age.Vulkan.Native.Types.EXT;
using Age.Vulkan.Native.Types.KHR;

using static Age.Core.Unsafe.UnmanagedUtils;

namespace Age.Rendering.Vulkan;

public abstract unsafe partial class VulkanRenderer : IDisposable
{
    private const ushort MAX_DESCRIPTORS_PER_POOL = 64;
    private const ushort MAX_FRAMES_IN_FLIGHT     = 2;

    private static readonly HashSet<string> validationLayers = [Vk.VK_LAYER_KHRONOS_VALIDATION];

    private readonly VkExtDebugUtilsExtension?                                 debugUtilsExtension;
    private readonly VkDebugUtilsMessengerEXT                                  debugUtilsMessenger;
    private readonly Dictionary<VkDescriptorType, List<DescriptorPoolHandler>> descriptorPools = [];
    private readonly VkFence[]                                                 fences                      = new VkFence[MAX_FRAMES_IN_FLIGHT];
    private readonly VkSemaphore[]                                             renderingFinishedSemaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];
    private readonly VkKhrSurfaceExtension                                     surfaceKhrExtension;
    private readonly List<WindowHandler>                                       windows                     = [];

    protected readonly VkInstance Instance;

    private WindowHandler?           activeWindow;
    private VkCommandBuffer[]        commandBuffers = [];
    private VkCommandPool            commandPool;
    private ushort                   currentFrame;
    private VkDevice                 device;
    private bool                     deviceInitialized;
    private bool                     disposed;
    private ulong                    frames;
    private VkQueue                  graphicsQueue;
    private uint                     graphicsQueueIndex;
    private VkPhysicalDevice         physicalDevice;
    private VkQueue                  presentationQueue;
    private uint                     presentationQueueIndex;
    private VkKhrSwapchainExtension? swapchainKhrExtension;

#pragma warning disable CA1822
    [MemberNotNullWhen(true, nameof(debugUtilsExtension))]
    private bool EnableValidationLayers => Debugger.IsAttached;
#pragma warning restore CA1822

    private IList<string> RequiredExtensions
    {
        get
        {
            var extensions = new List<string>
            {
                VkKhrSurfaceExtension.Name,
            };

            extensions.AddRange(this.PlatformExtensions);

            if (this.EnableValidationLayers)
            {
                extensions.Add(VkExtDebugUtilsExtension.Name);
            }

            return extensions;
        }
    }

    protected abstract string[] PlatformExtensions { get; }

    protected Vk Vk { get; }

    public unsafe VulkanRenderer(Vk vk)
    {
        this.Vk = vk;

        this.CreateInstance(out this.Instance, out this.surfaceKhrExtension, out this.debugUtilsExtension, out this.debugUtilsMessenger);
    }

    private static VkDescriptorType ConvertToDescriptorType(UniformType type) =>
        type switch
        {
            UniformType.CombinedImageSampler => VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            UniformType.UniformBuffer        => VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
            _ => throw new NotImplementedException(),
        };

    private static VkImageType ConvertToImageType(TextureType type) =>
        type switch
        {
            TextureType.T1D or TextureType.T1DArray                                              => VkImageType.VK_IMAGE_TYPE_1D,
            TextureType.T2D or TextureType.T2DArray or TextureType.Cube or TextureType.CubeArray => VkImageType.VK_IMAGE_TYPE_2D,
            TextureType.T3D                                                                      => VkImageType.VK_IMAGE_TYPE_3D,
            _ => throw new NotImplementedException(),
        };

    private static VkImageViewType ConvertToImageViewType(TextureType type) =>
        type switch
        {
            TextureType.T1D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_1D,
            TextureType.T1DArray  => VkImageViewType.VK_IMAGE_VIEW_TYPE_1D_ARRAY,
            TextureType.T2D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
            TextureType.T2DArray  => VkImageViewType.VK_IMAGE_VIEW_TYPE_2D_ARRAY,
            TextureType.T3D       => VkImageViewType.VK_IMAGE_VIEW_TYPE_3D,
            TextureType.Cube      => VkImageViewType.VK_IMAGE_VIEW_TYPE_CUBE,
            TextureType.CubeArray => VkImageViewType.VK_IMAGE_VIEW_TYPE_CUBE_ARRAY,
            _ => throw new NotImplementedException(),
        };

    private unsafe static VkBool32 DebugCallback(VkDebugUtilsMessageSeverityFlagBitsEXT messageSeverity, VkDebugUtilsMessageTypeFlagsEXT messageTypes, VkDebugUtilsMessengerCallbackDataEXT* pCallbackData, void* pUserData)
    {
        var defaultColor = Console.ForegroundColor;

        var color = messageSeverity switch
        {
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT => ConsoleColor.DarkRed,
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT => ConsoleColor.DarkYellow,
            VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT => ConsoleColor.DarkBlue,
            _ => defaultColor
        };

        Console.ForegroundColor = color;

        Console.WriteLine("validation layer: " + Marshal.PtrToStringAnsi((nint)pCallbackData->pMessage));

        Console.ForegroundColor = defaultColor;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void VkCheck(in VkResult result)
    {
        if (result != VkResult.VK_SUCCESS)
        {
            throw new Exception($"Vulkan Error: {result}");
        }
    }

    private VkCommandBuffer BeginSingleTimeCommands()
    {
        var allocInfo = new VkCommandBufferAllocateInfo
        {
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool        = this.commandPool,
            commandBufferCount = 1
        };

        VkCheck(this.Vk.AllocateCommandBuffers(this.device, allocInfo, out VkCommandBuffer commandBuffer));

        var beginInfo = new VkCommandBufferBeginInfo
        {
            flags = VkCommandBufferUsageFlagBits.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT
        };

        VkCheck(this.Vk.BeginCommandBuffer(commandBuffer, beginInfo));

        return commandBuffer;
    }

    [MemberNotNull(nameof(activeWindow))]
    private void CheckActiveWindow()
    {
        if (this.activeWindow == null)
        {
            throw new Exception("active window can't be null");
        }
    }

    [MemberNotNull(nameof(swapchainKhrExtension))]
    private void CheckDevice()
    {
        if (this.swapchainKhrExtension == null)
        {
            throw new Exception("device not initialized");
        }
    }

    private bool CheckValidationLayerSupport()
    {
        this.Vk.EnumerateInstanceLayerProperties(out VkLayerProperties[] properties);

        return validationLayers.Overlaps(properties.Select(x => Marshal.PtrToStringAnsi((nint)x.layerName)!));
    }

    private void CreateBuffer(VkDeviceSize size, VkBufferUsageFlagBits usage, VkMemoryPropertyFlagBits properties, out VkBuffer buffer, out Allocation allocation)
    {
        var bufferCreateInfo = new VkBufferCreateInfo
        {
            size  = size,
            usage = usage,
        };

        VkCheck(this.Vk.CreateBuffer(this.device, bufferCreateInfo, default, out buffer));

        this.Vk.GetBufferMemoryRequirements(this.device, buffer, out var memRequirements);

        var memoryType = this.FindMemoryType(memRequirements.memoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            allocationSize  = memRequirements.size,
            memoryTypeIndex = this.FindMemoryType(memRequirements.memoryTypeBits, properties)
        };

        VkCheck(this.Vk.AllocateMemory(this.device, memoryAllocateInfo, default, out var memory));
        VkCheck(this.Vk.BindBufferMemory(this.device, buffer, memory, 0));

        allocation = new()
        {
            Memory     = memory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = size,
        };
    }

    private void CreateCommandBuffers(out VkCommandBuffer[] commandBuffers)
    {
        var commandBufferAllocateInfo = new VkCommandBufferAllocateInfo
        {
            commandPool        = this.commandPool,
            commandBufferCount = MAX_FRAMES_IN_FLIGHT,
            level              = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY
        };

        VkCheck(this.Vk.AllocateCommandBuffers(this.device, commandBufferAllocateInfo, out commandBuffers));
    }

    private void CreateCommandPool(out VkCommandPool commandPool)
    {
        var createInfo = new VkCommandPoolCreateInfo
        {
            flags            = VkCommandPoolCreateFlagBits.VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT,
            queueFamilyIndex = this.graphicsQueueIndex,
        };

        VkCheck(this.Vk.CreateCommandPool(this.device, createInfo, default, out commandPool));
    }

    private VkDescriptorSet[] CreateDescriptorSets(VkDescriptorPool descriptorPool, VkDescriptorSetLayout descriptorSetLayout)
    {
        var descriptorSetLayouts = new VkDescriptorSetLayout[MAX_FRAMES_IN_FLIGHT]
        {
            descriptorSetLayout,
            descriptorSetLayout,
        };

        fixed (VkDescriptorSetLayout* pSetLayouts = descriptorSetLayouts)
        {
            var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
            {
                descriptorPool     = descriptorPool,
                descriptorSetCount = MAX_FRAMES_IN_FLIGHT,
                pSetLayouts        = pSetLayouts,
            };

            VkCheck(this.Vk.AllocateDescriptorSets(this.device, descriptorSetAllocateInfo, out var descriptorSets));

            return descriptorSets;
        }
    }

    private VkDescriptorPool CreateDescriptorPool()
    {
        var descriptorPoolSize = new VkDescriptorPoolSize[]
        {
            new()
            {
                type            = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                descriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
            new()
            {
                type            = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                descriptorCount = MAX_FRAMES_IN_FLIGHT,
            },
        };

        fixed (VkDescriptorPoolSize* pPoolSizes = descriptorPoolSize)
        {
            var descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                maxSets       = MAX_FRAMES_IN_FLIGHT,
                poolSizeCount = (uint)descriptorPoolSize.Length,
                pPoolSizes    = pPoolSizes,
            };

            VkCheck(this.Vk.CreateDescriptorPool(this.device, descriptorPoolCreateInfo, default, out var descriptorPool));

            return descriptorPool;
        }
    }

    private DescriptorPoolHandler CreateDescriptorPool(VkDescriptorType descriptorType)
    {
        if (this.descriptorPools.TryGetValue(descriptorType, out var descriptorPools))
        {
            if (descriptorPools.FirstOrDefault(x => x.Usage < MAX_DESCRIPTORS_PER_POOL) is DescriptorPoolHandler descriptorPool)
            {
                descriptorPool.Usage++;

                return descriptorPool;
            }
        }
        else
        {
            this.descriptorPools[descriptorType] = descriptorPools = [];
        }

        var sizes = new List<VkDescriptorPoolSize>();

        if (descriptorType.HasFlag(VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER))
        {
            sizes.Add(new() { type = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER, descriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        if (descriptorType.HasFlag(VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER))
        {
            sizes.Add(new() { type = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER, descriptorCount = MAX_DESCRIPTORS_PER_POOL });
        }

        fixed (VkDescriptorPoolSize* pPoolSizes = sizes.ToArray())
        {
            var descriptorPoolCreateInfo = new VkDescriptorPoolCreateInfo
            {
                maxSets       = MAX_DESCRIPTORS_PER_POOL,
                poolSizeCount = (uint)sizes.Count,
                pPoolSizes    = pPoolSizes,
            };

            VkCheck(this.Vk.CreateDescriptorPool(this.device, descriptorPoolCreateInfo, default, out var descriptorPool));

            var descriptorPoolHandler = new DescriptorPoolHandler
            {
                DescriptorType = descriptorType,
                Handler        = descriptorPool,
                Usage          = 1,
            };

            descriptorPools.Add(descriptorPoolHandler);

            return descriptorPoolHandler;
        }
    }

    private VkDescriptorSetLayout CreateDescriptorSetLayout()
    {
        var descriptorSetLayoutBindings = new VkDescriptorSetLayoutBinding[]
        {
            new()
            {
                binding         = 0,
                descriptorCount = 1,
                descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                stageFlags      = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT,
            },
        };

        fixed (VkDescriptorSetLayoutBinding* pBindings = descriptorSetLayoutBindings)
        {
            var descriptorSetLayoutCreateInfo = new VkDescriptorSetLayoutCreateInfo
            {
                pBindings    = pBindings,
                bindingCount = (uint)descriptorSetLayoutBindings.Length,
            };

            VkCheck(this.Vk.CreateDescriptorSetLayout(this.device, descriptorSetLayoutCreateInfo, default, out var descriptorSetLayout));

            return descriptorSetLayout;
        }
    }

    private void CreateDevice(out VkDevice device, out VkKhrSwapchainExtension swapchainKhrExtension, out VkQueue graphicsQueue, out VkQueue presentationQueue)
    {
        var queuePriorities  = 1f;
        var pQueuePriorities = &queuePriorities;

        var queueCreateInfos = new HashSet<uint> { this.graphicsQueueIndex, this.presentationQueueIndex }
            .Select(
                x => new VkDeviceQueueCreateInfo
                {
                    queueFamilyIndex = this.graphicsQueueIndex,
                    queueCount       = 1,
                    pQueuePriorities = pQueuePriorities,
                }
            )
            .ToArray();

        using var ppEnabledExtensionNames = new StringArrayPtr([VkKhrSwapchainExtension.Name]);

        fixed (VkDeviceQueueCreateInfo* pQueueCreateInfos = queueCreateInfos)
        {
            var deviceCreateInfo = new VkDeviceCreateInfo
            {
                enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                ppEnabledExtensionNames = ppEnabledExtensionNames,
                pQueueCreateInfos       = pQueueCreateInfos,
                queueCreateInfoCount    = (uint)queueCreateInfos.Length,
            };

            VkCheck(this.Vk.CreateDevice(this.physicalDevice, deviceCreateInfo, default, out device));

            if (!this.Vk.TryGetDeviceExtension(this.physicalDevice, device, out swapchainKhrExtension!))
            {
                throw new Exception($"Cannot found required extension {VkKhrSwapchainExtension.Name}");
            }

            this.Vk.GetDeviceQueue(device, this.graphicsQueueIndex, 0, out graphicsQueue);
            this.Vk.GetDeviceQueue(device, this.presentationQueueIndex, 0, out presentationQueue);
        }
    }

    private void CreateInstance(out VkInstance instance, out VkKhrSurfaceExtension surfaceKhrExtension, out VkExtDebugUtilsExtension? debugUtilsExtension, out VkDebugUtilsMessengerEXT debugUtilsMessenger)
    {
        if (this.EnableValidationLayers && !this.CheckValidationLayerSupport())
        {
            throw new Exception("validation layers requested, but not available!");
        }

        fixed (byte* pName = "Age"u8)
        {
            var applicationInfo = new VkApplicationInfo
            {
                apiVersion         = Vk.ApiVersion_1_0,
                applicationVersion = Vk.MakeApiVersion(0, 0, 1, 0),
                engineVersion      = Vk.MakeApiVersion(0, 0, 1, 0),
                pApplicationName   = pName,
                pEngineName        = pName,
            };

            var debugUtilsMessengerCreateInfo = this.EnableValidationLayers
                ?  new VkDebugUtilsMessengerCreateInfoEXT
                {
                    messageType = VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_DEVICE_ADDRESS_BINDING_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT
                        | VkDebugUtilsMessageTypeFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT,
                    messageSeverity = VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT
                        | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT
                        | VkDebugUtilsMessageSeverityFlagBitsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT,
                    pfnUserCallback = new(DebugCallback),
                }
                : default;

            using var ppEnabledLayerNames     = new StringArrayPtr(validationLayers.ToArray());
            using var ppEnabledExtensionNames = new StringArrayPtr(this.RequiredExtensions);

            var instanceCreateInfo = new VkInstanceCreateInfo
            {
                enabledExtensionCount   = (uint)ppEnabledExtensionNames.Length,
                enabledLayerCount       = (uint)ppEnabledLayerNames.Length,
                pApplicationInfo        = &applicationInfo,
                ppEnabledExtensionNames = ppEnabledExtensionNames,
                ppEnabledLayerNames     = ppEnabledLayerNames,
                pNext                   = NullIfDefault(debugUtilsMessengerCreateInfo, &debugUtilsMessengerCreateInfo),
            };

            VkCheck(this.Vk.CreateInstance(instanceCreateInfo, default, out instance));
            surfaceKhrExtension = this.Vk.GetInstanceExtension<VkKhrSurfaceExtension>(instance);

            debugUtilsExtension = default;
            debugUtilsMessenger = default;

            if (this.EnableValidationLayers)
            {
                debugUtilsExtension = this.Vk.GetInstanceExtension<VkExtDebugUtilsExtension>(instance);

                VkCheck(debugUtilsExtension.CreateDebugUtilsMessenger(instance, debugUtilsMessengerCreateInfo, default, out debugUtilsMessenger));
            }
        }
    }

    private void CreateImage(uint width, uint height, VkSampleCountFlagBits samples, VkFormat format, VkImageTiling tiling, VkImageUsageFlagBits usage, VkMemoryPropertyFlagBits properties, out VkImage image, out Allocation allocation)
    {
        var imageCreateInfo = new VkImageCreateInfo
        {
            arrayLayers = 1,
            extent      = new()
            {
                depth  = 1,
                height = height,
                width  = width,
            },
            format        = format,
            imageType     = VkImageType.VK_IMAGE_TYPE_2D,
            initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            mipLevels     = 1,
            samples       = samples,
            tiling        = tiling,
            usage         = usage,
        };

        VkCheck(this.Vk.CreateImage(this.device, imageCreateInfo, default, out image));

        this.Vk.GetImageMemoryRequirements(this.device, image, out var memRequirements);

        var memoryType = this.FindMemoryType(memRequirements.memoryTypeBits, properties);

        var memoryAllocateInfo = new VkMemoryAllocateInfo
        {
            allocationSize  = memRequirements.size,
            memoryTypeIndex = memoryType,
        };

        VkCheck(this.Vk.AllocateMemory(this.device, memoryAllocateInfo, default, out var deviceMemory));
        VkCheck(this.Vk.BindImageMemory(this.device, image, deviceMemory, 0));

        allocation = new()
        {
            Memory     = deviceMemory,
            Memorytype = memoryType,
            Offset     = 0,
            Size       = memRequirements.size,
        };
    }

    private VkImageView CreateImageView(VkImage image, VkFormat format, VkImageAspectFlagBits aspect)
    {
        var imageViewCreateInfo = new VkImageViewCreateInfo
        {
            format           = format,
            image            = image,
            subresourceRange = new()
            {
                aspectMask = aspect,
                layerCount = 1,
                levelCount = 1,
            },
            viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
        };

        VkCheck(this.Vk.CreateImageView(this.device, imageViewCreateInfo, default, out var imageView));

        return imageView;
    }

    private VkPipeline CreatePipeline(VkPipelineLayout pipelineLayout)
    {
        this.CheckActiveWindow();

        fixed (byte* pName = "main"u8)
        {
            var vertShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/shader.vert.spv"))!;
            var fragShaderCode = File.ReadAllBytes(Path.Join(AppContext.BaseDirectory, "Shaders/shader.frag.spv"))!;

            var vertShaderModule = this.CreateShaderModule(vertShaderCode);
            var fragShaderModule = this.CreateShaderModule(fragShaderCode);

            var pipelineShaderStageCreateInfos = new VkPipelineShaderStageCreateInfo[]
            {
                new()
                {
                    module = vertShaderModule,
                    pName  = pName,
                    stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_VERTEX_BIT,
                },
                new()
                {
                    module = fragShaderModule,
                    pName  = pName,
                    stage  = VkShaderStageFlagBits.VK_SHADER_STAGE_FRAGMENT_BIT,
                },
            };

            var vertexInputBindingDescription = new VkVertexInputBindingDescription
            {
                binding   = 0,
                stride    = (uint)Marshal.SizeOf<Vertex>(),
                inputRate = VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX,
            };

            var vertexInputAttributeDescription = new VkVertexInputAttributeDescription[]
            {
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    location = 0,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)),
                },
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT,
                    location = 1,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.Color)),
                },
                new()
                {
                    binding  = 0,
                    format   = VkFormat.VK_FORMAT_R32G32_SFLOAT,
                    location = 2,
                    offset   = (uint)Marshal.OffsetOf<Vertex>(nameof(Vertex.UV)),
                },
            };

            var dynamicStates = new VkDynamicState[]
            {
                VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                VkDynamicState.VK_DYNAMIC_STATE_SCISSOR,
            };

            fixed (VkVertexInputAttributeDescription* pVertexAttributeDescriptions    = vertexInputAttributeDescription)
            fixed (VkDynamicState*                    pDynamicStates                  = dynamicStates)
            fixed (VkPipelineShaderStageCreateInfo*   pPipelineShaderStageCreateInfos = pipelineShaderStageCreateInfos)
            {
                var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
                {
                    topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                };

                var pipelineVertexInputStateCreateInfo = new VkPipelineVertexInputStateCreateInfo
                {
                    pVertexAttributeDescriptions    = pVertexAttributeDescriptions,
                    pVertexBindingDescriptions      = &vertexInputBindingDescription,
                    vertexAttributeDescriptionCount = (uint)vertexInputAttributeDescription.Length,
                    vertexBindingDescriptionCount   = 1,
                };

                var pipelineDynamicStateCreateInfo = new VkPipelineDynamicStateCreateInfo
                {
                    dynamicStateCount = (uint)dynamicStates.Length,
                    pDynamicStates    = pDynamicStates,
                };

                var pipelineColorBlendAttachmentState = new VkPipelineColorBlendAttachmentState
                {
                    blendEnable    = true,
                    colorWriteMask = VkColorComponentFlagBits.VK_COLOR_COMPONENT_R_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_G_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_B_BIT
                        | VkColorComponentFlagBits.VK_COLOR_COMPONENT_A_BIT,
                    dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
                    srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                    srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_SRC_ALPHA,
                };

                var pipelineColorBlendStateCreateInfo = new VkPipelineColorBlendStateCreateInfo
                {
                    attachmentCount = 1,
                    pAttachments    = &pipelineColorBlendAttachmentState,
                };

                var pipelineMultisampleStateCreateInfo = new VkPipelineMultisampleStateCreateInfo
                {
                    rasterizationSamples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
                };

                var pipelineRasterizationStateCreateInfo = new VkPipelineRasterizationStateCreateInfo
                {
                    cullMode    = VkCullModeFlagBits.VK_CULL_MODE_BACK_BIT,
                    frontFace   = VkFrontFace.VK_FRONT_FACE_CLOCKWISE,
                    lineWidth   = 1,
                    polygonMode = VkPolygonMode.VK_POLYGON_MODE_FILL,
                };

                var pipelineViewportStateCreateInfo = new VkPipelineViewportStateCreateInfo
                {
                    viewportCount = 1,
                    scissorCount  = 1,
                };

                var graphicsPipelineCreateInfo = new VkGraphicsPipelineCreateInfo
                {
                    layout              = pipelineLayout,
                    pColorBlendState    = &pipelineColorBlendStateCreateInfo,
                    pDynamicState       = &pipelineDynamicStateCreateInfo,
                    pInputAssemblyState = &inputAssembly,
                    pMultisampleState   = &pipelineMultisampleStateCreateInfo,
                    pRasterizationState = &pipelineRasterizationStateCreateInfo,
                    pStages             = pPipelineShaderStageCreateInfos,
                    pVertexInputState   = &pipelineVertexInputStateCreateInfo,
                    pViewportState      = &pipelineViewportStateCreateInfo,
                    stageCount          = (uint)pipelineShaderStageCreateInfos.Length,
                    renderPass          = this.activeWindow.Swapchain.RenderPass,
                };

                VkCheck(this.Vk.CreateGraphicsPipelines(this.device, default, graphicsPipelineCreateInfo, default, out var pipeline));

                this.Vk.DestroyShaderModule(this.device, fragShaderModule, null);
                this.Vk.DestroyShaderModule(this.device, vertShaderModule, null);

                return pipeline;
            }
        }
    }

    private VkPipelineLayout CreatePipelineLayout(VkDescriptorSetLayout descriptorSetLayout)
    {
        var pipelineLayoutCreateInfo = new VkPipelineLayoutCreateInfo
        {
            pSetLayouts    = &descriptorSetLayout,
            setLayoutCount = 1,
        };

        VkCheck(this.Vk.CreatePipelineLayout(this.device, pipelineLayoutCreateInfo, default, out var pipelineLayout));

        return pipelineLayout;
    }

    private VkShaderModule CreateShaderModule(byte[] buffer)
    {
        fixed (byte* pCode = buffer)
        {
            var shaderModuleCreateInfo = new VkShaderModuleCreateInfo
            {
                codeSize = (uint)buffer.Length,
                pCode    = (uint*)pCode,
            };

            VkCheck(this.Vk.CreateShaderModule(this.device, shaderModuleCreateInfo, default, out var shaderModule));

            return shaderModule;
        }
    }

    private SwapchainHandler CreateSwapchain(VkSurfaceKHR surface, Size<uint> size)
    {
        this.CheckDevice();

        this.surfaceKhrExtension.GetPhysicalDeviceSurfaceCapabilities(this.physicalDevice, surface, out var surfaceCapabilities);
        this.surfaceKhrExtension.GetPhysicalDeviceSurfaceFormats(this.physicalDevice, surface, out VkSurfaceFormatKHR[] surfaceFormats);

        var extent = surfaceCapabilities.currentExtent;
        var format = surfaceFormats[0].format;

        fixed (uint* pQueueFamilyIndices = &this.presentationQueueIndex)
        {
            var swapchainCreateInfo = new VkSwapchainCreateInfoKHR
            {
                compositeAlpha        = VkCompositeAlphaFlagBitsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
                imageArrayLayers      = 1,
                imageColorSpace       = surfaceFormats[0].colorSpace,
                imageExtent           = extent,
                imageFormat           = format,
                imageUsage            = VkImageUsageFlagBits.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT,
                minImageCount         = surfaceCapabilities.minImageCount,
                pQueueFamilyIndices   = pQueueFamilyIndices,
                preTransform          = VkSurfaceTransformFlagBitsKHR.VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR,
                queueFamilyIndexCount = 1,
                surface               = surface,
            };

            VkCheck(this.swapchainKhrExtension.CreateSwapchain(this.device, swapchainCreateInfo, default, out var swapchain));
            VkCheck(this.swapchainKhrExtension.GetSwapchainImages(this.device, swapchain, out VkImage[] images));

            var imageViews   = new VkImageView[images.Length];
            var framebuffers = new VkFramebuffer[images.Length];

            var renderPass = this.CreateRenderPass(format);

            for (var i = 0; i < images.Length; i++)
            {
                imageViews[i]   = this.CreateImageView(images[i], format, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT);
                framebuffers[i] = this.CreateFrameBuffer(renderPass, imageViews[i], extent);
            }

            return new SwapchainHandler
            {
                Extent       = extent,
                Format       = format,
                Framebuffers = framebuffers,
                Handler      = swapchain,
                Images       = images,
                ImageViews   = imageViews,
                RenderPass   = renderPass,
            };
        }
    }

    private VkFramebuffer CreateFrameBuffer(VkRenderPass renderPass, VkImageView imageView, VkExtent2D extent)
    {
        var framebufferCreateInfo = new VkFramebufferCreateInfo
        {
            attachmentCount = 1,
            height          = extent.height,
            layers          = 1,
            pAttachments    = &imageView,
            width           = extent.width,
            renderPass      = renderPass,
        };

        VkCheck(this.Vk.CreateFramebuffer(this.device, framebufferCreateInfo, default, out var framebuffer));

        return framebuffer;
    }

    private VkRenderPass CreateRenderPass(VkFormat format)
    {
        var attachmentDescription = new VkAttachmentDescription
        {
            samples        = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT,
            finalLayout    = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR,
            format         = format,
            initialLayout  = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            loadOp         = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            stencilLoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
            stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
        };

        var attachmentReference = new VkAttachmentReference
        {
            layout = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
        };

        var subpass = new VkSubpassDescription
        {
            colorAttachmentCount = 1,
            pColorAttachments    = &attachmentReference,
            pipelineBindPoint    = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
        };

        var renderPassCreateInfo = new VkRenderPassCreateInfo
        {
            attachmentCount = 1,
            pAttachments    = &attachmentDescription,
            pSubpasses      = &subpass,
            subpassCount    = 1,
        };

        VkCheck(this.Vk.CreateRenderPass(this.device, renderPassCreateInfo, default, out var renderPass));

        return renderPass;
    }

    private void CreateSyncObjects()
    {
        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            VkCheck(this.Vk.CreateSemaphore(this.device, new VkSemaphoreCreateInfo(), default, out this.renderingFinishedSemaphores[i]));

            var fenceCreateInfo = new VkFenceCreateInfo
            {
                flags = VkFenceCreateFlagBits.VK_FENCE_CREATE_SIGNALED_BIT,
            };

            VkCheck(this.Vk.CreateFence(this.device, fenceCreateInfo, default, out this.fences[i]));
        }
    }

    private void CopyBuffer(VkBuffer srcBuffer, VkBuffer dstBuffer, VkDeviceSize size)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var copyRegion = new VkBufferCopy
        {
            size = size
        };

        this.Vk.CmdCopyBuffer(commandBuffer, srcBuffer, dstBuffer, copyRegion);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void CopyBufferToImage(VkBuffer buffer, VkImage image, uint width, uint height)
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var bufferImageCopy = new VkBufferImageCopy
        {
            imageExtent = new()
            {
                depth  = 1,
                height = height,
                width  = width,
            },
            imageSubresource = new()
            {
                aspectMask = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                layerCount = 1,
            }
        };

        this.Vk.CmdCopyBufferToImage(commandBuffer, buffer, image, VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, bufferImageCopy);

        this.EndSingleTimeCommands(commandBuffer);
    }

    private void DestroySwapchain(SwapchainHandler swapchain)
    {
        this.CheckDevice();

        this.Vk.DeviceWaitIdle(this.device);

        for (var i = 0; i < swapchain.ImageViews.Length; i++)
        {
            this.Vk.DestroyFramebuffer(this.device, swapchain.Framebuffers[i], null);
            this.Vk.DestroyImageView(this.device, swapchain.ImageViews[i], null);
        }

        this.Vk.DestroyRenderPass(this.device, swapchain.RenderPass, default);
        this.swapchainKhrExtension.DestroySwapchain(this.device, swapchain.Handler, null);
    }

    private void EndSingleTimeCommands(VkCommandBuffer commandBuffer)
    {
        VkCheck(this.Vk.EndCommandBuffer(commandBuffer));

        var submitInfo = new VkSubmitInfo
        {
            commandBufferCount = 1,
            pCommandBuffers    = &commandBuffer
        };

        VkCheck(this.Vk.QueueSubmit(this.graphicsQueue, submitInfo, default));
        VkCheck(this.Vk.QueueWaitIdle(this.graphicsQueue));

        this.Vk.FreeCommandBuffers(this.device, this.commandPool, commandBuffer);
    }

    private uint FindMemoryType(uint typeFilter, VkMemoryPropertyFlagBits properties)
    {
        this.Vk.GetPhysicalDeviceMemoryProperties(this.physicalDevice, out var memProperties);

        for (var i = 0u; i < memProperties.memoryTypeCount; i++)
        {
            if ((typeFilter & (1 << (int)i)) != 0 && memProperties.GetMemoryTypes(i).propertyFlags.HasFlag(properties))
            {
                return i;
            }
        }

        throw new Exception("Failed to find suitable memory type");
    }

    private void RemoveFromDescriptorPool(DescriptorPoolHandler descriptorPool)
    {
        descriptorPool.Usage--;

        if (descriptorPool.Usage == 0)
        {
            this.Vk.DestroyDescriptorPool(this.device, descriptorPool.Handler, null);

            var entries = this.descriptorPools[descriptorPool.DescriptorType];

            entries.Remove(descriptorPool);

            if (entries.Count == 0)
            {
                this.descriptorPools.Remove(descriptorPool.DescriptorType);
            }
        }
    }

    private void InitializeDevice(VkSurfaceKHR surface)
    {
        this.PickPhysicalDevice(surface, out this.physicalDevice, out this.graphicsQueueIndex, out this.presentationQueueIndex);
        this.CreateDevice(out this.device, out this.swapchainKhrExtension, out this.graphicsQueue, out this.presentationQueue);
        this.CreateCommandPool(out this.commandPool);
        this.CreateCommandBuffers(out this.commandBuffers);
        this.CreateSyncObjects();
    }

    private void PickPhysicalDevice(VkSurfaceKHR surface, out VkPhysicalDevice physicalDevice, out uint graphicsQueueIndex, out uint presentationQueueIndex)
    {
        this.Vk.EnumeratePhysicalDevices(this.Instance, out VkPhysicalDevice[] physicalDevices);

        var graphicsQueueFounded = -1;
        var presentationQueueFounded = -1;

        foreach (var device in physicalDevices)
        {
            this.Vk.GetPhysicalDeviceFeatures(device, out var supportedFeatures);
            this.Vk.GetPhysicalDeviceQueueFamilyProperties(device, out VkQueueFamilyProperties[] queueFamilyProperties);

            for (var i = 0u; i < queueFamilyProperties.Length; i++)
            {
                var queue = queueFamilyProperties[i];

                if (queue.queueFlags.HasFlag(VkQueueFlagBits.VK_QUEUE_GRAPHICS_BIT | VkQueueFlagBits.VK_QUEUE_TRANSFER_BIT))
                {
                    graphicsQueueFounded = (int)i;
                }

                this.surfaceKhrExtension.GetPhysicalDeviceSurfaceSupport(device, i, surface, out var supported);

                if (supported)
                {
                    presentationQueueFounded = (int)i;
                }

                if (graphicsQueueFounded > -1 && presentationQueueFounded > -1 && supportedFeatures.samplerAnisotropy)
                {
                    graphicsQueueIndex     = (uint)graphicsQueueFounded;
                    presentationQueueIndex = (uint)presentationQueueFounded;
                    physicalDevice         = device;

                    return;
                }
            }
        }

        throw new Exception("Failed to find a suitable GPU!");
    }

    private void RecreateSwapchain(WindowHandler window)
    {
        this.DestroySwapchain(window.Swapchain);

        window.Swapchain  = this.CreateSwapchain(window.Surface, window.Size);
    }

    private void TransitionImageLayout(
        VkImage                 image,
        VkImageLayout           oldLayout,
        VkImageLayout           newLayout,
        VkAccessFlagBits        srcAccessMask,
        VkAccessFlagBits        dstAccessMask,
        VkPipelineStageFlagBits sourceStage,
        VkPipelineStageFlagBits destinationStage
    )
    {
        var commandBuffer = this.BeginSingleTimeCommands();

        var imageMemoryBarrier = new VkImageMemoryBarrier
        {
            dstAccessMask       = dstAccessMask,
            dstQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            image               = image,
            newLayout           = newLayout,
            oldLayout           = oldLayout,
            srcAccessMask       = srcAccessMask,
            srcQueueFamilyIndex = Vk.VK_QUEUE_FAMILY_IGNORED,
            subresourceRange    = new()
            {
                aspectMask = VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT,
                layerCount = 1,
                levelCount = 1,
            }
        };

        this.Vk.CmdPipelineBarrier(commandBuffer, sourceStage, destinationStage, default, null, null, [imageMemoryBarrier]);

        this.EndSingleTimeCommands(commandBuffer);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            this.Vk.DeviceWaitIdle(this.device);

            foreach (var window in this.windows.ToArray())
            {
                this.FreeWindow(window);
            }

            foreach (var descriptorPool in this.descriptorPools.Values.SelectMany(x => x))
            {
                this.Vk.DestroyDescriptorPool(this.device, descriptorPool.Handler, null);
            }

            for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
            {
                this.Vk.DestroySemaphore(this.device, this.renderingFinishedSemaphores[i], null);
                this.Vk.DestroyFence(this.device, this.fences[i], null);
            }

            this.Vk.FreeCommandBuffers(this.device, this.commandPool, this.commandBuffers);
            this.Vk.DestroyCommandPool(this.device, this.commandPool, null);

            this.Vk.DestroyDevice(this.device, null);

            if (this.EnableValidationLayers)
            {
                this.debugUtilsExtension.DestroyDebugUtilsMessenger(this.Instance, this.debugUtilsMessenger, null);
            }

            this.Vk.DestroyInstance(this.Instance, null);

            this.disposed = true;
        }
    }

    public Frame BeginFrame()
    {
        this.CheckActiveWindow();
        this.CheckDevice();

        var commandBuffer = this.commandBuffers[this.currentFrame];
        var fence         = this.fences[this.currentFrame];
        var swapchain     = this.activeWindow.Swapchain;

        this.frames++;

        var frame = new Frame()
        {
            CommandBuffer = commandBuffer,
            Fence         = fence,
            Index         = this.frames,
            Viewport      = this.activeWindow.Size,
        };

        this.Vk.WaitForFences(this.device, fence, true, ulong.MaxValue);

        var result = this.swapchainKhrExtension.AcquireNextImage(this.device, swapchain.Handler, ulong.MaxValue, this.activeWindow.Semaphores[this.currentFrame], default, out var imageIndex);

        if (result == VkResult.VK_ERROR_OUT_OF_DATE_KHR)
        {
            this.RecreateSwapchain(this.activeWindow);

            frame.Skipped = true;

            return frame;
        }
        else if (result is not VkResult.VK_SUCCESS and not VkResult.VK_SUBOPTIMAL_KHR)
        {
            throw new Exception("failed to acquire swap chain image!");
        }

        this.Vk.ResetFences(this.device, fence);
        this.Vk.ResetCommandBuffer(commandBuffer, default);

        this.activeWindow.CurrentBuffer = imageIndex;

        var clearColorValue = new VkClearColorValue();

        clearColorValue.float32[0] = 1;
        clearColorValue.float32[1] = 1;
        clearColorValue.float32[2] = 1;
        clearColorValue.float32[3] = 1;

        var clearValue = new VkClearValue
        {
            color = clearColorValue
        };

        VkCheck(this.Vk.BeginCommandBuffer(commandBuffer));

        var renderPassBeginInfo = new VkRenderPassBeginInfo
        {
            clearValueCount = 1,
            framebuffer     = swapchain.Framebuffers[this.activeWindow.CurrentBuffer],
            pClearValues    = &clearValue,
            renderArea      = new()
            {
                extent = swapchain.Extent,
            },
            renderPass = swapchain.RenderPass,
        };

        this.Vk.CmdBeginRenderPass(commandBuffer, renderPassBeginInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

        var viewport = new VkViewport
        {
            height   = swapchain.Extent.height,
            maxDepth = 1,
            width    = swapchain.Extent.width,
        };

        this.Vk.CmdSetViewport(commandBuffer, 0, viewport);

        var scissor = new VkRect2D
        {
            extent = swapchain.Extent
        };

        this.Vk.CmdSetScissor(commandBuffer, 0, scissor);

        return frame;
    }

    public void BindIndexBuffer(VkCommandBuffer commandBuffer, IndexBufferHandler indexBuffer) =>
        this.Vk.CmdBindIndexBuffer(commandBuffer, indexBuffer.Buffer.Handler, 0, indexBuffer.Type);

    public void BindPipeline(VkCommandBuffer commandBuffer, ShaderHandler shader) =>
        this.Vk.CmdBindPipeline(commandBuffer, shader.PipelineBindPoint, shader.Pipeline);

    public void BindVertexBuffer(VkCommandBuffer commandBuffer, VertexBufferHandler vertexBuffer) =>
        this.Vk.CmdBindVertexBuffers(commandBuffer, 0, [vertexBuffer.Buffer.Handler], [0]);

    public void BindVertexBuffer(VkCommandBuffer commandBuffer, VertexBufferHandler[] vertexBuffers) =>
        this.Vk.CmdBindVertexBuffers(commandBuffer, 0, [.. vertexBuffers.Select(x => x.Buffer.Handler)], new VkDeviceSize[vertexBuffers.Length]);

    public void BindUniformSet(VkCommandBuffer commandBuffer, UniformSet uniformSet) =>
        this.Vk.CmdBindDescriptorSets(commandBuffer, uniformSet.Shader.PipelineBindPoint, uniformSet.Shader.PipelineLayout, 0, uniformSet.DescriptorSets, null);

    public BufferHandler CreateBuffer(ulong size, VkBufferUsageFlagBits usage, VkMemoryPropertyFlagBits properties)
    {
        this.CreateBuffer(size, usage, properties, out var buffer, out var allocation);

        return new()
        {
            Allocation = allocation,
            Handler    = buffer,
            Usage      = usage,
        };
    }

    public IndexBufferHandler CreateIndexBuffer(IList<ushort> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.VK_INDEX_TYPE_UINT16);

    public IndexBufferHandler CreateIndexBuffer(IList<uint> indices) =>
        this.CreateIndexBuffer(indices, VkIndexType.VK_INDEX_TYPE_UINT32);

    public IndexBufferHandler CreateIndexBuffer<T>(IList<T> indices, VkIndexType indexType) where T : unmanaged, INumber<T>
    {
        VkDeviceSize bufferSize = (ulong)(sizeof(T) * indices.Count);

        var buffer = this.CreateBuffer(
            bufferSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_INDEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        );

        this.UpdateBuffer(buffer, [.. indices]);

        return new()
        {
            Buffer = buffer,
            Type   = indexType,
            Size   = (uint)indices.Count,
        };
    }

    public ShaderHandler CreateShader()
    {
        var descriptorSetLayout = this.CreateDescriptorSetLayout();
        var pipelineLayout      = this.CreatePipelineLayout(descriptorSetLayout);
        var pipeline            = this.CreatePipeline(pipelineLayout);

        return new()
        {
            DescriptorSetLayout = descriptorSetLayout,
            Pipeline            = pipeline,
            PipelineBindPoint   = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            PipelineLayout      = pipelineLayout,
        };
    }

    public VkSampler CreateSampler()
    {
        this.Vk.GetPhysicalDeviceProperties(this.physicalDevice, out var properties);

        var samplerCreateInfo = new VkSamplerCreateInfo
        {
            addressModeU  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeV  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeW  = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            borderColor   = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            compareOp     = VkCompareOp.VK_COMPARE_OP_ALWAYS,
            magFilter     = VkFilter.VK_FILTER_LINEAR,
            maxAnisotropy = properties.limits.maxSamplerAnisotropy,
            maxLod        = 1,
            minFilter     = VkFilter.VK_FILTER_LINEAR,
            mipmapMode    = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_LINEAR,
        };

        VkCheck(this.Vk.CreateSampler(this.device, samplerCreateInfo, default, out var sampler));

        return sampler;
    }

    public TextureHandler CreateTexture(TextureCreate textureCreate)
    {
        var imageType = ConvertToImageType(textureCreate.TextureType);

        var samples = VkSampleCountFlagBits.VK_SAMPLE_COUNT_1_BIT;
        var tiling  = VkImageTiling.VK_IMAGE_TILING_OPTIMAL;
        var usage   = VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_SRC_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_TRANSFER_DST_BIT | VkImageUsageFlagBits.VK_IMAGE_USAGE_SAMPLED_BIT;
        var format  = VkFormat.VK_FORMAT_B8G8R8A8_SRGB;

        this.CreateImage(
            textureCreate.Width,
            textureCreate.Height,
            samples,
            format,
            tiling,
            usage,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT,
            out var image,
            out var allocation
        );

        this.TransitionImageLayout(
            image,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            default,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT
        );

        var imageView = this.CreateImageView(image, VkFormat.VK_FORMAT_B8G8R8A8_SRGB, VkImageAspectFlagBits.VK_IMAGE_ASPECT_COLOR_BIT);

        var texture = new TextureHandler
        {
            Allocation   = allocation,
            Depth        = textureCreate.Depth,
            Format       = format,
            Height       = textureCreate.Height,
            Image        = image,
            ImageTiling  = tiling,
            ImageType    = imageType,
            ImageUsage   = usage,
            ImageView    = imageView,
            SampleCount  = samples,
            Samples      = samples,
            Width        = textureCreate.Width,
        };

        this.UpdateTexture(texture, textureCreate.Data);

        return texture;
    }

    public UniformSet CreateUniformSet(IList<Uniform> uniforms, ShaderHandler shader)
    {
        using var disposables = new Disposables();

        var poolKey = uniforms.Select(x => ConvertToDescriptorType(x.Type)).Aggregate((previous, current) => previous | current);

        var writes = new List<VkWriteDescriptorSet>();

        var descriptorPool = this.CreateDescriptorPool(poolKey);

        var descriptorSetLayout = shader.DescriptorSetLayout;

        var descriptorSetAllocateInfo = new VkDescriptorSetAllocateInfo
        {
            descriptorPool     = descriptorPool.Handler,
            descriptorSetCount = 1,
            pSetLayouts        = &descriptorSetLayout,
        };

        this.Vk.AllocateDescriptorSets(this.device, descriptorSetAllocateInfo, out var descriptorSets);

        foreach (var uniform in uniforms)
        {
            switch (uniform)
            {
                case CombinedImageSamplerUniform combinedImageSampler:
                {
                    var pImageInfo = new PointerArray<VkDescriptorImageInfo>(combinedImageSampler.Images.Count);

                    disposables.Add(pImageInfo);

                    for (var i = 0; i < combinedImageSampler.Images.Count; i++)
                    {
                        var image = combinedImageSampler.Images[i];

                        var descriptorImageInfo = new VkDescriptorImageInfo
                        {
                            sampler     = image.Sampler,
                            imageView   = image.Texture.ImageView,
                            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
                        };

                        pImageInfo[i] = descriptorImageInfo;
                    }

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        descriptorCount = 1,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
                        dstBinding      = uniform.Binding,
                        dstSet          = descriptorSets[0],
                        pImageInfo      = pImageInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                case UniformBufferUniform uniformBuffer:
                {
                    var descriptorBufferInfo = new VkDescriptorBufferInfo
                    {
                        buffer = uniformBuffer.Buffer.Handler,
                        offset = uniformBuffer.Buffer.Allocation.Offset,
                        range  = uniformBuffer.Buffer.Allocation.Size,
                    };

                    var pBufferInfo = new PointerArray<VkDescriptorBufferInfo>([descriptorBufferInfo]);

                    disposables.Add(pBufferInfo);

                    var writeDescriptorSet = new VkWriteDescriptorSet
                    {
                        descriptorCount = 1,
                        descriptorType  = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
                        dstBinding      = uniform.Binding,
                        dstSet          = descriptorSets[0],
                        pBufferInfo     = pBufferInfo,
                    };

                    writes.Add(writeDescriptorSet);

                    break;
                }
                default:
                    throw new Exception();
            }

        }

        this.Vk.UpdateDescriptorSets(this.device, [..writes], null);

        var uniformSet = new UniformSet()
        {
            DescriptorPool = descriptorPool,
            DescriptorSets = descriptorSets,
            Shader         = shader,
        };

        return uniformSet;
    }

    public VertexBufferHandler CreateVertexBuffer<T>(T[] data) where T : unmanaged
    {
        var size = (ulong)(data.Length * sizeof(T));
        var buffer = this.CreateBuffer(
            size,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_DST_BIT | VkBufferUsageFlagBits.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        );

        this.UpdateBuffer(buffer, data);

        return new()
        {
            Buffer = buffer,
            Size   = size,
        };
    }

    public virtual WindowHandler CreateWindow(VkSurfaceKHR surface, Size<uint> size, bool activate)
    {
        if (!this.deviceInitialized)
        {
            this.InitializeDevice(surface);

            this.deviceInitialized = true;
        }

        var swapchain = this.CreateSwapchain(surface, size);

        var semaphores = new VkSemaphore[MAX_FRAMES_IN_FLIGHT];

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            VkCheck(this.Vk.CreateSemaphore(this.device, new VkSemaphoreCreateInfo(), default, out semaphores[i]));
        }

        var window = new WindowHandler
        {
            Semaphores = semaphores,
            Size       = size,
            Surface    = surface,
            Swapchain  = swapchain,
        };

        if (activate)
        {
            this.activeWindow = window;
        }

        this.windows.Add(window);

        return window;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void DrawIndexed(VkCommandBuffer commandBuffer, IndexBufferHandler indexBuffer) =>
        this.Vk.CmdDrawIndexed(commandBuffer, indexBuffer.Size, 1, 0, 0, 0);

    public void EndFrame(Frame frame)
    {
        this.CheckActiveWindow();
        this.CheckDevice();

        this.Vk.CmdEndRenderPass(frame.CommandBuffer);
        this.Vk.EndCommandBuffer(frame.CommandBuffer);

        var fence = this.fences[this.currentFrame];

        var waitSemaphores = new[]
        {
            this.activeWindow.Semaphores[this.currentFrame]
        };

        var waitStages = new VkPipelineStageFlags[]
        {
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT
        };

        var commandBuffers = new[]
        {
            this.commandBuffers[this.currentFrame]
        };

        var signalSemaphores = new[]
        {
            this.renderingFinishedSemaphores[this.currentFrame]
        };

        fixed (VkSemaphore*          pWaitSemaphores   = waitSemaphores)
        fixed (VkPipelineStageFlags* pWaitDstStageMask = waitStages)
        fixed (VkCommandBuffer*      pCommandBuffers   = commandBuffers)
        fixed (VkSemaphore*          pSignalSemaphores = signalSemaphores)
        {
            var submitInfo = new VkSubmitInfo
            {
                commandBufferCount   = 1,
                pCommandBuffers      = pCommandBuffers,
                pSignalSemaphores    = pSignalSemaphores,
                pWaitDstStageMask    = pWaitDstStageMask,
                pWaitSemaphores      = pWaitSemaphores,
                signalSemaphoreCount = 1,
                waitSemaphoreCount   = 1,
            };

            if (this.Vk.QueueSubmit(this.graphicsQueue, submitInfo, fence) != VkResult.VK_SUCCESS)
            {
                throw new Exception("failed to submit draw command buffer!");
            }

            var dependency = new VkSubpassDependency
            {
                srcSubpass    = Vk.VK_SUBPASS_EXTERNAL,
                dstSubpass    = 0,
                srcStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                srcAccessMask = default,
                dstStageMask  = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
                dstAccessMask = VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT
            };

            var presentInfo = new VkPresentInfoKHR
            {
                waitSemaphoreCount = 1,
                pWaitSemaphores    = pSignalSemaphores
            };

            var swapChains = new[]
            {
                this.activeWindow.Swapchain.Handler,
            };

            var imageIndex = this.activeWindow.CurrentBuffer;

            fixed (VkSwapchainKHR* pSwapchains = swapChains)
            {
                presentInfo.swapchainCount = 1;
                presentInfo.pSwapchains    = pSwapchains;
                presentInfo.pImageIndices  = &imageIndex;

                var result = this.swapchainKhrExtension.QueuePresent(this.presentationQueue, presentInfo);

                if (result is VkResult.VK_ERROR_OUT_OF_DATE_KHR or VkResult.VK_SUBOPTIMAL_KHR || this.activeWindow.FramebufferResized)
                {
                    this.activeWindow.FramebufferResized = false;

                    this.RecreateSwapchain(this.activeWindow);
                }
                else if (result != VkResult.VK_SUCCESS)
                {
                    throw new Exception("failed to present swap chain image!");
                }
            }
        }

        this.currentFrame = (ushort)((this.currentFrame + 1) % MAX_FRAMES_IN_FLIGHT);
    }

    public void FreeBuffer(BufferHandler buffer)
    {
        this.Vk.DestroyBuffer(this.device, buffer.Handler, null);
        this.Vk.FreeMemory(this.device, buffer.Allocation.Memory, null);
    }

    public void FreeIndexBuffer(IndexBufferHandler indexBuffer) =>
        this.FreeBuffer(indexBuffer.Buffer);

    public void FreeVertexBuffer(VertexBufferHandler vertexBuffer) =>
        this.FreeBuffer(vertexBuffer.Buffer);

    public void FreeSampler(VkSampler sampler) =>
        this.Vk.DestroySampler(this.device, sampler, null);

    public void FreeShader(Shader shader)
    {
        this.Vk.DestroyPipeline(this.device, shader.Handler.Pipeline, null);
        this.Vk.DestroyPipelineLayout(this.device, shader.Handler.PipelineLayout, null);
        this.Vk.DestroyDescriptorSetLayout(this.device, shader.Handler.DescriptorSetLayout, null);
    }

    public void FreeTexture(TextureHandler texture)
    {
        this.Vk.FreeMemory(this.device, texture.Allocation.Memory, null);
        this.Vk.DestroyImage(this.device, texture.Image, null);
        this.Vk.DestroyImageView(this.device, texture.ImageView, null);
    }

    public void FreeUniformSet(UniformSet uniformSet)
    {
        VkCheck(this.Vk.FreeDescriptorSets(this.device, uniformSet.DescriptorPool.Handler, uniformSet.DescriptorSets));

        this.RemoveFromDescriptorPool(uniformSet.DescriptorPool);
    }

    public void FreeWindow(WindowHandler window)
    {
        this.CheckDevice();

        this.Vk.DeviceWaitIdle(this.device);

        for (var i = 0; i < MAX_FRAMES_IN_FLIGHT; i++)
        {
            this.Vk.DestroySemaphore(this.device, window.Semaphores[i], null);
        }

        this.DestroySwapchain(window.Swapchain);

        this.surfaceKhrExtension.DestroySurface(this.Instance, window.Surface, null);

        this.windows.Remove(window);
    }

    public void UpdateBuffer<T>(BufferHandler buffer, T data) where T : unmanaged =>
        this.UpdateBuffer(buffer, [data]);

    public void UpdateBuffer<T>(BufferHandler buffer, T[] data) where T : unmanaged
    {
        this.CreateBuffer(
            buffer.Allocation.Size,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var allocation
        );

        var stagingMemory = allocation.Memory;

        this.Vk.MapMemory(this.device, stagingMemory, 0, 0, data);
        this.Vk.UnmapMemory(this.device, stagingMemory);

        this.CopyBuffer(stagingBuffer, buffer.Handler, buffer.Allocation.Size);

        this.Vk.DestroyBuffer(this.device, stagingBuffer, null);
        this.Vk.FreeMemory(this.device, stagingMemory, null);
    }

    public void UpdateIndexBuffer<T>(VertexBufferHandler indexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateIndexBuffer<T>(VertexBufferHandler indexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(indexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBufferHandler vertexBuffer, T data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateVertexBuffer<T>(VertexBufferHandler vertexBuffer, T[] data) where T : unmanaged =>
        this.UpdateBuffer(vertexBuffer.Buffer, data);

    public void UpdateTexture(TextureHandler textureData, uint[] data)
    {
        VkDeviceSize imageSize = (ulong)data.Length * 4;

        this.CreateBuffer(
            imageSize,
            VkBufferUsageFlagBits.VK_BUFFER_USAGE_TRANSFER_SRC_BIT,
            VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT | VkMemoryPropertyFlagBits.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT,
            out var stagingBuffer,
            out var allocation
        );

        var stagingMemory = allocation.Memory;

        VkCheck(this.Vk.MapMemory(this.device, stagingMemory, 0, 0, data));
        this.Vk.UnmapMemory(this.device, stagingMemory);

        this.TransitionImageLayout(
            textureData.Image,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            default,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT
        );

        this.CopyBufferToImage(stagingBuffer, textureData.Image, textureData.Width, textureData.Height);

        this.TransitionImageLayout(
            textureData.Image,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            VkAccessFlagBits.VK_ACCESS_TRANSFER_WRITE_BIT,
            VkAccessFlagBits.VK_ACCESS_SHADER_READ_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TRANSFER_BIT,
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT
        );

        this.Vk.FreeMemory(this.device, stagingMemory, null);
        this.Vk.DestroyBuffer(this.device, stagingBuffer, null);
    }

    public void WaitIdle() =>
        this.Vk.DeviceWaitIdle(this.device);
}
