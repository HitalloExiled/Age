using Age.Graphs;

namespace Age.Tests.Age;

public partial class RenderGraphTest
{
    private abstract class TestPass<TThis, TOutput>(List<Entry> results) : RenderGraphNode<TOutput>
    where TOutput : new()
    {
        private TOutput? output;
        public override TOutput? Output => this.output;
        public override string   Name   => typeof(TThis).Name;

        protected override void AfterExecute() =>
            results.Add(new(typeof(TThis).Name, null, this.Output?.ToString()));

        protected void SetOutput(TOutput? value) =>
            this.output = value;

        protected override void OnDisposed(bool disposing) { }
    }

    private abstract class TestPass<TThis, TInput, TOutput>(List<Entry> results) : RenderGraphNode<TInput, TOutput>
    {
        private TOutput? output;

        public override TInput?  Input  { get; set; }
        public override TOutput? Output => this.output;
        public override string   Name   => typeof(TThis).Name;

        protected override void AfterExecute() =>
            results.Add(new(typeof(TThis).Name, this.Input?.ToString(), this.Output?.ToString()));

        protected void SetOutput(TOutput? value) =>
            this.output = value;

        protected override void OnDisposed(bool disposing) { }
    }
}
