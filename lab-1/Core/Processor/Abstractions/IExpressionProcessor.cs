using Core.Models;

namespace Core.Formatting.Abstractions;

public interface IExpressionProcessor
{
     ExpressionValidationResult ValidateExpression(string expression, bool returnOnError);
}
