using Age;
using Age.Core;
using Age.Playground;

Logger.Level = LogLevel.Info;

using var engine = new Engine("Age", new(800, 800));

engine.Window.UIScene = new Editor();

engine.Run();
