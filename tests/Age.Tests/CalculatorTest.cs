namespace Age.Tests;

public class CalculatorTest
{
    [Fact]
    public void Sum() =>
        Assert.Equal(3, Calculator.Sum(1, 2));
}
