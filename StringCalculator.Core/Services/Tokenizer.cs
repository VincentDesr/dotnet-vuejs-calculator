using System.Text;
using StringCalculator.Core.Interfaces;
using StringCalculator.Core.Tokens;
using StringCalculator.Core.Operators;
using StringCalculator.Core.Functions;

namespace StringCalculator.Core.Services;

/// <summary>
/// Tokenizes mathematical expressions into a list of tokens.
/// </summary>
public class Tokenizer : ITokenizer
{
    private readonly Dictionary<string, IOperator> _operators;
    private readonly Dictionary<string, IFunction> _functions;

    public Tokenizer()
    {
        // Register operators
        _operators = new IOperator[]
        {
            new AdditionOperator(),
            new SubtractionOperator(),
            new MultiplicationOperator(),
            new DivisionOperator(),
            new ExponentiationOperator()
        }.ToDictionary(op => op.Symbol);

        // Register functions
        _functions = new IFunction[]
        {
            new SqrtFunction()
        }.ToDictionary(fn => fn.Name);
    }

    public IEnumerable<Token> Tokenize(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new ArgumentException("Expression cannot be empty", nameof(expression));

        var tokens = new List<Token>();
        var i = 0;

        while (i < expression.Length)
        {
            var ch = expression[i];

            // Skip whitespace
            if (char.IsWhiteSpace(ch))
            {
                i++;
                continue;
            }

            // Numbers (including decimals)
            if (char.IsDigit(ch) || ch == '.')
            {
                tokens.Add(ReadNumber(expression, ref i));
                continue;
            }

            // Functions (alphabetic)
            if (char.IsLetter(ch))
            {
                tokens.Add(ReadFunction(expression, ref i));
                continue;
            }

            // Parentheses
            if (ch == '(' || ch == ')')
            {
                tokens.Add(new ParenthesisToken(ch.ToString()));
                i++;
                continue;
            }

            // Operators (including negative numbers)
            if (IsOperatorChar(ch))
            {
                // Check if it's a negative number vs subtraction operator
                if (ch == '-' && IsNegativeSign(tokens))
                {
                    i++; // Skip the minus
                    var number = ReadNumber(expression, ref i);
                    tokens.Add(new NumberToken($"-{number.Value}"));
                }
                else
                {
                    tokens.Add(ReadOperator(expression, ref i));
                }
                continue;
            }

            throw new ArgumentException($"Unexpected character: {ch}");
        }

        return tokens;
    }

    private Token ReadNumber(string expression, ref int index)
    {
        var sb = new StringBuilder();
        var hasDecimal = false;

        while (index < expression.Length)
        {
            var ch = expression[index];

            if (char.IsDigit(ch))
            {
                sb.Append(ch);
                index++;
            }
            else if (ch == '.' && !hasDecimal)
            {
                sb.Append(ch);
                hasDecimal = true;
                index++;
            }
            else
            {
                break;
            }
        }

        return new NumberToken(sb.ToString());
    }

    private Token ReadFunction(string expression, ref int index)
    {
        var sb = new StringBuilder();

        while (index < expression.Length && char.IsLetter(expression[index]))
        {
            sb.Append(expression[index]);
            index++;
        }

        var name = sb.ToString().ToLower();

        if (!_functions.TryGetValue(name, out var function))
            throw new ArgumentException($"Unknown function: {name}");

        return new FunctionToken(function);
    }

    private Token ReadOperator(string expression, ref int index)
    {
        var symbol = expression[index].ToString();

        if (!_operators.TryGetValue(symbol, out var op))
            throw new ArgumentException($"Unknown operator: {symbol}");

        index++;
        return new OperatorToken(op);
    }

    private bool IsOperatorChar(char ch) => _operators.ContainsKey(ch.ToString());

    private bool IsNegativeSign(List<Token> tokens)
    {
        // Negative if: at start, after operator, or after left parenthesis
        if (tokens.Count == 0)
            return true;

        var lastToken = tokens[^1];
        return lastToken.Type == TokenType.Operator ||
               (lastToken.Type == TokenType.Parenthesis && ((ParenthesisToken)lastToken).IsLeft);
    }
}
