using Age.Numerics;

namespace Age.Tests.Numerics;

public class Vector2Tests
{
    [Fact]
    public void Vector2Angle()
    {
        var v1 = new Vector2<float>(1, 0);
        var v2 = new Vector2<float>(1, 0);

        void rotateAndAssert(float angle)
        {
            var radians = Angle.DegreesToRadians(angle);

            v2.X = float.Cos(radians);
            v2.Y = float.Sin(radians);

            var vectorAngle = Vector2<float>.Angle(v1, v2);

            if (vectorAngle < 0)
            {
                vectorAngle = Math<float>.Tau - -Vector2<float>.Angle(v1, v2);
            }

            Assert.True(Math<float>.IsApprox(vectorAngle, radians));
        }

        rotateAndAssert(0);
        rotateAndAssert(45);
        rotateAndAssert(90);
        rotateAndAssert(135);
        rotateAndAssert(180);
        rotateAndAssert(225);
        rotateAndAssert(270);
        rotateAndAssert(315);
        rotateAndAssert(359);
    }
}
