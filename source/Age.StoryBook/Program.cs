using Age;
using Age.Core;
using Age.StoryBook;

Logger.Level = LogLevel.Info;

using var engine = new Engine("StoryBook", new(800 + 16, 800 + 39), new(800, 100));

engine.Window.Tree.Root.AppendChild(new StoryBook());

engine.Run();
