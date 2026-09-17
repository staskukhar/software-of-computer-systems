using Core.Formatting.Abstractions;
using Core.Helpers;
using Core.Models;
using System.Text;

namespace Core.Formatting;

public class ExpressionProcessor
    : IExpressionProcessor
{
    public ExpressionValidationResult ValidateExpression(string expression, bool returnOnError)
    {
        Dictionary<int, string> errors = [];
        TokenState state = TokenState.Default;
        StringBuilder buffer = new StringBuilder();
        int parenthesisDepth = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            char symbol = expression[i];
            switch (state)
            {
                case TokenState.Default:
                    ProcessFirstSymbol(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.NegativeSign:
                    ProcessSymbolAfterNegativeSign(symbol, i, buffer, ref state, errors);
                    break;

                case TokenState.Digit:
                    ProcessSymbolAfterDigit(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.FloatSeparator:
                    ProcessSymbolAfterFloatSeparator(symbol, i, buffer, ref state, errors);
                    break;

                case TokenState.Letter:
                    ProcessSymbolAfterLetter(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.Operator:
                    ProcessSymbolAfterOperator(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.OpenParenthesis:
                    ProcessSymbolAfterOpenParenthesis(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.ClosedParenthesis:
                    ProcessSymbolAfterClosedParenthesis(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;

                case TokenState.Coma:
                    ProcessSymbolAfterComa(symbol, i, buffer, ref state, errors, ref parenthesisDepth);
                    break;
            }

            if (returnOnError && errors.Any())
            {
                return new(false, errors);
            }
        }

        ValidateTerminalState(state, buffer, expression.Length, parenthesisDepth, errors);

        return new(!errors.Any(), errors);
    }

    private void ChangeStateTo(ref TokenState currentState, TokenState newState, StringBuilder buffer)
    {
        buffer.Clear();
        currentState = newState;
    }

    private void ValidateTerminalState(
        TokenState state,
        StringBuilder buffer,
        int expressionLength,
        int parenthesisDepth,
        Dictionary<int, string> errors)
    {
        if (parenthesisDepth > 0)
        {
            errors.TryAdd(expressionLength, "Unclosed parenthesis.");
        }

        switch (state)
        {
            case TokenState.Default:
                errors.TryAdd(expressionLength, "Expression is empty.");
                break;

            case TokenState.NegativeSign:
                errors.TryAdd(expressionLength, "Expression cannot end with a negative sign.");
                break;

            case TokenState.FloatSeparator:
                errors.TryAdd(expressionLength, "Expression cannot end after decimal separator.");
                break;

            case TokenState.Operator:
                string op = buffer.ToString();
                if (op != "++" && op != "--")
                {
                    errors.TryAdd(expressionLength, "Expression cannot end with an operator.");
                }
                break;

            case TokenState.Coma:
                errors.TryAdd(expressionLength, "Expression cannot end with a comma.");
                break;
        }
    }

    private void ProcessFirstSymbol(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsNegativeSign(symbol))
        {
            ChangeStateTo(ref state, TokenState.NegativeSign, buffer);
            buffer.Append(symbol);
        }
        else if (char.IsLetter(symbol))
        {
            ChangeStateTo(ref state, TokenState.Letter, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOpenParenthesis(symbol))
        {
            ChangeStateTo(ref state, TokenState.OpenParenthesis, buffer);
            buffer.Append(symbol);
            parenthesisDepth++;
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol at the start.");
        }
    }

    private void ProcessSymbolAfterNegativeSign(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors)
    {
        if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else
        {
            errors.Add(symbolIndex, "Expected digit after negative sign.");
        }
    }

    private void ProcessSymbolAfterDigit(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (char.IsDigit(symbol))
        {
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsFloatSeparator(symbol))
        {
            ChangeStateTo(ref state, TokenState.FloatSeparator, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOperator(symbol))
        {
            ChangeStateTo(ref state, TokenState.Operator, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsCloseParenthesis(symbol))
        {
            parenthesisDepth--;
            if (parenthesisDepth < 0)
            {
                errors.Add(symbolIndex, "Unexpected close parenthesis.");
                parenthesisDepth = 0;
            }
            ChangeStateTo(ref state, TokenState.ClosedParenthesis, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsComa(symbol))
        {
            ChangeStateTo(ref state, TokenState.Coma, buffer);
            buffer.Append(symbol);
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after digit.");
        }
    }

    private void ProcessSymbolAfterFloatSeparator(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors)
    {
        if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after float separator.");
        }
    }

    private void ProcessSymbolAfterLetter(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (char.IsLetterOrDigit(symbol))
        {
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOpenParenthesis(symbol))
        {
            ChangeStateTo(ref state, TokenState.OpenParenthesis, buffer);
            buffer.Append(symbol);
            parenthesisDepth++;
        }
        else if (TokenHelper.IsOperator(symbol))
        {
            ChangeStateTo(ref state, TokenState.Operator, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsCloseParenthesis(symbol))
        {
            parenthesisDepth--;
            if (parenthesisDepth < 0)
            {
                errors.Add(symbolIndex, "Unexpected close parenthesis.");
                parenthesisDepth = 0;
            }
            ChangeStateTo(ref state, TokenState.ClosedParenthesis, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsComa(symbol))
        {
            ChangeStateTo(ref state, TokenState.Coma, buffer);
            buffer.Append(symbol);
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after identifier.");
        }
    }

    private void ProcessSymbolAfterOperator(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (TokenHelper.IsAllowedOperatorSequence(buffer, symbol))
        {
            buffer.Append(symbol);
        }
        else if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else if (char.IsLetter(symbol))
        {
            ChangeStateTo(ref state, TokenState.Letter, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOpenParenthesis(symbol))
        {
            ChangeStateTo(ref state, TokenState.OpenParenthesis, buffer);
            buffer.Append(symbol);
            parenthesisDepth++;
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after operator.");
        }
    }

    private void ProcessSymbolAfterOpenParenthesis(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsNegativeSign(symbol))
        {
            ChangeStateTo(ref state, TokenState.NegativeSign, buffer);
            buffer.Append(symbol);
        }
        else if (char.IsLetter(symbol))
        {
            ChangeStateTo(ref state, TokenState.Letter, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOpenParenthesis(symbol))
        {
            ChangeStateTo(ref state, TokenState.OpenParenthesis, buffer);
            buffer.Append(symbol);
            parenthesisDepth++;
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after open parenthesis.");
        }
    }

    private void ProcessSymbolAfterClosedParenthesis(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (TokenHelper.IsOperator(symbol))
        {
            ChangeStateTo(ref state, TokenState.Operator, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsCloseParenthesis(symbol))
        {
            parenthesisDepth--;
            if (parenthesisDepth < 0)
            {
                errors.Add(symbolIndex, "Unexpected close parenthesis.");
                parenthesisDepth = 0;
            }
            ChangeStateTo(ref state, TokenState.ClosedParenthesis, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsComa(symbol))
        {
            ChangeStateTo(ref state, TokenState.Coma, buffer);
            buffer.Append(symbol);
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after close parenthesis.");
        }
    }

    private void ProcessSymbolAfterComa(
        char symbol,
        int symbolIndex,
        StringBuilder buffer,
        ref TokenState state,
        Dictionary<int, string> errors,
        ref int parenthesisDepth)
    {
        if (char.IsDigit(symbol))
        {
            ChangeStateTo(ref state, TokenState.Digit, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsNegativeSign(symbol))
        {
            ChangeStateTo(ref state, TokenState.NegativeSign, buffer);
            buffer.Append(symbol);
        }
        else if (char.IsLetter(symbol))
        {
            ChangeStateTo(ref state, TokenState.Letter, buffer);
            buffer.Append(symbol);
        }
        else if (TokenHelper.IsOpenParenthesis(symbol))
        {
            ChangeStateTo(ref state, TokenState.OpenParenthesis, buffer);
            buffer.Append(symbol);
            parenthesisDepth++;
        }
        else
        {
            errors.Add(symbolIndex, "Invalid symbol after comma.");
        }
    }
}
