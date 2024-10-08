namespace ThirdParty.Vulkan.Enums;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkResult.html">VkResult</see>
/// </summary>
public enum VkResult
{
    Success                                     = 0,
    NotReady                                    = 1,
    Timeout                                     = 2,
    EventSet                                    = 3,
    EventReset                                  = 4,
    Incomplete                                  = 5,
    ErrorOutOfHostMemory                        = -1,
    ErrorOutOfDeviceMemory                      = -2,
    ErrorInitializationFailed                   = -3,
    ErrorDeviceLost                             = -4,
    ErrorMemoryMapFailed                        = -5,
    ErrorLayerNotPresent                        = -6,
    ErrorExtensionNotPresent                    = -7,
    ErrorFeatureNotPresent                      = -8,
    ErrorIncompatibleDriver                     = -9,
    ErrorTooManyObjects                         = -10,
    ErrorFormatNotSupported                     = -11,
    ErrorFragmentedPool                         = -12,
    ErrorUnknown                                = -13,
    ErrorOutOfPoolMemory                        = -1000069000,
    ErrorInvalidExternalHandle                  = -1000072003,
    ErrorFragmentation                          = -1000161000,
    ErrorInvalidOpaqueCaptureAddress            = -1000257000,
    PipelineCompileRequired                     = 1000297000,
    ErrorSurfaceLostKHR                         = -1000000000,
    ErrorNativeWindowInUseKHR                   = -1000000001,
    SuboptimalKHR                               = 1000001003,
    ErrorOutOfDateKHR                           = -1000001004,
    ErrorIncompatibleDisplayKHR                 = -1000003001,
    ErrorValidationFailedEXT                    = -1000011001,
    ErrorInvalidShaderNV                        = -1000012000,
    ErrorImageUsageNotSupportedKHR              = -1000023000,
    ErrorVideoPictureLayoutNotSupportedKHR      = -1000023001,
    ErrorVideoProfileOperationNotSupportedKHR   = -1000023002,
    ErrorVideoProfileFormatNotSupportedKHR      = -1000023003,
    ErrorVideoProfileCodecNotSupportedKHR       = -1000023004,
    ErrorVideoStdVersionNotSupportedKHR         = -1000023005,
    ErrorInvalidDrmFormatModifierPlaneLayoutEXT = -1000158000,
    ErrorNotPermittedKHR                        = -1000174001,
    ErrorFullScreenExclusiveModeLostEXT         = -1000255000,
    ThreadIdleKHR                               = 1000268000,
    ThreadDoneKHR                               = 1000268001,
    OperationDeferredKHR                        = 1000268002,
    OperationNotDeferredKHR                     = 1000268003,
#if EnableBetaExtensions
    ErrorInvalidVideoStdParametersKHR           = -1000299000,
#endif
    ErrorCompressionExhaustedEXT                = -1000338000,
    ErrorIncompatibleShaderBinaryEXT            = 1000482000,
    ErrorOutOfPoolMemoryKHR                     = ErrorOutOfPoolMemory,
    ErrorInvalidExternalHandleKHR               = ErrorInvalidExternalHandle,
    ErrorFragmentationEXT                       = ErrorFragmentation,
    ErrorNotPermittedEXT                        = ErrorNotPermittedKHR,
    ErrorInvalidDeviceAddressEXT                = ErrorInvalidOpaqueCaptureAddress,
    ErrorInvalidOpaqueCaptureAddressKHR         = ErrorInvalidOpaqueCaptureAddress,
    PipelineCompileRequiredEXT                  = PipelineCompileRequired,
    ErrorPipelineCompileRequiredEXT             = PipelineCompileRequired,
}
