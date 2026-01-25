using Microsoft.AspNetCore.Mvc;
using StringCalculator.Api.Models;
using StringCalculator.Core.Services;

namespace StringCalculator.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly CalculatorService _calculatorService;
    private readonly ILogger<CalculatorController> _logger;

    public CalculatorController(
        CalculatorService calculatorService,
        ILogger<CalculatorController> logger)
    {
        _calculatorService = calculatorService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current server time and API status.
    /// </summary>
    /// <returns>Current time and status message.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            message = "String Calculator API is running",
            serverTime = DateTime.UtcNow,
            localTime = DateTime.Now,
            endpoint = "/api/calculator/evaluate"
        });
    }

    /// <summary>
    /// Evaluates a mathematical expression.
    /// </summary>
    /// <param name="request">The evaluation request containing the expression.</param>
    /// <returns>The evaluation result.</returns>
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(EvaluateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Evaluate([FromBody] EvaluateRequest request)
    {
        var startTime = DateTime.UtcNow;
        try
        {
            if (string.IsNullOrWhiteSpace(request?.Expression))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid expression",
                    Detail = "Expression cannot be empty",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            _logger.LogInformation("Evaluating expression: {Expression}", request.Expression);

            var result = _calculatorService.Evaluate(request.Expression);

            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation("Evaluation completed in {ElapsedMs}ms", elapsed);

            return Ok(new EvaluateResponse(result, request.Expression));
        }
        catch (DivideByZeroException ex)
        {
            _logger.LogWarning(ex, "Division by zero in expression: {Expression}", request?.Expression);
            
            return BadRequest(new ProblemDetails
            {
                Title = "Division by zero",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid expression: {Expression}", request?.Expression);
            
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid expression",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error evaluating expression: {Expression}", request?.Expression);
            
            return BadRequest(new ProblemDetails
            {
                Title = "Evaluation error",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
