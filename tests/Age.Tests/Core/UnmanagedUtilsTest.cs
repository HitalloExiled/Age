using System.Runtime.InteropServices;
using Age.Core.Unsafe;

namespace Age.Tests.Core;

public class UnmanagedUtilsTest
{
    [Fact]
    public unsafe void CopyFromPointerShouldPass()
    {
        fixed (int* pSource = new[] { 1, 2, 3})
        {
            var destination = new int[3];

            UnmanagedUtils.Copy((nint)pSource, destination, 3);

            for (var i = 0; i < destination.Length; i++)
            {
                Assert.Equal(pSource[i], destination[i]);
            }
        }
    }

    [Fact]
    public unsafe void CopyToPointerShouldPass()
    {
        var source       = new[] { 1, 2, 3 };
        var pDestination = stackalloc int[3];

        UnmanagedUtils.Copy(source, (nint)pDestination, 3);

        for (var i = 0; i < source.Length; i++)
        {
            Assert.Equal(source[i], pDestination[i]);
        }
    }

    [Fact]
    public unsafe void PointerToArrayShouldPass()
    {
        fixed (int* pSource = new[] { 1, 2, 3})
        {
            var destination = UnmanagedUtils.PointerToArray<int>((nint)pSource, 3);

            for (var i = 0; i < destination.Length; i++)
            {
                Assert.Equal(pSource[i], destination[i]);
            }
        }
    }

    [Fact]
    public unsafe void ZeroFillShouldPass()
    {
        var pointer = (byte*)Marshal.AllocHGlobal(3);

        UnmanagedUtils.ZeroFill(pointer, 3);

        for (var i = 0; i < 3; i++)
        {
            Assert.Equal(0, pointer[i]);
        }

        Marshal.FreeHGlobal((nint)pointer);
    }
}
