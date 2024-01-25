namespace ThirdParty.Vulkan.Enums.KHR;

/// <summary>
/// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkColorSpaceKHR.html">VkColorSpaceKHR</see>
/// </summary>
public enum ColorSpace
{
    SrgbNonlinear         = 0,
    DisplayP3Nonlinear    = 1000104001,
    ExtendedSrgbLinear    = 1000104002,
    DisplayP3Linear       = 1000104003,
    DciP3Nonlinear        = 1000104004,
    Bt709Linear           = 1000104005,
    Bt709Nonlinear        = 1000104006,
    Bt2020Linear          = 1000104007,
    Hdr10St2084           = 1000104008,
    Dolbyvision           = 1000104009,
    Hdr10Hlg              = 1000104010,
    AdobergbLinear        = 1000104011,
    AdobergbNonlinear     = 1000104012,
    PassThrough           = 1000104013,
    ExtendedSrgbNonlinear = 1000104014,
    DisplayNative         = 1000213000,
    DciP3Linear           = DisplayP3Linear,
}
