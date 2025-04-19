using System.Text;
using Age.Core;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class StringBufferVsStringBuilderBenchmarks
{
    private readonly StringBuilder builder = new();
    private readonly StringBuffer handler = new();
    private string text = "";

    public string[] Text = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten"];

    [Benchmark(Baseline = true)]
    public string RawText()
    {
        this.text = "";

        foreach (var item in this.Text)
        {
            this.text += item;
        }

        return this.text;
    }

    [Benchmark]
    public string StringBuilder()
    {
        this.builder.Clear();

        foreach (var item in this.Text)
        {
            this.builder.Append(item);
        }

        return this.builder.ToString();
    }

    [Benchmark]
    public string StringHandler()
    {
        this.handler.Clear();

        foreach (var item in this.Text)
        {
            this.handler.Append(item);
        }

        return this.handler.ToString();
    }
}
