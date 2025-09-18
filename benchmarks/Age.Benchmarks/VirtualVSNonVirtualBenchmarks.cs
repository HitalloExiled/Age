using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

public class Base
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int NonVirtualSum(int i) => i * 2;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public virtual int VirtualSum(int i) => i * 2;
}

public class Derived : Base
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public override int VirtualSum(int i) => i * 2;
}

[ShortRunJob]
[MemoryDiagnoser]
public class VirtualVSNonVirtualBenchmarks
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Base Create(bool derived) =>
        derived ? new Derived() : new Base();

    private readonly Base @base   = Create(false);
    private readonly Base derived = Create(true);

    [Benchmark(Baseline = true)]
    public int BaseNonVirtualSum() => this.@base.NonVirtualSum(2);

    [Benchmark]
    public int BaseVirtualSum() => this.@base.VirtualSum(2);

    [Benchmark]
    public int DerivedNonVirtualSum() => this.@derived.NonVirtualSum(2);

    [Benchmark]
    public int DerivedVirtualSum() => this.@derived.VirtualSum(2);
}
