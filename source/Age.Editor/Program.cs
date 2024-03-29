using Age;
using Age.Core;
using Age.Editor;

Logger.Level = LogLevel.Trace;

using var engine = new Engine("Age", new(800, 600), new(800, 300));

engine.Window.Content.Add(new Editor());

engine.Run();
