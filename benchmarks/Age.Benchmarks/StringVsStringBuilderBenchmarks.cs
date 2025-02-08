using System.Text;
using Age.Core;
using BenchmarkDotNet.Attributes;

#pragma warning disable CA1001 // TODO Remove;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class StringVsStringBuilderBenchmarks
{
    private StringBuilder builder = new();
    private StringHandler handler = new();

    [Params("Hello world!!!")]
    public string Text = null!;

    [Benchmark(Baseline = true)]
    public string RawText()
    {
        var value = "";

        foreach (var item in this.Text)
        {
            value += item;
        }

        return value;
    }

    [Benchmark]
    public string StringBuilder()
    {
        builder.Clear();

        foreach (var item in this.Text)
        {
            builder.Append([item]);
        }

        return builder.ToString();
    }

    [Benchmark]
    public string StringHandler()
    {
        handler.Clear();

        foreach (var item in this.Text)
        {
            handler.Append([item]);
        }

        return handler.ToString();
    }
}
