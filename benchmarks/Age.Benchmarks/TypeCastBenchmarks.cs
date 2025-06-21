using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Age.Benchmarks;

#pragma warning disable CA1822, CA1859

public enum Kind
{
    N1,
    N2,
    N3,
    N4,
    N5,
    N6,
    N7,
    N8,
    N9,
    N10,
}

public abstract class Kindable
{
    public abstract Kind Kind { get; }
}

public class N1 : Kindable
{
    public override Kind Kind => Kind.N1;
}

public class N2 : N1
{
    public override Kind Kind => Kind.N2;
}

public class N3 : N2
{
    public override Kind Kind => Kind.N3;
}

public class N4 : N3
{
    public override Kind Kind => Kind.N4;
}

public class N5 : N4
{
    public override Kind Kind => Kind.N5;
}

public class N6 : N5
{
    public override Kind Kind => Kind.N6;
}

public class N7 : N6
{
    public override Kind Kind => Kind.N7;
}

public class N8 : N7
{
    public override Kind Kind => Kind.N8;
}

public class N9 : N8
{
    public override Kind Kind => Kind.N9;
}

public class N10 : N9
{
    public override Kind Kind => Kind.N10;
}

// [SimpleJob(RuntimeMoniker.NativeAot90)]
[ShortRunJob]
[MemoryDiagnoser]
public class TypeCastBenchmarks
{
    private static Kindable GetInstance(int id) => id switch
    {
        1  => new N1(),
        2  => new N2(),
        3  => new N3(),
        4  => new N4(),
        5  => new N5(),
        6  => new N6(),
        7  => new N7(),
        8  => new N8(),
        9  => new N9(),
        10 => new N10(),
        _ => throw new Exception(),
    };

    // [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    [Params(1, 2, 3)]
    public int Index;

    [Benchmark(Baseline = true)]
    public Kindable? AsExpression() =>
        GetInstance(this.Index) as N1;

    [Benchmark]
    public Kindable? IsExpression() =>
        GetInstance(this.Index) is N1 a ? a : null;

    [Benchmark]
    public Kindable? TypeOfAndCast()
    {
        var instance = GetInstance(this.Index);

        return instance.GetType() == typeof(N1) ? (N1)instance : null;
    }

    [Benchmark]
    public Kindable? TypeOfAndUnsafeCast()
    {
        var instance = GetInstance(this.Index);

        return instance.GetType() == typeof(N1) ? Unsafe.As<Kindable, N1>(ref instance) : null;
    }

    [Benchmark]
    public Kindable? KindCheckAndUnsafeCast()
    {
        var instance = GetInstance(this.Index);

        return instance.Kind == Kind.N1 ? Unsafe.As<Kindable, N1>(ref instance) : null;
    }
}
