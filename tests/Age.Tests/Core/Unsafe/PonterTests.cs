using Age.Core.Unsafe;

namespace Age.Tests.Core.Unsafe;

public unsafe class PonterTests
{
    [Fact]
    public void NonGenericPointer()
    {
        using var pointer = new Pointer(sizeof(int) * 4);

        var ptr = pointer.As<int>();

        ptr[0] = 1;
        ptr[1] = 2;
        ptr[2] = 3;
        ptr[3] = 4;

        Assert.True((nint)ptr == (nint)pointer);

        Assert.Equal(ptr[0], pointer.Get<int>(0));
        Assert.Equal(ptr[1], pointer.Get<int>(1));
        Assert.Equal(ptr[2], pointer.Get<int>(2));
        Assert.Equal(ptr[3], pointer.Get<int>(3));

        pointer.Set(0, 2);
        pointer.Set(1, 3);
        pointer.Set(2, 4);
        pointer.Set(3, 5);

        Assert.Equal(ptr[0], pointer.Get<int>(0));
        Assert.Equal(ptr[1], pointer.Get<int>(1));
        Assert.Equal(ptr[2], pointer.Get<int>(2));
        Assert.Equal(ptr[3], pointer.Get<int>(3));
    }

    [Fact]
    public void GenericPointer()
    {
        var pointer = new Pointer<int>(4);

        int* ptr = pointer;

        ptr[0] = 1;
        ptr[1] = 2;
        ptr[2] = 3;
        ptr[3] = 4;

        Assert.Equal(ptr[0], pointer[0]);
        Assert.Equal(ptr[1], pointer[1]);
        Assert.Equal(ptr[2], pointer[2]);
        Assert.Equal(ptr[3], pointer[3]);

        pointer[0] = 2;
        pointer[1] = 3;
        pointer[2] = 4;
        pointer[3] = 5;

        Assert.Equal(ptr[0], pointer[0]);
        Assert.Equal(ptr[1], pointer[1]);
        Assert.Equal(ptr[2], pointer[2]);
        Assert.Equal(ptr[3], pointer[3]);
    }

    [Fact]
    public void GenericPointerAccessAfterDisposedShouldThows()
    {
        var pointer = new Pointer<int>(4);
        pointer.Dispose();

        Assert.Throws<NullReferenceException>(() => pointer[0] == 1);
    }

    [Fact]
    public void NonGenericPointerAccessAfterDisposedShouldThows()
    {
        var pointer = new Pointer(sizeof(int) * 4);
        pointer.Dispose();

        Assert.Throws<NullReferenceException>(() => pointer.Set(0, 1));
    }
}