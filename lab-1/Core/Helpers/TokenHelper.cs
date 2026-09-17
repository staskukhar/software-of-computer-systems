using Core.Models;
using System.Text;

namespace Core.Helpers;

public static class TokenHelper
{
    public static readonly char floatSeparator = '.';

    public static readonly Dictionary<string, TokenType> TokenSymbols = new Dictionary<string, TokenType>()
    {
        { " ", TokenType.Whitespace },
        { "+", TokenType.Addition },
        { "-", TokenType.Subtraction },
        { "*", TokenType.Multiplication },
        { "/", TokenType.Division },
        { "^", TokenType.Power },
        { "//", TokenType.IntegerDivision },
        { "%", TokenType.Modulo },
        { "(", TokenType.OpenParenthesis },
        { ")", TokenType.CloseParenthesis },
    };

    public static bool IsNegativeSign(char symbol) =>
        symbol == '-';

    public static bool IsFloatSeparator(char symbol) =>
        symbol == '.';

    public static bool IsOperator(char symbol) =>
        symbol == '+' ||
        symbol == '-' ||
        symbol == '*' ||
        symbol == '/' ||
        symbol == '^' ||
        symbol == '%';

    public static bool IsAllowedOperatorSequence(StringBuilder buffer, char symbol)
    {
        string complexOperator = String.Concat(buffer, symbol);

        return complexOperator == "//" ||
            complexOperator == "++" ||
            complexOperator == "--";
    }

    public static bool IsOpenParenthesis(char symbol) =>
        symbol == '(';

    public static bool IsCloseParenthesis(char symbol) =>
        symbol == ')';

    public static bool IsParenthesis(char symbol) =>
       IsOpenParenthesis(symbol) || IsCloseParenthesis(symbol);

    public static bool IsComa(char symbol) =>
        symbol == ',';
}
