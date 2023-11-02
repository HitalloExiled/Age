namespace Age.Loaders.Wavefront;

public record Token(TokenType Type, int Line, int Column, int Index, string Value = "");
