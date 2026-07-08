using ThirdParty.Vulkan.Enums;

namespace ThirdParty.Vulkan;

public unsafe struct VkWaylandSurfaceCreateInfoKHR
{
    public VkStructureType                SType = VkStructureType.WaylandSurfaceCreateInfoKHR;
    public void*                          PNext;
    public VkWaylandSurfaceCreateFlagsKHR Flags;
    public wl_display                     Display;
    public wl_surface                     Surface;

    public VkWaylandSurfaceCreateInfoKHR()
    { }
}
