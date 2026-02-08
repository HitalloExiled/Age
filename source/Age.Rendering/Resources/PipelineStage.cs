using ThirdParty.Vulkan.Flags;

namespace Age.Rendering.Resources;

#pragma warning disable CA1069


public enum PipelineStage
{
    None                             = VkPipelineStageFlags.None,
    NoneKHR                          = VkPipelineStageFlags.NoneKHR,
    TopOfPipe                        = VkPipelineStageFlags.TopOfPipe,
    DrawIndirect                     = VkPipelineStageFlags.DrawIndirect,
    VertexInput                      = VkPipelineStageFlags.VertexInput,
    VertexShader                     = VkPipelineStageFlags.VertexShader,
    TessellationControlShader        = VkPipelineStageFlags.TessellationControlShader,
    TessellationEvaluationShader     = VkPipelineStageFlags.TessellationEvaluationShader,
    GeometryShader                   = VkPipelineStageFlags.GeometryShader,
    FragmentShader                   = VkPipelineStageFlags.FragmentShader,
    EarlyFragmentTests               = VkPipelineStageFlags.EarlyFragmentTests,
    LateFragmentTests                = VkPipelineStageFlags.LateFragmentTests,
    ColorAttachmentOutput            = VkPipelineStageFlags.ColorAttachmentOutput,
    ComputeShader                    = VkPipelineStageFlags.ComputeShader,
    Transfer                         = VkPipelineStageFlags.Transfer,
    BottomOfPipe                     = VkPipelineStageFlags.BottomOfPipe,
    Host                             = VkPipelineStageFlags.Host,
    AllGraphics                      = VkPipelineStageFlags.AllGraphics,
    AllCommands                      = VkPipelineStageFlags.AllCommands,
    CommandPreprocessNV              = VkPipelineStageFlags.CommandPreprocessNV,
    ConditionalRenderingEXT          = VkPipelineStageFlags.ConditionalRenderingEXT,
    TaskShaderEXT                    = VkPipelineStageFlags.TaskShaderEXT,
    TaskShaderNV                     = VkPipelineStageFlags.TaskShaderNV,
    MeshShaderEXT                    = VkPipelineStageFlags.MeshShaderEXT,
    MeshShaderNV                     = VkPipelineStageFlags.MeshShaderNV,
    RayTracingShaderKHR              = VkPipelineStageFlags.RayTracingShaderKHR,
    RayTracingShaderNV               = VkPipelineStageFlags.RayTracingShaderNV,
    FragmentShadingRateAttachmentKHR = VkPipelineStageFlags.FragmentShadingRateAttachmentKHR,
    ShadingRateImageNV               = VkPipelineStageFlags.ShadingRateImageNV,
    FragmentDensityProcessEXT        = VkPipelineStageFlags.FragmentDensityProcessEXT,
    TransformFeedbackEXT             = VkPipelineStageFlags.TransformFeedbackEXT,
    AccelerationStructureBuildKHR    = VkPipelineStageFlags.AccelerationStructureBuildKHR,
    AccelerationStructureBuildNV     = VkPipelineStageFlags.AccelerationStructureBuildNV,
}
