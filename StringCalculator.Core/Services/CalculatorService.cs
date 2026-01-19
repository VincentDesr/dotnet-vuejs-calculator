using StringCalculator.Core.Interfaces;

namespace StringCalculator.Core.Services;

/// <summary>
/// Orchestrates tokenization, parsing, and evaluation of mathematical expressions.
/// </summary>
public class CalculatorService
{
    private readonly ITokenizer _tokenizer;
    private readonly IExpressionParser _parser;
    private readonly IExpressionEvaluator _evaluator;

    public CalculatorService(
        ITokenizer tokenizer,
        IExpressionParser parser,
        IExpressionEvaluator evaluator)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }

    /// <summary>
    /// Creates a CalculatorService with default implementations.
    /// </summary>
    public CalculatorService() : this(
        new Tokenizer(),
        new ExpressionParser(),
        new ExpressionEvaluator())
    {
    }

    /// <summary>
    /// Evaluates a mathematical expression.
    /// </summary>
    /// <param name="expression">The expression to evaluate.</param>
    /// <returns>The result of the evaluation.</returns>
    public double Evaluate(string expression)
    {
        var tokens = _tokenizer.Tokenize(expression);
        var postfix = _parser.Parse(tokens);
        return _evaluator.Evaluate(postfix);
    }
}
