using Age.Core.Collections;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public unsafe class ListAddBenchmark
{
    private const int COUNT = 32;
    private NativeList<long> nList = new(COUNT);
    private readonly List<long> list = new(COUNT);
    private readonly UnsafeList* uList = UnsafeList.Allocate<long>(COUNT);

    ~ListAddBenchmark()
    {
        this.nList.Dispose();
        UnsafeList.Free(this.uList);
    }

    [Benchmark]
    public void ListAdd()
    {
        for (var i = 0; i < COUNT; i++)
        {
            this.list.Add(i);
        }

        this.list.Clear();
    }

    [Benchmark]
    public void NativeListAdd()
    {
        for (var i = 0; i < COUNT; i++)
        {
            this.nList.Add(i);
        }

        this.nList.Clear();
    }

    [Benchmark]
    public void UnsafeListAdd()
    {
        for (var i = 0; i < COUNT; i++)
        {
            UnsafeList.Add(this.uList, i);
        }

        UnsafeList.Clear(this.uList);
    }
}
