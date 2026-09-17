namespace Core.Models;

public enum TokenType
{
    Unknown,
    EscapeSequence,
    Whitespace,
    Number,
    Variable,
    Function,
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Power,
    IntegerDivision,
    Modulo,
    OpenParenthesis,
    CloseParenthesis,
}
