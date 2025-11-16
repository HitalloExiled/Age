using System.Globalization;
using System.Numerics;
using Age.Resources.Loaders.Wavefront.Exceptions;
using Age.Numerics;

namespace Age.Resources.Loaders.Wavefront.Parsers;

public abstract class Parser
{
    private readonly Scanner scanner;

    private Token current = new(TokenType.Bof, 0, 0, 0);

    protected Token Lookahead { get; private set; }

    public Parser(StreamReader reader)
    {
        this.scanner   = new(reader);
        this.Lookahead = this.scanner.NextToken();
    }

    protected static ParseException ParseException(Token token, string message) =>
        new(message, token.Line, token.Column, token.Index);

    protected static ParseException UnexpectedTokenError(Token token)
    {
        var message = token.Type switch
        {
            TokenType.NumericLiteral => "Unexpected number",
            TokenType.Eof            => "Unexpected end of expression",
            _                        => $"Unexpected token {token.Value}",
        };

        return new(message, token.Line, token.Column, token.Index);
    }

    protected void ExpectNewLine()
    {
        if (this.Lookahead.Type is not TokenType.NewLine and not TokenType.Eof)
        {
            throw UnexpectedTokenError(this.Lookahead);
        }

        this.NextToken();
    }

    protected bool Match(string value) =>
        this.Lookahead.Type == TokenType.Punctuator && this.Lookahead.Value == value;

    protected bool MatchIdentifier(string value) =>
        this.Lookahead.Type == TokenType.Identifier && this.Lookahead.Value == value;

    protected bool MatchNewLine() =>
        this.Lookahead.Type == TokenType.NewLine;

    protected Token NextToken(bool unrestricted = false)
    {
        var token = this.current = this.Lookahead;

        this.Lookahead = this.scanner.NextToken(unrestricted);

        return token;
    }

    protected Color ParseColor(float? defaultValue = null)
    {
        var values = defaultValue.HasValue
            ? this.ParseVector(3, 0, defaultValue: defaultValue.Value)
            : this.ParseVector(3);

        return new(values[0], values[1], values[2]);
    }

    protected T ParseNumber<T>(T? fallback = default) where T : INumber<T> =>
        this.TryParseNumber<T>(out var value)
            ? value
            : fallback != null
                ? fallback
                : throw UnexpectedTokenError(this.Lookahead);

    protected float[] ParseVector(int size, int minSize = 3, float minValue = float.MinValue, float maxValue = float.MaxValue, float defaultValue = 0) =>
        this.ParseVector(size, minSize, out var _, minValue, maxValue, defaultValue);

    protected float[] ParseVector(int size, int minSize, out int parsed, float minValue = float.MinValue, float maxValue = float.MaxValue, float defaultValue = 0)
    {
        parsed = 0;

        var values = new float[size];
        var index  = 0;

        while (index < values.Length)
        {
            var toke = this.Lookahead;

            if (this.TryParseNumber<float>(out var value))
            {
                if (value < minValue || value > maxValue)
                {
                    throw ParseException(toke, $"Value out of range {minValue}..{maxValue}");
                }

                values[index] = value;

                parsed = index + 1;
            }
            else
            {
                values[index] = index < minSize ? throw UnexpectedTokenError(this.Lookahead) : defaultValue;
            }

            index++;
        }

        return values;
    }

    protected Vector3<float> ParseVector3(int minSize = 3, float minValue = float.MinValue, float maxValue = float.MaxValue, float defaultValue = 0)
    {
        var values = this.ParseVector(3, minSize, minValue, maxValue, defaultValue);

        return new(values[0], values[1], values[2]);
    }

    protected void Restore()
    {
        this.Lookahead = this.current;

        this.scanner.Restore();
    }

    protected bool TryParseNumber<T>(out T value) where T : INumber<T>
    {
        var isNegative = false;

        if (this.Match("-"))
        {
            this.NextToken();

            isNegative = true;
        }

        if (this.Lookahead.Type == TokenType.NumericLiteral)
        {
            var sign = isNegative ? "-" : "";

            value = T.Parse(sign + this.NextToken().Value, CultureInfo.InvariantCulture);

            return true;
        }

        value = T.Zero;

        return false;
    }
}
