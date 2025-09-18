using Age.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance;

// _ = BenchmarkRunner.Run<GenericMathBenchmarks>(config, args);
// _ = BenchmarkRunner.Run<NodeTraversalBenchmarks>(config, args);
// _ = BenchmarkRunner.Run<TypeCastBenchmarks>(config, args);
// _ = BenchmarkRunner.Run<StringVsStringBuilderBenchmarks>(config, args);
// _ = BenchmarkRunner.Run<ComposedTreeTraversalEnumeratorBenchmarks>(config, args);
_ = BenchmarkRunner.Run<VirtualVSNonVirtualBenchmarks>(config, args);
