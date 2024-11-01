using System.Numerics;
using Age.Numerics;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

[ShortRunJob]
[MemoryDiagnoser]
public class GenericMathBenchmarks
{
    [Params(100, 1000)]
    public int Size;

    public static float Sum(float left, float right) => left + right;
    public static double Sum(double left, double right) => left + right;

    public static T Sum<T>(T left, T right) where T : INumber<T> => left + right;
    public static T SumByMathTwo<T>(T value) where T : INumber<T> => value + Math<T>.Two;
    public static T SumByOnePlusOne<T>(T value) where T : INumber<T> => value + T.One + T.One;
    public static T SumCreateChecked<T>(T value) where T : INumber<T> => value + T.CreateChecked(2);

    [Benchmark(Baseline = true)]
    public float SumFloat()
    {
        var x = 0f;

        for (var i = 0f; i < this.Size; i++)
        {
            x += Sum(i, 2f);
        }

        return x;
    }

    [Benchmark]
    public double SumDouble()
    {
        var x = 0.0;

        for (var i = 0.0; i < this.Size; i++)
        {
            x += Sum(i, 2.0);
        }

        return x;
    }

    [Benchmark]
    public float SumGeneric()
    {
        var x = 0.0f;

        for (var i = 0.0f; i < this.Size; i++)
        {
            x += Sum<float>(i, 2.0f);
        }

        return x;
    }

    [Benchmark]
    public float SumCreateChecked()
    {
        var x = 0.0f;

        for (var i = 0.0f; i < this.Size; i++)
        {
            x += SumCreateChecked(i);
        }

        return x;
    }

    [Benchmark]
    public float SumByMathTwo()
    {
        var x = 0.0f;

        for (var i = 0.0f; i < this.Size; i++)
        {
            x += SumByMathTwo(i);
        }

        return x;
    }

    [Benchmark]
    public float SumByOnePlusOne()
    {
        var x = 0.0f;

        for (var i = 0.0f; i < this.Size; i++)
        {
            x += SumByOnePlusOne(i);
        }

        return x;
    }
}
