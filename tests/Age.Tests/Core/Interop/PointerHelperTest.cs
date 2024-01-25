using System.Runtime.InteropServices;
using Age.Core.Interop;

namespace Age.Tests.Core.Interop;

public class PointerHelperTest
{
    [Fact]
    public unsafe void CopyFromPointerShouldPass()
    {
        fixed (int* pSource = new[] { 1, 2, 3 })
        {
            var destination = new int[3];

            PointerHelper.Copy((nint)pSource, destination, 3);

            for (var i = 0; i < destination.Length; i++)
            {
                Assert.Equal(pSource[i], destination[i]);
            }
        }
    }

    [Fact]
    public unsafe void CopyToPointerShouldPass()
    {
        var source = new[] { 1, 2, 3 };
        var pDestination = stackalloc int[3];

        PointerHelper.Copy(source, (nint)pDestination, 3);

        for (var i = 0; i < source.Length; i++)
        {
            Assert.Equal(source[i], pDestination[i]);
        }
    }

    [Fact]
    public unsafe void NullIfDefaultShouldPass()
    {
        var value = 1;
        var pValue = &value;

        Assert.True(PointerHelper.NullIfDefault(default, pValue) == null);
        Assert.True(PointerHelper.NullIfDefault(value, pValue) == pValue);
    }

    [Fact]
    public unsafe void PointerToArrayShouldPass()
    {
        fixed (int* pSource = new[] { 1, 2, 3 })
        {
            var destination = PointerHelper.ToArray<int>((nint)pSource, 3);

            for (var i = 0; i < destination.Length; i++)
            {
                Assert.Equal(pSource[i], destination[i]);
            }
        }
    }

    [Fact]
    public unsafe void ZeroFillShouldPass()
    {
        var pointer = (byte*)NativeMemory.Alloc(3);

        PointerHelper.ZeroFill(pointer, 3);

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(0, pointer[i]);
        }

        Marshal.FreeHGlobal((nint)pointer);
    }
}
