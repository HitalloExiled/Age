using Age;
using Age.Core;
using Age.Playground;

Logger.Level = LogLevel.Info;

using var engine = new Engine("Age", new(800 + 16, 800 + 39), new(800, 100));

engine.Window.Tree.Root.AppendChild(new Editor());

engine.Run();
