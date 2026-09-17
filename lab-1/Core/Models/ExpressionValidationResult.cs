namespace Core.Models;

public record ExpressionValidationResult(
    bool IsValid,
    Dictionary<int, string> errors);
