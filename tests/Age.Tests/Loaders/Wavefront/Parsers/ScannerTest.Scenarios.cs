using Age.Loaders.Wavefront;
using Age.Loaders.Wavefront.Exceptions;

namespace Age.Tests.Loaders.Wavefront.Parsers;

public partial class ScannerTest
{
    public record ValidScenario(string Source, bool Unrestricted, Token Expected, bool Skip);
    public record InvalidScenario(string Source, SyntaxErrorException Expected, bool Skip);

    public static class Scenarios
    {
        private const bool SKIP = false;

        private static readonly ValidScenario[] valid =
        [
            new("",          false, new Token(TokenType.Eof,            1, 1,  0), SKIP),
            new(" ",         false, new Token(TokenType.Eof,            1, 2,  1), SKIP),
            new("  \n",      false, new Token(TokenType.NewLine,        1, 3,  2), SKIP),
            new("\n",        false, new Token(TokenType.NewLine,        1, 1,  0), SKIP),
            new("\r",        false, new Token(TokenType.Eof,            1, 2,  1), SKIP),
            new("#",         false, new Token(TokenType.Eof,            1, 2,  1), SKIP),
            new("# fooo \n", false, new Token(TokenType.NewLine,        1, 8,  7), SKIP),
            new("v",         false, new Token(TokenType.Identifier,     1, 1,  0, "v"),       SKIP),
            new("v xyz",     false, new Token(TokenType.Identifier,     1, 1,  0, "v"),       SKIP),
            new("vt",        false, new Token(TokenType.Identifier,     1, 1,  0, "vt"),      SKIP),
            new("usemtl",    false, new Token(TokenType.Identifier,     1, 1,  0, "usemtl"),  SKIP),
            new("-",         false, new Token(TokenType.Punctuator,     1, 1,  0, "-"),       SKIP),
            new("/",         false, new Token(TokenType.Punctuator,     1, 1,  0, "/"),       SKIP),
            new("1",         false, new Token(TokenType.NumericLiteral, 1, 1,  0, "1"),       SKIP),
            new("123",       false, new Token(TokenType.NumericLiteral, 1, 1,  0, "123"),     SKIP),
            new("123.456",   false, new Token(TokenType.NumericLiteral, 1, 1,  0, "123.456"), SKIP),
            new("foo.bar",   true,  new Token(TokenType.StringLiteral,  1, 1,  0, "foo.bar"), SKIP),
            new("foo.bar\n", true,  new Token(TokenType.StringLiteral,  1, 1,  0, "foo.bar"), SKIP),
            new("foo bar",   true,  new Token(TokenType.StringLiteral,  1, 1,  0, "foo"),     SKIP),
        ];

        private static readonly InvalidScenario[] invalid =
        [
            new("+",   new SyntaxErrorException("Invalid or unexpected token", 1, 1, 0), SKIP),
            new("1.",  new SyntaxErrorException("Invalid or unexpected token", 1, 3, 2), SKIP),
            new("1.a", new SyntaxErrorException("Invalid or unexpected token", 1, 3, 2), SKIP),
        ];

        public static IEnumerable<object[]> Valid   => valid.Select(x => new[] { x });
        public static IEnumerable<object[]> Invalid => invalid.Select(x => new[] { x });
    }

}
