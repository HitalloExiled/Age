using System.Runtime.CompilerServices;
using Age.Numerics;
using Mat4x4 = System.Numerics.Matrix4x4;

namespace Age.Tests.Numerics;

public class Matrix4x4Tests
{
    [Fact]
    public void Constructor()
    {
        var actual   = new Matrix4x4<float>(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);
        var expected = new Mat4x4(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void Constructor2()
    {
        var actual   = new Matrix4x4<float>(new(1, 1, 1, 1), new(2, 2, 2, 2), new(3, 3, 3, 3), new(4, 4, 4, 4));
        var expected = new Mat4x4(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void Indexer()
    {
        var actual = Matrix4x4<float>.Identity;

        Assert.Equal(1, actual[2, 2]);
    }

    [Fact]
    public void IsIdentity()
    {
        Assert.False(new Matrix4x4<float>().IsIdentity);
        Assert.True(Matrix4x4<float>.Identity.IsIdentity);
    }

    [Fact]
    public void LookingAt()
    {
        var actual   = Matrix4x4<float>.LookingAt(new(2), new(), new(0, 0, 1));
        var expected = Mat4x4.CreateLookAt(new(2), new(), new(0, 0, 1));

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void Perspective()
    {
        var actual   = Matrix4x4<float>.Perspective(800, 400, 0.1f, 10);
        var expected = Mat4x4.CreatePerspective(800, 400, 0.1f, 10);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void PerspectiveWithPositiveInfinityFarPlane()
    {
        var actual   = Matrix4x4<float>.Perspective(800, 400, 0.1f, float.PositiveInfinity);
        var expected = Mat4x4.CreatePerspective(800, 400, 0.1f, float.PositiveInfinity);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void PerspectiveFov()
    {
        var actual   = Matrix4x4<float>.PerspectiveFov(Angle.Radians(45), 800 / 400, 0.1f, 10);
        var expected = Mat4x4.CreatePerspectiveFieldOfView(Angle.Radians(45), 800 / 400, 0.1f, 10);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    [Fact]
    public void PerspectiveFovWithPositiveInfinityFarPlane()
    {
        var actual   = Matrix4x4<float>.PerspectiveFov(Angle.Radians(45), 800 / 400, 0.1f, float.PositiveInfinity);
        var expected = Mat4x4.CreatePerspectiveFieldOfView(Angle.Radians(45), 800 / 400, 0.1f, float.PositiveInfinity);

        Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    }

    // [Fact] // TODO - REVIEW Comparision
    // public void Rotated()
    // {
    //     var actual   = Matrix4x4<float>.Rotated(new(0, 0, 1), Angle.Radians(90));
    //     var expected = Mat4x4.CreateFromAxisAngle(new(0, 0, 1), Angle.Radians(90));

    //     Assert.Equal(Unsafe.As<Mat4x4, Matrix4x4<float>>(ref expected), actual);
    // }

    [Fact]
    public void Rotation()
    {
        // TODO: Review after implements 3D transform on viewport
        static void rotateAndAssert(double angle)
        {
            var rotated  = Matrix4x4<double>.Rotated(Vector3<double>.Up, Angle.Radians(angle));
            var rotation = new Quaternion<double>(Vector3<double>.Up, Angle.Radians(angle));

            Assert.True(Math.Abs(rotated.Rotation.Dot(rotation)) > 1 - 0.000001);

            var matrix = Matrix4x4<double>.Identity;
            matrix.Rotation = rotation;

            Assert.True(Math.Abs(rotation.Dot(matrix.Rotation)) > 1 - 0.000001);
        }

        rotateAndAssert(0);
        rotateAndAssert(45);
        rotateAndAssert(90);
        rotateAndAssert(135);
        rotateAndAssert(180);
        rotateAndAssert(225);
        rotateAndAssert(270);
        rotateAndAssert(315);
        rotateAndAssert(360);
    }

    [Fact]
    public void PerspectiveWithInvalidFovShouldThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.Perspective(800, 400, 0, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.Perspective(800, 400, 0.1f, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.Perspective(800, 400, 10, 0.1f));
    }

    [Fact]
    public void PerspectiveFovWithInvalidFovShouldThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov(-1, 800 / 400, 0.1f, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov((float)Math.PI, 800 / 400, 0.1f, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov(Angle.Radians(45), 800 / 400, 0, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov(Angle.Radians(45), 800 / 400, 0.1f, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov(Angle.Radians(45), 800 / 400, 10, 0.1f));
    }
}
