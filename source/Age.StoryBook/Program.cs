using Age;
using Age.Core;
using Age.StoryBook;

Logger.Level = LogLevel.Info;

using var engine = new Engine("StoryBook", new(800 + 16, 800 + 39), new(800, 100));

engine.Window.Scene2D = new StoryBook();

engine.Run();
