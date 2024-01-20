namespace Age.Vulkan.Enums;

/// <summary>
/// <para>Framebuffer logical operations.</para>
/// <para>Logical operations are controlled by the logicOpEnable and logicOp members of <see cref="VkPipelineColorBlendStateCreateInfo"/>. The logicOpEnable state can also be controlled by <see cref="VkExtExtendedDynamicState3.CmdSetLogicOpEnable"/> if graphics pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_LOGIC_OP_ENABLE_EXT"/> set in <see cref="VkPipelineDynamicStateCreateInfo.pDynamicStates"/>. The logicOp state can also be controlled by <see cref="VkExtExtendedDynamicState2.CmdSetLogicOp"/> if graphics pipeline is created with <see cref="VkDynamicState.VK_DYNAMIC_STATE_LOGIC_OP_EXT"/> set in <see cref="VkPipelineDynamicStateCreateInfo.pDynamicStates"/>. If logicOpEnable is true, then a logical operation selected by logicOp is applied between each color attachment and the fragment’s corresponding output value, and blending of all attachments is treated as if it were disabled. Any attachments using color formats for which logical operations are not supported simply pass through the color values unmodified. The logical operation is applied independently for each of the red, green, blue, and alpha components.</para>
/// <para>See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkLogicOp.html">Logical Operations</see></para>
/// </summary>
/// <remarks>Provided by VK_VERSION_1_0</remarks>
public enum VkLogicOp
{
    VK_LOGIC_OP_CLEAR         = 0,
    VK_LOGIC_OP_AND           = 1,
    VK_LOGIC_OP_AND_REVERSE   = 2,
    VK_LOGIC_OP_COPY          = 3,
    VK_LOGIC_OP_AND_INVERTED  = 4,
    VK_LOGIC_OP_NO_OP         = 5,
    VK_LOGIC_OP_XOR           = 6,
    VK_LOGIC_OP_OR            = 7,
    VK_LOGIC_OP_NOR           = 8,
    VK_LOGIC_OP_EQUIVALENT    = 9,
    VK_LOGIC_OP_INVERT        = 10,
    VK_LOGIC_OP_OR_REVERSE    = 11,
    VK_LOGIC_OP_COPY_INVERTED = 12,
    VK_LOGIC_OP_OR_INVERTED   = 13,
    VK_LOGIC_OP_NAND          = 14,
    VK_LOGIC_OP_SET           = 15,
}
