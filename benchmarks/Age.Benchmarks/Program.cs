using Age.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance;
var _ = BenchmarkRunner.Run<TypeCastBenchmarks>(config, args);
