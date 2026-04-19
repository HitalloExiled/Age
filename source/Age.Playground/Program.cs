// using Age;
// using Age.Core;
// using Age.Playground;

// Logger.Level = LogLevel.Info;

// using var engine = new Engine("Age", new(800 + 16, 800 + 39), new(800, 100));

// engine.Window.UIScene = new Editor();

// engine.Run();

using Age.Core;
using Age.Platforms.Display;

Logger.Level = LogLevel.Trace;

Window.Register("org.age.playground");

var window = new Window();

while (!window.IsClosed)
{
    window.DoEvents();
}

Window.Destroy();
