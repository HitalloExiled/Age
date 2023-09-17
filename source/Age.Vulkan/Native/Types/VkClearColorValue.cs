using System.Runtime.InteropServices;

namespace Age.Vulkan.Native.Types;

/// <summary>
/// <para>Structure specifying a clear color value.</para>
/// <para>The four array elements of the clear color map to R, G, B, and A components of image formats, in order.</para>
/// <para>If the image has more than one sample, the same value is written to all samples for any pixels being cleared.</para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct VkClearColorValue
{
    /// <summary>
    /// Are the color clear values when the format of the image or attachment is one of the numeric formats with a numeric type that is floating-point. Floating point values are automatically converted to the format of the image, with the clear value being treated as linear if the image is sRGB.
    /// </summary>
    [FieldOffset(0)]
    public fixed float float32[4];

    /// <summary>
    /// Are the color clear values when the format of the image or attachment has a numeric type that is signed integer (SINT). Signed integer values are converted to the format of the image by casting to the smaller type (with negative 32-bit values mapping to negative values in the smaller type). If the integer clear value is not representable in the target type (e.g. would overflow in conversion to that type), the clear value is undefined.
    /// </summary>
    [FieldOffset(0)]
    public fixed int int32[4];

    /// <summary>
    /// Are the color clear values when the format of the image or attachment has a numeric type that is unsigned integer (UINT). Unsigned integer values are converted to the format of the image by casting to the integer type with fewer bits.
    /// </summary>
    [FieldOffset(0)]
    public fixed uint uint32[4];
}
