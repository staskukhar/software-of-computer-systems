using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class SimpleExpressionTests : ExpressionProcessorTestsBase
{
    [Theory]
    [InlineData("1+1")]
    [InlineData("1+8.5")]
    [InlineData("1.5-1")]
    [InlineData("1/10")]
    [InlineData("2*2")]
    [InlineData("9//4")]
    [InlineData("2^3")]
    [InlineData("5%2")]
    [InlineData("1++")]
    [InlineData("1--")]
    public void TwoSimpleConstantsAroundOperator_ReturnsValidResult(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true);

        Assert.True(result.IsValid);
    }
}
