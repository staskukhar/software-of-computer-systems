using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class NegativeSignTests : ExpressionProcessorTestsBase
{
    [Theory]
    [InlineData("-5")]
    [InlineData("-5+3")]
    [InlineData("(-5)")]
    [InlineData("8+(-9)")]
    public void ValidNegativeSignExpressions_ReturnValid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("-")]
    [InlineData("1+-5")]
    public void InvalidNegativeSignExpressions_ReturnInvalid(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, false);

        Assert.False(result.IsValid);
    }
}
