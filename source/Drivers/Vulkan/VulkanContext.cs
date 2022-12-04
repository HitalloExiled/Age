using System.Text;
using Age.Core;
using Age.Core.Error;
using Age.Extensions;
using Silk.NET.Core;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using static Age.Core.Config.Macros;
using static Age.Core.Error.ErrorMacro;

namespace Age.Drivers.Vulkan;


internal abstract class VulkanContext
{
    private struct VulkanContextData
    {
    }

    private const int MAX_EXTENSIONS = 128;

    private readonly HashSet<string> enabledInstanceExtensionNames = new();

    private static readonly List<List<string>> instanceValidationLayersAlt = new()
    {
		// Preferred set of validation layers.
		new () { "VK_LAYER_KHRONOS_validation" },

		// Alternative (deprecated, removed in SDK 1.1.126.0) set of validation layers.
		new () { "VK_LAYER_LUNARG_standard_validation" },

		// Alternative (deprecated, removed in SDK 1.1.121.1) set of validation layers.
		new () { "VK_LAYER_GOOGLE_threading", "VK_LAYER_LUNARG_parameter_validation", "VK_LAYER_LUNARG_object_tracker", "VK_LAYER_LUNARG_core_validation", "VK_LAYER_GOOGLE_unique_objects" }
	};

    public static Vk VK { get; private set; } = null!;

    private VulkanContextData      data = new();
    private DebugUtilsMessengerEXT dbgMessenger;
    private DebugReportCallbackEXT dbgDebugReport;

    private ExtDebugUtils  extDebugUtils;
    private Instance       inst;
    private Version32      instanceApiVersion;
    private bool           nstInitialized;
    private VulkanHooks?   vulkanHooks;
    private ExtDebugReport extDebugReport;

