namespace Core.Models;

public class Token(string value, TokenType type)
{
    public required string Value { get; set; } = value;

    public required TokenType Type { get; set; } = type;
}
