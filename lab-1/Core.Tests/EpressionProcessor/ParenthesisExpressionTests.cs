using Core.Formatting;
using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class ParenthesisExpressionTests
{
    private ExpressionProcessor _processor;

    public ParenthesisExpressionTests()
    {
        _processor = new ExpressionProcessor();
    }

    [Theory]
    [InlineData("(1+2)")]
    [InlineData("((1+2)*3)")]
    [InlineData("sin(x)")]
    [InlineData("max(a,b)")]
    public void ValidParenthesisExpressions_ReturnValid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("(1+2")]
    [InlineData("1+2)")]
    [InlineData("()")]
    public void InvalidParenthesisExpressions_ReturnInvalid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, false);

        Assert.False(result.IsValid);
    }
}
