using ThirdParty.Vulkan.Flags;
using ThirdParty.Vulkan.Native;

namespace ThirdParty.Vulkan;

public static partial class GraphicsPipeline
{
    /// <summary>
    /// See <see href="https://registry.khronos.org/vulkan/specs/1.3-extensions/man/html/VkGraphicsPipelineCreateInfo.html">VkGraphicsPipelineCreateInfo</see>
    /// </summary>
    public unsafe record CreateInfo : NativeReference<VkGraphicsPipelineCreateInfo>
    {
        private Pipeline?                              basePipelineHandle;
        private PipelineColorBlendState.CreateInfo?    colorBlendState;
        private PipelineDepthStencilState.CreateInfo?  depthStencilState;
        private PipelineDynamicState.CreateInfo?       dynamicState;
        private PipelineInputAssemblyState.CreateInfo? inputAssemblyState;
        private PipelineLayout?                        layout;
        private PipelineMultisampleState.CreateInfo?   multisampleState;
        private PipelineRasterizationState.CreateInfo? rasterizationState;
        private RenderPass?                            renderPass;
        private PipelineShaderStage.CreateInfo[]       stages = [];
        private PipelineTessellationState.CreateInfo?  tessellationState;
        private PipelineVertexInputState.CreateInfo?   vertexInputState;
        private PipelineViewportState.CreateInfo?      viewportState;

        public nint Next
        {
            get => (nint)this.PNative->pNext;
            init => this.PNative->pNext = value.ToPointer();
        }

        public PipelineCreateFlags Flags
        {
            get => this.PNative->flags;
            init => this.PNative->flags = value;
        }

        public PipelineShaderStage.CreateInfo[] Stages
        {
            get => this.stages;
            init => Init(ref this.stages, ref this.PNative->pStages, ref this.PNative->stageCount, value);
        }

        public PipelineVertexInputState.CreateInfo? VertexInputState
        {
            get => this.vertexInputState;
            init => this.PNative->pVertexInputState = this.vertexInputState = value;
        }

        public PipelineInputAssemblyState.CreateInfo? InputAssemblyState
        {
            get => this.inputAssemblyState;
            init => this.PNative->pInputAssemblyState = this.inputAssemblyState = value;
        }

        public PipelineTessellationState.CreateInfo? TessellationState
        {
            get => this.tessellationState;
            init => this.PNative->pTessellationState = this.tessellationState = value;
        }

        public PipelineViewportState.CreateInfo? ViewportState
        {
            get => this.viewportState;
            init => this.PNative->pViewportState = this.viewportState = value;
        }

        public PipelineRasterizationState.CreateInfo? RasterizationState
        {
            get => this.rasterizationState;
            init => this.PNative->pRasterizationState = this.rasterizationState = value;
        }

        public PipelineMultisampleState.CreateInfo? MultisampleState
        {
            get => this.multisampleState;
            init => this.PNative->pMultisampleState = this.multisampleState = value;
        }

        public PipelineDepthStencilState.CreateInfo? DepthStencilState
        {
            get => this.depthStencilState;
            init => this.PNative->pDepthStencilState = this.depthStencilState = value;
        }

        public PipelineColorBlendState.CreateInfo? ColorBlendState
        {
            get => this.colorBlendState;
            init => this.PNative->pColorBlendState = this.colorBlendState = value;
        }

        public PipelineDynamicState.CreateInfo? DynamicState
        {
            get => this.dynamicState;
            init => this.PNative->pDynamicState = this.dynamicState = value;
        }

        public PipelineLayout? Layout
        {
            get => this.layout;
            init => this.PNative->layout = this.layout = value;
        }

        public RenderPass? RenderPass
        {
            get => this.renderPass;
            init => this.PNative->renderPass = this.renderPass = value;
        }

        public uint Subpass
        {
            get => this.PNative->subpass;
            init => this.PNative->subpass = value;
        }

        public Pipeline? BasePipelineHandle
        {
            get => this.basePipelineHandle;
            init => this.PNative->basePipelineHandle = this.basePipelineHandle = value;
        }

        public int BasePipelineIndex
        {
            get => this.PNative->basePipelineIndex;
            init => this.PNative->basePipelineIndex = value;
        }

        protected override void OnFinalize() =>
            Free(ref this.PNative->pStages);
    }
}
