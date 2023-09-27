using Age.Numerics;
using Mat4x4 = System.Numerics.Matrix4x4;

namespace Age.Tests.Numerics;

public class Matrix4x4Tests
{
    private const double RADIANS = 0.017453292519943295;

    private static bool Compare(Mat4x4 expected, Matrix4x4<float> actual) =>
        expected[0, 0] == actual[0, 0] &&
        expected[0, 1] == actual[0, 1] &&
        expected[0, 2] == actual[0, 2] &&
        expected[0, 3] == actual[0, 3] &&
        expected[1, 0] == actual[1, 0] &&
        expected[1, 1] == actual[1, 1] &&
        expected[1, 2] == actual[1, 2] &&
        expected[1, 3] == actual[1, 3] &&
        expected[2, 0] == actual[2, 0] &&
        expected[2, 1] == actual[2, 1] &&
        expected[2, 2] == actual[2, 2] &&
        expected[2, 3] == actual[2, 3] &&
        expected[3, 0] == actual[3, 0] &&
        expected[3, 1] == actual[3, 1] &&
        expected[3, 2] == actual[3, 2] &&
        expected[3, 3] == actual[3, 3];

    [Fact]
    public void Constructor()
    {
        var actual   = new Matrix4x4<float>(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);
        var expected = new Mat4x4(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void Constructor2()
    {
        var actual   = new Matrix4x4<float>(new(1, 1, 1, 1), new(2, 2, 2, 2), new(3, 3, 3, 3), new(4, 4, 4, 4));
        var expected = new Mat4x4(1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4, 1, 2, 3, 4);

        Assert.True(Compare(expected, actual));
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
    public void LookAt()
    {
        var actual   = Matrix4x4<float>.LookAt(new(2), new(), new(0, 0, 1));
        var expected = Mat4x4.CreateLookAt(new(2), new(), new(0, 0, 1));

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void Perspective()
    {
        var actual   = Matrix4x4<float>.Perspective(800, 400, 0.1f, 10);
        var expected = Mat4x4.CreatePerspective(800, 400, 0.1f, 10);

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void PerspectiveWithPositiveInfinityFarPlane()
    {
        var actual   = Matrix4x4<float>.Perspective(800, 400, 0.1f, float.PositiveInfinity);
        var expected = Mat4x4.CreatePerspective(800, 400, 0.1f, float.PositiveInfinity);

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void PerspectiveFov()
    {
        var actual   = Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), 800 / 400, 0.1f, 10);
        var expected = Mat4x4.CreatePerspectiveFieldOfView((float)(45 * RADIANS), 800 / 400, 0.1f, 10);

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void PerspectiveFovWithPositiveInfinityFarPlane()
    {
        var actual   = Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), 800 / 400, 0.1f, float.PositiveInfinity);
        var expected = Mat4x4.CreatePerspectiveFieldOfView((float)(45 * RADIANS), 800 / 400, 0.1f, float.PositiveInfinity);

        Assert.True(Compare(expected, actual));
    }

    [Fact]
    public void Rotate()
    {
        var actual   = Matrix4x4<float>.Rotate(new(0, 0, 1), (float)(90 * RADIANS));
        var expected = Mat4x4.CreateFromAxisAngle(new(0, 0, 1), (float)(90 * RADIANS));

        Assert.True(Compare(expected, actual));
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
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), 800 / 400, 0, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), 800 / 400, 0.1f, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4<float>.PerspectiveFov((float)(45 * RADIANS), 800 / 400, 10, 0.1f));
    }
}
