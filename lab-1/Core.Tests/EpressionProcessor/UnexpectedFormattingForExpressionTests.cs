using Core.Models;

namespace Core.Tests.EpressionProcessor;

public class UnexpectedFormattingForExpressionTests : ExpressionProcessorTestsBase
{
    [Theory]
    [InlineData("5 * 8-cos( x )/ 9++")]
    [InlineData("""
        x +
        x -1 * 9 +
        65 / 19 +
        cos(x)
        """)]
    public void DifferentFormatting_MustNotFailTheValidation(string expression)
    {
        ExpressionValidationResult result = _processor.ValidateExpression(expression, true, minify: true);

        Assert.True(result.IsValid);
    }
}
