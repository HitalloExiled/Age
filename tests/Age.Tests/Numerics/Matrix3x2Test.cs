using Age.Numerics;

namespace Age.Tests.Numerics;

public class Matrix3x2Test
{
    private const float ERROR_MARGIN = 0.000001f;
    private const float RADIANS      = (float)(Math.PI / 180);

    [Fact]
    public void Translate()
    {
        var matrix = Matrix3x2<float>.Identity;

        void translateAndAssert(Vector2<float> position)
        {
            matrix.Translation = position;

            Assert.Equal(position, matrix.Translation);
            Assert.Equal(0, matrix.Rotation);
            Assert.True(float.Abs(1 - matrix.Scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(1 - matrix.Scale.Y) < ERROR_MARGIN);
        }

        translateAndAssert(new(1, 1));
        translateAndAssert(new(0.5f, 0.5f));
        translateAndAssert(new(0.25f, 0.25f));
    }

    [Fact]
    public void Rotate()
    {
        var matrix = Matrix3x2<float>.Identity;

        void rotateAndAssert(float angle)
        {
            var rotation = angle * RADIANS;

            matrix.Rotation = rotation;

            Assert.True(matrix.Rotation - rotation < ERROR_MARGIN);
            Assert.True(float.Abs(1 - matrix.Scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(1 - matrix.Scale.Y) < ERROR_MARGIN);
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
    public void Scale()
    {
        var matrix = Matrix3x2<float>.Identity;

        void scaleAndAssert(Vector2<float> scale)
        {
            matrix.Scale = scale;

            Assert.True(float.Abs(matrix.Scale.X - scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(matrix.Scale.Y - scale.Y) < ERROR_MARGIN);
        }

        scaleAndAssert(new(1, 1));
        scaleAndAssert(new(0.5f, 0.5f));
    }

    [Fact]
    public void RotateAndTranslate()
    {
        var matrix = Matrix3x2<float>.Identity;

        void rotateTranslateAndAssert(float angle, Vector2<float> position)
        {
            var rotation = angle * RADIANS;

            matrix.Rotation    = rotation;
            matrix.Translation = position;

            Assert.Equal(position, matrix.Translation);
            Assert.True(float.Abs(matrix.Rotation - rotation) < ERROR_MARGIN);
            Assert.True(float.Abs(1 - matrix.Scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(1 - matrix.Scale.Y) < ERROR_MARGIN);
        }

        rotateTranslateAndAssert(0,   new(1, 1));
        rotateTranslateAndAssert(45,  new(0.5f, 0.5f));
        rotateTranslateAndAssert(90,  new(0.5f, 0.25f));
        rotateTranslateAndAssert(135, new(0.25f, 0.75f));
    }

    [Fact]
    public void RotateAndScale()
    {
        var matrix = Matrix3x2<float>.Identity;

        void rotateAndScaleAndAssert(float angle, Vector2<float> scale)
        {
            var rotation = angle * RADIANS;

            matrix.Rotation = rotation;
            matrix.Scale    = scale;

            Assert.True(float.Abs(matrix.Rotation - rotation) < ERROR_MARGIN);
            Assert.True(float.Abs(matrix.Scale.X - scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(matrix.Scale.Y - scale.Y) < ERROR_MARGIN);
        }

        rotateAndScaleAndAssert(0,  new(1, 1));
        rotateAndScaleAndAssert(45, new(0.5f, 0.5f));
        rotateAndScaleAndAssert(90, new(0.5f, 0.25f));
        rotateAndScaleAndAssert(135, new(0.25f, 0.75f));
    }

    [Fact]
    public void TranslateRotateAndScale()
    {
        var matrix = Matrix3x2<float>.Identity;

        void translateRotateScaleAndAssert(Vector2<float> position, float angle, Vector2<float> scale)
        {
            var rotation = angle * RADIANS;

            matrix.Translation = position;
            matrix.Rotation    = rotation;
            matrix.Scale       = scale;

            Assert.Equal(position, matrix.Translation);
            Assert.True(float.Abs(matrix.Rotation - rotation) < ERROR_MARGIN);
            Assert.True(float.Abs(matrix.Scale.X - scale.X) < ERROR_MARGIN);
            Assert.True(float.Abs(matrix.Scale.Y - scale.Y) < ERROR_MARGIN);
        }

        translateRotateScaleAndAssert(new(1, 1),           0, new(1, 1));
        translateRotateScaleAndAssert(new(0.5f, 0.5f),    45, new(0.5f, 0.5f));
        translateRotateScaleAndAssert(new(0.5f, 0.25f),   90, new(0.5f, 0.25f));
        translateRotateScaleAndAssert(new(0.25f, 0.75f), 135, new(0.25f, 0.75f));
    }

    [Fact]
    public void Inverse()
    {
        var parent = Matrix3x2<double>.Identity;

        parent.Translation = new(1, 0);
        parent.Rotation    = 45;
        parent.Scale       = new(0.25, 0.75);

        var child = Matrix3x2<double>.Identity;

        child.Translation = new(1, 0);
        child.Rotation    = -90;
        child.Scale       = new(3, 2);

        var childGlobal = child * parent;
        var inverse     = parent.Inverse();
        var childLocal  = childGlobal * inverse;

        Assert.Equal(child, childLocal);
    }
}
