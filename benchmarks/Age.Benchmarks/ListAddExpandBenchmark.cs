using Age.Core.Collections;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public unsafe class ListAddExpandBenchmark
{
    private const int COUNT = 8;
    private const int COUNTMAX = 65;

    private NativeList<long> nList = new(COUNT);
    private readonly List<long> list = new(COUNT);
    private readonly UnsafeList* uList = UnsafeList.Allocate<long>(COUNT);

    ~ListAddExpandBenchmark()
    {
        this.nList.Dispose();
        UnsafeList.Free(this.uList);
    }

    [Benchmark]
    public void ListAdd()
    {
        for (var i = 0; i < COUNTMAX; i++)
        {
            this.list.Add(i);
        }

        this.list.Clear();
    }

    [Benchmark]
    public void NativeListAdd()
    {
        for (var i = 0; i < COUNTMAX; i++)
        {
            this.nList.Add(i);
        }

        this.nList.Clear();
    }

    [Benchmark]
    public void UnsafeListAdd()
    {
        for (var i = 0; i < COUNTMAX; i++)
        {
            UnsafeList.Add(this.uList, i);
        }

        UnsafeList.Clear(this.uList);
    }
}
