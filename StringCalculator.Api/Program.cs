using StringCalculator.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register calculator service
builder.Services.AddSingleton<CalculatorService>();

// Configure CORS for Vue frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Add request timing middleware
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    var path = context.Request.Path;

    Console.WriteLine($"[{startTime:HH:mm:ss.fff}] Incoming request: {context.Request.Method} {path}");

    await next();

    var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
    Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] Completed {path} in {elapsed}ms");
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseCors("AllowVueFrontend");
app.MapControllers();

app.Run();
