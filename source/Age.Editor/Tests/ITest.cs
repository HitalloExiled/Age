using Age.Elements;

namespace Age.Editor.Tests;

public interface ITest : IDisposable
{
    void Setup(Canvas canvas, in TestContext context);
}
