using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private abstract class TestPass<TThis, TOutput>(List<Entry> results) : RenderGraphNode<TOutput>
    where TOutput : new()
    {
        public override TOutput? Output { get; set; }

        protected override void AfterExecute() =>
            results.Add(new(typeof(TThis).Name, null, this.Output?.ToString()));

        protected override void OnDisposed(bool disposing) { }
    }

    private abstract class TestPass<TThis, TInput, TOutput>(List<Entry> results) : RenderGraphNode<TInput, TOutput>
    {
        public override TInput?  Input  { get; set; }
        public override TOutput? Output { get; set; }

        protected override void AfterExecute() =>
            results.Add(new(typeof(TThis).Name, this.Input?.ToString(), this.Output?.ToString()));

        protected override void OnDisposed(bool disposing) { }
    }
}
