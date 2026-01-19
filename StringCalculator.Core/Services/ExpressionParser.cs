using StringCalculator.Core.Interfaces;
using StringCalculator.Core.Tokens;

namespace StringCalculator.Core.Services;

/// <summary>
/// Parses infix notation to postfix notation using the Shunting Yard algorithm.
/// </summary>
public class ExpressionParser : IExpressionParser
{
    public IEnumerable<Token> Parse(IEnumerable<Token> tokens)
    {
        var output = new List<Token>();
        var operatorStack = new Stack<Token>();

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Number:
                    output.Add(token);
                    break;

                case TokenType.Function:
                    operatorStack.Push(token);
                    break;

                case TokenType.Operator:
                    var currentOp = ((OperatorToken)token).Operator;

                    // Pop operators with higher or equal precedence (considering associativity)
                    while (operatorStack.Count > 0)
                    {
                        var top = operatorStack.Peek();

                        if (top.Type == TokenType.Operator)
                        {
                            var topOp = ((OperatorToken)top).Operator;

                            if ((currentOp.IsLeftAssociative && currentOp.Precedence <= topOp.Precedence) ||
                                (!currentOp.IsLeftAssociative && currentOp.Precedence < topOp.Precedence))
                            {
                                output.Add(operatorStack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (top.Type == TokenType.Function)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    operatorStack.Push(token);
                    break;

                case TokenType.Parenthesis:
                    var paren = (ParenthesisToken)token;

                    if (paren.IsLeft)
                    {
                        operatorStack.Push(token);
                    }
                    else
                    {
                        // Pop until left parenthesis
                        while (operatorStack.Count > 0)
                        {
                            var top = operatorStack.Pop();

                            if (top.Type == TokenType.Parenthesis && ((ParenthesisToken)top).IsLeft)
                            {
                                // If there's a function on top, pop it to output
                                if (operatorStack.Count > 0 && operatorStack.Peek().Type == TokenType.Function)
                                {
                                    output.Add(operatorStack.Pop());
                                }
                                break;
                            }

                            output.Add(top);
                        }
                    }
                    break;
            }
        }

        // Pop remaining operators
        while (operatorStack.Count > 0)
        {
            var top = operatorStack.Pop();

            if (top.Type == TokenType.Parenthesis)
                throw new ArgumentException("Mismatched parentheses");

            output.Add(top);
        }

        return output;
    }
}
