using System.Runtime.CompilerServices;
using System.Text;
using Age.Resources.Loaders.Wavefront.Exceptions;
using TokenPosition = (int Line, int Column, int Index);

namespace Age.Resources.Loaders.Wavefront.Parsers;

public partial class Scanner(StreamReader reader)
{
    private readonly StreamReader reader = reader;

    private int index;
    private int lineNumber = 1;
    private int lineStart;

    private bool Eof  => this.reader.EndOfStream;
    private char Next => (char)this.reader.Peek();

    private State previousState = new(0, 1, 0);

    private static Token CreateToken(TokenType type, TokenPosition position, string value = "") =>
        new(type, position.Line, position.Column, position.Index, value);

    private static bool IsIdentifierPart(char character) =>
        char.IsAsciiLetter(character) || character == '_';

    private void Advance()
    {
        this.index++;
        this.reader.Read();
    }

    private TokenPosition GetTokenPosition() =>
        (this.lineNumber, this.index - this.lineStart + 1, this.index);

    private Token ScanIdentifier()
    {
        var position = this.GetTokenPosition();

        var id = new StringBuilder();

        do
        {
            id.Append(this.Next);

            this.Advance();
        }
        while (!this.Eof && IsIdentifierPart(this.Next));

        return CreateToken(TokenType.Identifier, position, id.ToString());
    }

    private Token ScanNumericLiteral()
    {
        var position = this.GetTokenPosition();

        var result = new StringBuilder();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void collectDigit()
        {
            while (!this.Eof && char.IsDigit(this.Next))
            {
                result.Append(this.Next);

                this.Advance();
            }
        }

        collectDigit();

        if (this.Next == '.')
        {
            result.Append(this.Next);

            this.Advance();

            if (this.Eof || !char.IsDigit(this.Next))
            {
                throw this.ThrowUnexpectedToken();
            }

            collectDigit();
        }

        return CreateToken(TokenType.NumericLiteral, position, result.ToString());
    }

    private Token ScanPunctuator()
    {
        var position = this.GetTokenPosition();

        var next = this.Next;

        var index = this.index;

        switch (next)
        {
            case '.':
            case '-':
            case '/':
                this.Advance();

                break;
        }

        return index == this.index ? throw this.ThrowUnexpectedToken() : CreateToken(TokenType.Punctuator, position, next.ToString());
    }

    private Token ScanStringLiteral()
    {
        var position = this.GetTokenPosition();

        var id = new StringBuilder();

        while (!this.Eof && this.Next != '\n' && !char.IsWhiteSpace(this.Next))
        {
            id.Append(this.Next);

            this.Advance();
        }

        return CreateToken(TokenType.StringLiteral, position, id.ToString());
    }

    private SyntaxErrorException ThrowUnexpectedToken(string? message = null) =>
        new(message ?? "Invalid or unexpected token", this.lineNumber, this.index - this.lineStart + 1, this.index);

    public Token NextToken(bool unrestricted = false)
    {
        this.previousState = new(this.index, this.lineNumber, this.lineStart);

        while (!this.Eof)
        {
            var next = this.Next;

            if (next == '\n')
            {
                var position = this.GetTokenPosition();

                this.lineNumber++;

                this.Advance();

                this.lineStart = this.index;

                return CreateToken(TokenType.NewLine, position);
            }
            else if (char.IsWhiteSpace(next))
            {
                do
                {
                    this.Advance();
                }
                while (!this.Eof && this.Next != '\n' && char.IsWhiteSpace(this.Next));
            }
            else if (next == '#')
            {
                do
                {
                    this.Advance();
                }
                while (!this.Eof && this.Next != '\n');
            }
            else
            {
                return unrestricted
                    ? this.ScanStringLiteral()
                    : IsIdentifierPart(next)
                        ? this.ScanIdentifier()
                        : char.IsDigit(next)
                            ? this.ScanNumericLiteral()
                            : this.ScanPunctuator();
            }
        }

        return CreateToken(TokenType.Eof, this.GetTokenPosition());
    }

    public void Restore()
    {
        this.index      = this.previousState.Index;
        this.lineNumber = this.previousState.LineNumber;
        this.lineStart  = this.previousState.LineStart;

        this.reader.DiscardBufferedData();
        this.reader.BaseStream.Seek(this.previousState.Index, SeekOrigin.Begin);
    }
}
