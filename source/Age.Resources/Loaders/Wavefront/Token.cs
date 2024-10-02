namespace Age.Resources.Loaders.Wavefront;

public record struct Token(TokenType Type, int Line, int Column, int Index, string Value = "");
