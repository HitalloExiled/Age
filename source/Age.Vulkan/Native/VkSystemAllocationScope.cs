namespace Age.Vulkan.Native;

/// <summary>
/// <para>Most Vulkan commands operate on a single object, or there is a sole object that is being created or manipulated. When an allocation uses an allocation scope of <see cref="VK_SYSTEM_ALLOCATION_SCOPE_OBJECT"/> or <see cref="VK_SYSTEM_ALLOCATION_SCOPE_CACHE"/>, the allocation is scoped to the object being created or manipulated.</para>
/// <para>When an implementation requires host memory, it will make callbacks to the application using the most specific allocator and allocation scope available:</para>
/// <para>If an allocation is scoped to the duration of a command, the allocator will use the <see cref="VK_SYSTEM_ALLOCATION_SCOPE_COMMAND"/> allocation scope. The most specific allocator available is used: if the object being created or manipulated has an allocator, that object’s allocator will be used, else if the parent <see cref="VkDevice"/> has an allocator it will be used, else if the parent <see cref="VkInstance"/> has an allocator it will be used. Else,</para>
/// <para>If an allocation is associated with a <see cref="VkValidationCacheEXT"/> or <see cref="VkPipelineCache"/> object, the allocator will use the <see cref="VK_SYSTEM_ALLOCATION_SCOPE_CACHE"/> allocation scope. The most specific allocator available is used (cache, else device, else instance). Else,</para>
/// <para>If an allocation is scoped to the lifetime of an object, that object is being created or manipulated by the command, and that object’s type is not <see cref="VkDevice"/> or <see cref="VkInstance"/>, the allocator will use an allocation scope of <see cref="VK_SYSTEM_ALLOCATION_SCOPE_OBJECT"/>. The most specific allocator available is used (object, else device, else instance). Else,</para>
/// <para>If an allocation is scoped to the lifetime of a device, the allocator will use an allocation scope of <see cref="VK_SYSTEM_ALLOCATION_SCOPE_DEVICE"/>. The most specific allocator available is used (device, else instance). Else,</para>
/// <para>If the allocation is scoped to the lifetime of an instance and the instance has an allocator, its allocator will be used with an allocation scope of <see cref="VK_SYSTEM_ALLOCATION_SCOPE_INSTANCE"/>.</para>
/// <para>Otherwise an implementation will allocate memory through an alternative mechanism that is unspecified.</para>
/// </summary>
public enum VkSystemAllocationScope
{
    /// <summary>
    /// Specifies that the allocation is scoped to the duration of the Vulkan command.
    /// </summary>
    VK_SYSTEM_ALLOCATION_SCOPE_COMMAND  = 0,

    /// <summary>
    /// Specifies that the allocation is scoped to the lifetime of the Vulkan object that is being created or used.
    /// </summary>
    VK_SYSTEM_ALLOCATION_SCOPE_OBJECT   = 1,

    /// <summary>
    /// Specifies that the allocation is scoped to the lifetime of a VkPipelineCache or <see cref="VkValidationCacheEXT"/> object.
    /// </summary>
    VK_SYSTEM_ALLOCATION_SCOPE_CACHE    = 2,

    /// <summary>
    /// Specifies that the allocation is scoped to the lifetime of the Vulkan device.
    /// </summary>
    VK_SYSTEM_ALLOCATION_SCOPE_DEVICE   = 3,

    /// <summary>
    /// Specifies that the allocation is scoped to the lifetime of the Vulkan instance.
    /// </summary>
    VK_SYSTEM_ALLOCATION_SCOPE_INSTANCE = 4,
}
