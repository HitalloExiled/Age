using Age.Core.Collections;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public unsafe class ListIndexOfBenchmark
{
    private const int COUNT = 32;

    private NativeList<long> nList = new(COUNT);
    private readonly List<long> list = new(COUNT);
    private readonly UnsafeList* uList = UnsafeList.Allocate<long>(COUNT);

    public ListIndexOfBenchmark()
    {
        for (var i = 0; i < COUNT; i++)
        {
            this.list.Add(i);
            this.nList.Add(i);
            UnsafeList.Add(this.uList, i);
        }
    }

    ~ListIndexOfBenchmark()
    {
        this.nList.Dispose();
        UnsafeList.Free(this.uList);
    }

    [Benchmark]
    public void ListIndexof()
    {
        var num = this.list.IndexOf(COUNT / 3 * 2);
    }

    [Benchmark]
    public void NativeListIndexOf()
    {
        var num = this.nList.IndexOf(COUNT / 3 * 2);
    }

    [Benchmark]
    public void UnsafeListIndexOf()
    {
        var num = UnsafeList.IndexOf<long>(this.uList, COUNT / 3 * 2);
    }
}
