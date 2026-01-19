using StringCalculator.Core.Interfaces;
using StringCalculator.Core.Tokens;

namespace StringCalculator.Core.Services;

/// <summary>
/// Evaluates postfix notation expressions.
/// </summary>
public class ExpressionEvaluator : IExpressionEvaluator
{
    public double Evaluate(IEnumerable<Token> tokens)
    {
        var stack = new Stack<double>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    stack.Push(((NumberToken)token).NumericValue);
                    break;

                case TokenType.Operator:
                    if (stack.Count < 2)
                        throw new ArgumentException("Invalid expression: not enough operands");

                    var right = stack.Pop();
                    var left = stack.Pop();
                    var op = ((OperatorToken)token).Operator;
                    stack.Push(op.Execute(left, right));
                    break;

                case TokenType.Function:
                    var func = ((FunctionToken)token).Function;

                    if (stack.Count < func.ArgumentCount)
                        throw new ArgumentException($"Invalid expression: not enough arguments for {func.Name}");

                    var args = new double[func.ArgumentCount];
                    for (int i = func.ArgumentCount - 1; i >= 0; i--)
                    {
                        args[i] = stack.Pop();
                    }

                    stack.Push(func.Execute(args));
                    break;

                default:
                    throw new ArgumentException($"Unexpected token type in postfix: {token.Type}");
            }
        }

        if (stack.Count != 1)
            throw new ArgumentException("Invalid expression: multiple values remaining");

        return stack.Pop();
    }
}
