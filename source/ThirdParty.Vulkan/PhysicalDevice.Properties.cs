using Age.Core.Interop;
using ThirdParty.Vulkan.Enums;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public unsafe partial class PhysicalDevice
{
    /// <inheritdoc cref="VkPhysicalDeviceProperties" />
    public record Properties : NativeReference<VkPhysicalDeviceProperties>
    {
        private string?           deviceName;
        private Limits?           limits;
        private Guid?             pipelineCacheUUID;
        private SparseProperties? sparseProperties;

        public Version             ApiVersion        => new(this.PNative->apiVersion);
        public Version             DriverVersion     => new(this.PNative->driverVersion);
        public uint                VendorId          => this.PNative->vendorID;
        public uint                DeviceId          => this.PNative->deviceID;
        public PhysicalDeviceType  DeviceType        => this.PNative->deviceType;
        public string              DeviceName        => Get(ref this.deviceName, this.PNative->deviceName)!;
        public Guid                PipelineCacheUUID => this.pipelineCacheUUID ??= new(PointerHelper.ToArray(this.PNative->pipelineCacheUUID, Constants.VK_UUID_SIZE));
        public Limits              Limits            => this.limits ??= new(this.PNative->limits);
        public SparseProperties    SparseProperties  => this.sparseProperties ??= new(this.PNative->sparseProperties);

        internal Properties(in VkPhysicalDeviceProperties physicalDeviceProperties) : base(physicalDeviceProperties) { }
    }
}
