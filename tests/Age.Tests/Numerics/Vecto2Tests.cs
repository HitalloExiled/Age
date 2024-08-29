using Age.Numerics;

namespace Age.Tests.Numerics;

public class Vecto2Tests
{
    [Fact]
    public void Angle()
    {
        const float RADIANS = (float)(Math.PI / 180);

        var v1 = new Vector2<float>(1, 0);
        var v2 = new Vector2<float>(1, 0);

        void rotateAndAssert(float angle)
        {
            v1.X = float.Cos(angle * RADIANS);
            v1.Y = float.Sin(angle * RADIANS);

            Assert.True(Vector2.Angle(v1, v2) - angle * RADIANS < float.Epsilon);
        }

        rotateAndAssert(0);
        rotateAndAssert(45);
        rotateAndAssert(90);
        rotateAndAssert(135);
        rotateAndAssert(135);
        rotateAndAssert(180);
        rotateAndAssert(225);
        rotateAndAssert(270);
        rotateAndAssert(315);
        rotateAndAssert(360);
    }
}
