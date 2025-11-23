using Age.Graphs;
using Age.Numerics;
using Age.Rendering.Resources;
using Age.Scenes;

namespace Age.Tests.Age.Scenes;

#pragma warning disable CS0067

public partial class SceneGraphCacheTest
{
    private sealed class SubViewport : Viewport
    {
        public override event Action? Resized;

        public override Size<uint> Size { get; set; }

        public override string       NodeName     => nameof(SubViewport);
        public override RenderGraph  RenderGraph  => throw new NotImplementedException();
        public override RenderTarget RenderTarget => throw new NotImplementedException();
        public override Texture2D    Texture      => throw new NotImplementedException();

        public SubViewport(string name) => this.Name = name;

        private protected override void OnConnectedInternal() { }
        private protected override void OnDisconnectingInternal() { }
    }
}
