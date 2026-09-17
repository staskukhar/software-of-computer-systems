using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class VariableAndFunctionTests : ExpressionProcessorTestsBase
{
    [Theory]
    [InlineData("x")]
    [InlineData("x+y")]
    [InlineData("sin(x)")]
    [InlineData("max(a,b)")]
    [InlineData("x1+y2")]
    [InlineData("calculate1(x)")]
    public void ValidVariableAndFunctionExpressions_ReturnValid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true);

        Assert.True(result.IsValid);
    }
}
