using System.Runtime.InteropServices;
using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public class NativeStringArrayTests
{
    [Fact]
    public unsafe void ConstructorShouldPass()
    {
        var list = new[]
        {
            "One",
            "Two",
            "Three",
        };

        using var stringArrayPtr = new NativeStringArray(list);

        Assert.Equal(list.Length, stringArrayPtr.Length);

        for (var i = 0; i < list.Length; i++)
        {
            Assert.Equal(list[i], Marshal.PtrToStringAnsi((nint)stringArrayPtr.PpData[i]));
        }
    }

    [Fact]
    public unsafe void ToListShouldPass()
    {
        var list = new[]
        {
            "One",
            "Two",
            "Three",
        };

        using var stringArrayPtr = new NativeStringArray(list);

        Assert.True(list.SequenceEqual(stringArrayPtr.ToArray()));
    }

    [Fact]
    public unsafe void ImplicitOperatorShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        using var stringArrayPtr = new NativeStringArray(list);

        byte** ppData = stringArrayPtr;

        Assert.True(ppData == stringArrayPtr.PpData);
    }

    [Fact]
    public unsafe void DisposeShouldPass()
    {
        var list = new[]
        {
            "One",
        };

        var stringArrayPtr = new NativeStringArray(list);

        stringArrayPtr.Dispose();
        stringArrayPtr.Dispose();

        Assert.True(true);
    }

    [Fact]
    public unsafe void DestructorShouldPass()
    {
        _ = new NativeStringArray(Array.Empty<string>());

        Assert.True(true);
    }
}