    private ErrorType CreateInstance()
    {
        VK = Vk.GetApi();

        // Obtain Vulkan version.
        this.ObtainVulkanVersion();

        // Initialize extensions.
        var error = this.InitializeInstanceExtensions();

        if (error != ErrorType.OK)
        {
            return error;
        }

        var enabledExtensionCount = 0u;
        var enabledExtensionNames = new string[MAX_EXTENSIONS];

        if (ERR_FAIL_COND_V(this.enabledInstanceExtensionNames.Count > MAX_EXTENSIONS))
        {
            return ErrorType.ERR_CANT_CREATE;
        }

        foreach (var extensionName in this.enabledInstanceExtensionNames)
        {
            enabledExtensionNames[enabledExtensionCount++] = extensionName;
        }

        // We'll set application version to the Vulkan version we're developing against, even if our instance is based on
        // an older Vulkan version, devices can still support newer versions of Vulkan.
        // The exception is when we're on Vulkan 1.0, we should not set this to anything but 1.0.
        // Note that this value is only used by validation layers to warn us about version issues.
        var applicationApiVersion = this.instanceApiVersion == Vk.Version10 ? Vk.Version10 : Vk.Version12;

        unsafe
        {
            this.data = new VulkanContextData();

            var pEnabledExtensionNamesPointer = stackalloc byte*[enabledExtensionNames.Length];

            UnsafeUtils.StringToByUTF8BytesPointer(enabledExtensionNames, pEnabledExtensionNamesPointer);

            fixed (byte*              pCs      = GLOBAL_GET<string>("application/config/name")?.ToUTF8Bytes())
            fixed (byte*              pVersion = Encoding.UTF8.GetBytes(AGEVersion.VERSION_SHORT_NAME))
            fixed (VulkanContextData* pData    = &this.data)
            {
                var app = new ApplicationInfo
                {
                    SType              = StructureType.ApplicationInfo,
                    PApplicationName   = pCs,
                    ApplicationVersion = 0,
                    PEngineName        = pVersion,
                    EngineVersion      = new Version32(AGEVersion.VERSION_MAJOR, AGEVersion.VERSION_MINOR, AGEVersion.VERSION_PATCH),
                    ApiVersion         = applicationApiVersion
                };

                var instInfo = new InstanceCreateInfo
                {
                    SType                   = StructureType.InstanceCreateInfo,
                    PApplicationInfo        = &app,
                    EnabledExtensionCount   = enabledExtensionCount,
                    PpEnabledExtensionNames = pEnabledExtensionNamesPointer,
                };

                if (this.UseValidationLayers())
                {
                    this.GetPreferredValidationLayers(ref instInfo.EnabledExtensionCount, instInfo.PpEnabledExtensionNames);
                }

                /*
                * This is info for a temp callback to use during CreateInstance.
                * After the instance is created, we use the instance-based
                * function to register the final callback.
                */
                DebugUtilsMessengerCreateInfoEXT dbgMessengerCreateInfo;
                DebugReportCallbackCreateInfoEXT dbgReportCallbackCreateInfo;

                if (VK.IsExtensionPresent(ExtDebugUtils.ExtensionName))
                {
                    // VK_EXT_debug_utils style.
                    dbgMessengerCreateInfo = new()
                    {
                        SType = StructureType.DebugUtilsMessengerCreateInfoExt,
                        MessageSeverity =
                            DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt
                            | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt,
                        MessageType =
                            DebugUtilsMessageTypeFlagsEXT.GeneralBitExt
                            | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt
                            | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
                        PfnUserCallback = new(this.DebugMessengerCallback),
                        PUserData = pData,
                    };

                    instInfo.PNext = &dbgMessengerCreateInfo;
                }
                else if (VK.IsExtensionPresent(ExtDebugReport.ExtensionName))
                {
                    dbgReportCallbackCreateInfo = new()
                    {
                        SType = StructureType.DebugReportCallbackCreateInfoExt,
                        Flags =
                            DebugReportFlagsEXT.DebugBitExt
                            | DebugReportFlagsEXT.WarningBitExt
                            | DebugReportFlagsEXT.PerformanceWarningBitExt
                            | DebugReportFlagsEXT.ErrorBitExt,
                        PfnCallback = new(this.DebugReportCallback),
                        PUserData = pData
                    };

                    instInfo.PNext = &dbgReportCallbackCreateInfo;
                }


                if (this.vulkanHooks != null)
                {
                    if (!this.vulkanHooks.CreateVulkanInstance(ref instInfo, out var inst))
                    {
                        this.inst = inst;

                        return ErrorType.ERR_CANT_CREATE;
                    }
                }
                else
                {
                    var inst = this.inst;

                    var err = VK.CreateInstance(&instInfo, null, &inst);

                    this.inst = inst;

                    var created = ERR_FAIL_COND_V_MSG(
                        err == Result.ErrorIncompatibleDriver,
                        """
                        Cannot find a compatible Vulkan installable client driver (ICD).
                        vkCreateInstance Failure
                        """
                    ) || ERR_FAIL_COND_V_MSG(
                        err == Result.ErrorExtensionNotPresent,
                        """
                        Cannot find a specified extension library.
                        Make sure your layers path is set appropriately.
                        vkCreateInstance Failure
                        """
                    ) || ERR_FAIL_COND_V_MSG(
                        err != Result.Success,
                        """
                        vkCreateInstance failed.
                        Do you have a compatible Vulkan installable client driver (ICD) installed?
                        Please look at the Getting Started guide for additional information.
                        vkCreateInstance Failure
                        """
                    );

                    if (!created)
                    {
                        return ErrorType.ERR_CANT_CREATE;
                    }

                    this.nstInitialized = true;
                }

                // #if USE_VOLK
                //     volkLoadInstance(inst);
                // #endif

                if (VK.IsExtensionPresent(ExtDebugUtils.ExtensionName))
                {
                    if (VK.TryGetInstanceExtension<ExtDebugUtils>(this.inst, out var extDebugUtils))
                    {
                        this.extDebugUtils = extDebugUtils;

                        fixed (DebugUtilsMessengerEXT* pDbgMessenger = &this.dbgMessenger)
                        {
                            var err = this.extDebugUtils.CreateDebugUtilsMessenger(this.inst, &dbgMessengerCreateInfo, null, pDbgMessenger);

                            switch (err)
                            {
                                case Result.Success:
                                    break;
                                case Result.ErrorOutOfHostMemory:
                                    return ERR_FAIL_V_MSG(
                                        ErrorType.ERR_CANT_CREATE,
                                        """
                                        CreateDebugUtilsMessengerEXT: out of host memory
                                        CreateDebugUtilsMessengerEXT Failure
                                        """
                                    );
                                default:
                                    ERR_FAIL_V_MSG(
                                        ErrorType.ERR_CANT_CREATE,
                                        """
                                        CreateDebugUtilsMessengerEXT: unknown failure
                                        CreateDebugUtilsMessengerEXT Failure
                                        """
                                    );
                                    break;
                            }
                        }
                    }

                    return ERR_FAIL_V_MSG(
                        ErrorType.ERR_CANT_CREATE,
                        """
                        GetProcAddr: Failed to init VK_EXT_debug_utils
                        GetProcAddr: Failure
                        """
                    );

                }
                else if (VK.IsExtensionPresent(ExtDebugReport.ExtensionName))
                {
                    if (VK.TryGetInstanceExtension<ExtDebugReport>(this.inst, out var extDebugReport))
                    {
                        this.extDebugReport = extDebugReport;

                        fixed (DebugReportCallbackEXT* pDbgDebugReport = &this.dbgDebugReport)
                        {
                            var err = this.extDebugReport.CreateDebugReportCallback(this.inst, &dbgReportCallbackCreateInfo, null, pDbgDebugReport);

                            switch (err)
                            {
                                case Result.Success:
                                    break;
                                case Result.ErrorOutOfHostMemory:
                                    ERR_FAIL_V_MSG(
                                        ErrorType.ERR_CANT_CREATE,
                                        """
                                        CreateDebugReportCallbackEXT: out of host memory
                                        CreateDebugReportCallbackEXT Failure
                                        """
                                    );
                                    break;
                                default:
                                    ERR_FAIL_V_MSG(
                                        ErrorType.ERR_CANT_CREATE,
                                        """
                                        CreateDebugReportCallbackEXT: unknown failure
                                        CreateDebugReportCallbackEXT Failure
                                        """
                                    );

                                    break;
                            }
                        }

                    }

                    return ERR_FAIL_V_MSG(
                        ErrorType.ERR_CANT_CREATE,
                        """
                        GetProcAddr: Failed to init VK_EXT_debug_report
                        GetProcAddr: Failure
                        """
                        );
                }
            }
        }

        return ErrorType.OK;
    }

