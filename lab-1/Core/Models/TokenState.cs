namespace Core.Models;

public enum TokenState
{
    Default,
    Digit,
    NegativeSign,
    Letter,
    OpenParenthesis,
    ClosedParenthesis,
    FloatSeparator,
    Coma,
    Operator
}
