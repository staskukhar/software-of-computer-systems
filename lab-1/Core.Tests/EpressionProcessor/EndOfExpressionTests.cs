using Core.Formatting;
using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class EndOfExpressionTests
{
    private ExpressionProcessor _processor;

    public EndOfExpressionTests()
    {
        _processor = new ExpressionProcessor();
    }

    [Theory]
    [InlineData("1++")]
    [InlineData("1--")]
    public void ValidTerminalOperatorExpressions_ReturnValid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("1+")]
    [InlineData("1.")]
    [InlineData("(1+2")]
    [InlineData("")]
    public void IncompleteExpressions_ReturnInvalid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, false);

        Assert.False(result.IsValid);
    }
}