    private unsafe uint DebugMessengerCallback(
        DebugUtilsMessageSeverityFlagsEXT   messageSeverity,
        DebugUtilsMessageTypeFlagsEXT       messageTypes,
        DebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void*                               pUserData
    ) => throw new NotImplementedException();

    private unsafe uint DebugReportCallback(
        uint                     flags,
        DebugReportObjectTypeEXT objectType,
        ulong                    @object,
        nuint                    location,
        int                      messageCode,
        byte*                    pLayerPrefix,
        byte*                    pMessage,
        void*                    pUserData
    ) => throw new NotImplementedException();

    private unsafe ErrorType GetPreferredValidationLayers(ref uint count, byte** pNames)
    {
        count = 0;

        if (pNames != null)
        {
            *pNames = null;
        }

        var instanceLayerCount = 0u;
        var properties         = default(LayerProperties);

        var err = VK.EnumerateInstanceLayerProperties(ref instanceLayerCount, ref properties);

        if (err != Result.Success)
        {
            return ERR_FAIL_V(ErrorType.ERR_CANT_CREATE);
        }

        if (instanceLayerCount < 1)
        {
            return ErrorType.OK;
        }

        var instanceLayers = new LayerProperties();

        err = VK.EnumerateInstanceLayerProperties(ref instanceLayerCount, ref instanceLayers);

        if (err != Result.Success)
        {
            return ERR_FAIL_V(ErrorType.ERR_CANT_CREATE);
        }

        for (var i = 0; i < instanceValidationLayersAlt.Count; i++)
        {
            if (this.CheckLayers(instanceValidationLayersAlt[i].Count, instanceValidationLayersAlt[i], instanceLayerCount, instanceLayers))
            {
                count = (uint)instanceValidationLayersAlt[i].Count;

                if (pNames != null)
                {
                    unsafe
                    {
                        var pEntry = stackalloc byte*[instanceValidationLayersAlt[i].Count];

                        UnsafeUtils.StringToByUTF8BytesPointer(instanceValidationLayersAlt[i], pEntry);

                        pNames = pEntry;
                    }
                }

                break;
            }
	    }

        return ErrorType.OK;
    }

    private bool CheckLayers(int count, List<string> list, uint instanceLayerCount, LayerProperties instanceLayers) => throw new NotImplementedException();
    private bool UseValidationLayers() => throw new NotImplementedException();
    private ErrorType InitializeInstanceExtensions() => throw new NotImplementedException();

    private unsafe ErrorType ObtainVulkanVersion()
    {
        var apiVersion = 0u;
        var result = VK.EnumerateInstanceVersion(&apiVersion);

        if (result == Result.Success)
        {
            this.instanceApiVersion = (Version32)apiVersion;
        }
        else
        {
            return ERR_FAIL_V(ErrorType.ERR_CANT_CREATE);
        }

        return ErrorType.OK;
    }

    public ErrorType Initialize()
    {
        var err = this.CreateInstance();

        if (err != ErrorType.OK)
        {
            return err;
        }

        return ErrorType.OK;
    }

}
