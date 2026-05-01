using Age.Core.Collections;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class DictionaryBenchmark
{
    [Params(32, 64)]
    public int Count;

    private NativeDictionary<int, long> ndict = new();
    private readonly Dictionary<int, long> dict = new();

    ~DictionaryBenchmark()
    {
        this.ndict.Dispose();
    }

    [Benchmark]
    public void AddRemove()
    {
        for (var i = 0; i < this.Count; i++)
        {
            this.dict.Add(i, i * i);
        }

        for (var i = 0; i < this.Count; i++)
        {
            this.dict.Remove(i);
        }
    }

    [Benchmark]
    public void NativeAddRemove()
    {
        for (var i = 0; i < this.Count; i++)
        {
            this.ndict.Add(i, i * i);
        }

        for (var i = 0; i < this.Count; i++)
        {
            this.ndict.Remove(i);
        }
    }
}
