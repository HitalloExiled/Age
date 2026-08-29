using Age;
using Age.Core;
using Age.StoryBook;

Logger.Level = LogLevel.Info;

using var engine = new Engine("StoryBook", new(800, 800));

engine.Window.Scene = new StoryBook();

engine.Run();
