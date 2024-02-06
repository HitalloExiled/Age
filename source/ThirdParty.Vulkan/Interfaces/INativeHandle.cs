namespace ThirdParty.Vulkan.Interfaces;

public unsafe interface INativeHandle
{
    nint ToPointer();
}
