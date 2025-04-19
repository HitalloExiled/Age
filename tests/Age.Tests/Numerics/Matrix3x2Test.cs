using Age.Numerics;

namespace Age.Tests.Numerics;

public class Matrix3x2Test
{
    public static TheoryData<Vector2<float>> TranslateData =>
    [
        new Vector2<float>(1, 1),
        new Vector2<float>(0.5f, 0.5f),
        new Vector2<float>(0.25f, 0.25f)
    ];

    [Theory]
    [MemberData(nameof(TranslateData), MemberType = typeof(Matrix3x2Test))]
    public void CreateTranslated(Vector2<float> position)
    {
        var matrix = Matrix3x2<float>.CreateTranslated(position);

        Assert.Equal(position, matrix.Translation);
        Assert.Equal(0, matrix.Rotation);
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    [Theory]
    [MemberData(nameof(TranslateData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateTranslation(Vector2<float> position)
    {
        var matrix = Matrix3x2<float>.Identity;

        matrix.Translation = position;

        Assert.Equal(position, matrix.Translation);
        Assert.Equal(0, matrix.Rotation);
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    public static TheoryData<float> RotateData => [0f, 45f, 90f, 135f, 179f, -179f, -135f, -90f, -45f];

    [Theory]
    [MemberData(nameof(RotateData), MemberType = typeof(Matrix3x2Test))]
    public void CreateRotated(float angle)
    {
        var rotation = Angle.DegreesToRadians(angle);

        var matrix = Matrix3x2<float>.CreateRotated(rotation);

        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    [Theory]
    [MemberData(nameof(RotateData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateRotatation(float angle)
    {
        var rotation = Angle.DegreesToRadians(angle);

        var matrix = Matrix3x2<float>.Identity;

        matrix.Rotation = rotation;

        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    public static TheoryData<Vector2<float>> ScaleData => [new Vector2<float>(1, 1), new Vector2<float>(0.5f, 0.5f)];

    [Theory]
    [MemberData(nameof(ScaleData), MemberType = typeof(Matrix3x2Test))]
    public void CreateScaled(Vector2<float> scale)
    {
        var matrix = Matrix3x2<float>.CreateScaled(scale);

        Assert.True(matrix.Scale.IsApprox(scale));
    }

    [Theory]
    [MemberData(nameof(ScaleData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateScale(Vector2<float> scale)
    {
        var matrix = Matrix3x2<float>.Identity;

        matrix.Scale = scale;

        Assert.True(matrix.Scale.IsApprox(scale));
    }

    public static TheoryData<(float, Vector2<float>)> RotateAndTranslateData =>
        [
            (0f,   new Vector2<float>(1, 1)),
            (45f,  new Vector2<float>(0.5f, 0.5f)),
            (90f,  new Vector2<float>(0.5f, 0.25f)),
            (135f, new Vector2<float>(0.25f, 0.75f)),
        ];

    [Theory]
    [MemberData(nameof(RotateAndTranslateData), MemberType = typeof(Matrix3x2Test))]
    public void RotateAndTranslate((float Rotation, Vector2<float> Translation) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix = Matrix3x2<float>.CreateRotated(rotation)
            * Matrix3x2<float>.CreateTranslated(data.Translation);

        Assert.Equal(data.Translation, matrix.Translation);
        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    [Theory]
    [MemberData(nameof(RotateAndTranslateData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateRotationAndTranslation((float Rotation, Vector2<float> Translation) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix = Matrix3x2<float>.Identity;

        matrix.Rotation    = rotation;
        matrix.Translation = data.Translation;

        Assert.Equal(data.Translation, matrix.Translation);
        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(Vector2<float>.One));
    }

    public static TheoryData<(float, Vector2<float>)> ScaleAndRotateData =>
        [
            (0f,   new Vector2<float>(1, 1)),
            (45f,  new Vector2<float>(0.5f, 0.5f)),
            (90f,  new Vector2<float>(0.5f, 0.25f)),
            (135f, new Vector2<float>(0.25f, 0.75f)),
        ];

    [Theory]
    [MemberData(nameof(ScaleAndRotateData), MemberType = typeof(Matrix3x2Test))]
    public void ScaleAndRotate((float Rotation, Vector2<float> Scale) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix =
            Matrix3x2<float>.CreateScaled(data.Scale)
            * Matrix3x2<float>.CreateRotated(rotation);

        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(data.Scale));
    }

    [Theory]
    [MemberData(nameof(ScaleAndRotateData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateScaleAndRotatation((float Rotation, Vector2<float> Scale) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix = Matrix3x2<float>.Identity;

        matrix.Scale    = data.Scale;
        matrix.Rotation = rotation;

        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(data.Scale));
    }

    public static TheoryData<(Vector2<float>, float, Vector2<float>)> ScaleRotateAndTranslateData =>
        [
            (new Vector2<float>(1, 1),           0f, new Vector2<float>(1, 1)),
            (new Vector2<float>(0.5f, 0.5f),    45f, new Vector2<float>(0.5f, 0.5f)),
            (new Vector2<float>(0.5f, 0.25f),   90f, new Vector2<float>(0.5f, 0.25f)),
            (new Vector2<float>(0.25f, 0.75f), 135f, new Vector2<float>(0.25f, 0.75f)),
        ];

    [Theory]
    [MemberData(nameof(ScaleRotateAndTranslateData), MemberType = typeof(Matrix3x2Test))]
    public void ScaleRotateAndTranslate((Vector2<float> Translation, float Rotation, Vector2<float> Scale) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix =
            Matrix3x2<float>.CreateScaled(data.Scale)
            * Matrix3x2<float>.CreateRotated(rotation)
            * Matrix3x2<float>.CreateTranslated(data.Translation);

        Assert.Equal(data.Translation, matrix.Translation);
        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(data.Scale));
    }

    [Theory]
    [MemberData(nameof(ScaleRotateAndTranslateData), MemberType = typeof(Matrix3x2Test))]
    public void UpdateScaleRotateAndTranslate((Vector2<float> Translation, float Rotation, Vector2<float> Scale) data)
    {
        var rotation = Angle.DegreesToRadians(data.Rotation);

        var matrix = Matrix3x2<float>.Identity;

        matrix.Translation = data.Translation;
        matrix.Scale       = data.Scale;
        matrix.Rotation    = rotation;

        Assert.Equal(data.Translation, matrix.Translation);
        Assert.True(Math<float>.IsApprox(matrix.Rotation, rotation));
        Assert.True(matrix.Scale.IsApprox(data.Scale));
    }

    [Fact]
    public void Inverse()
    {
        var parent = Matrix3x2<float>.CreateTranslated(1, 0)
            * Matrix3x2<float>.CreateRotated(Angle.DegreesToRadians(45f))
            * Matrix3x2<float>.CreateScaled(0.25f, 0.75f);

        var child = Matrix3x2<float>.CreateTranslated(1, 0)
            * Matrix3x2<float>.CreateRotated(Angle.DegreesToRadians(90f))
            * Matrix3x2<float>.CreateScaled(3, 2);

        var childGlobal = child * parent;
        var inverse     = parent.Inverse();
        var childLocal  = childGlobal * inverse;

        Assert.True(child.IsAprox(childLocal));
    }
}
