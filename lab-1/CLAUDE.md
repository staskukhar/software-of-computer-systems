# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~Core.Tests.EpressionProcessor.SimpleExpressionTests"

# Run a single test method
dotnet test --filter "DisplayName~TwoSimpleConstantsAroundOperator_ReturnsValidResult"
```

## Architecture

This is a .NET 10 solution with two projects:

- **Core** — the expression validation library
- **Core.Tests** — xUnit tests for the library

### Expression validation flow

`ExpressionProcessor.ValidateExpression` is a character-by-character state machine. `TokenState` represents the current parser state (e.g. `Digit`, `Letter`, `Operator`, `OpenParenthesis`). On each character, a dedicated `ProcessSymbolAfter*` method handles valid transitions and records errors into a `Dictionary<int, string>` keyed by character index.

The `returnOnError` flag controls whether validation stops at the first error or collects all errors. `ValidateTerminalState` runs after the loop to catch invalid end states (e.g. unclosed parenthesis, trailing operator).

`TokenHelper` holds the static symbol-to-`TokenType` mappings and predicate helpers (`IsOperator`, `IsNegativeSign`, etc.) used throughout the processor.

`ExpressionValidationResult` is a record with `IsValid` and the error dictionary.

### Key design note

A negative sign (`-`) is only valid at the start of an expression or immediately after `(` — not after an operator. The `NegativeSign` state enforces this; `1+-5` is invalid.
