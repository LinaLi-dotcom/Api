using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactCorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") 
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<ILoanService, LoanService>();

var app = builder.Build();

// Apply CORS policy to all endpoints
app.UseCors("ReactCorsPolicy");

// Use middlewares for logging and rewriting
// app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));
app.Use(async (context, next) => {
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Started.");
    await next();
    Console.WriteLine($"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}] Finished.");
});

app.MapGet("/loan", (ILoanService service) => 
{
    // Return the loan calculation form or any other UI for input
    return Results.Ok("Loan calculation form");
});

app.MapPost("/loan/calculate", (LoanRequest request, ILoanService service) =>
{
    try
    {
        // Calculate the monthly payment based on the loan request
        decimal monthlyPayment = service.CalculateMonthlyPayment(request);

        // Return the monthly payment
        return TypedResults.Ok(monthlyPayment);
    }
    catch (Exception ex)
    {
        // Handle any errors during the calculation
        Console.WriteLine($"Error calculating loan payment: {ex.Message}");
        return Results.BadRequest("Error calculating loan payment");
    }
});

app.Run();

public record LoanRequest(int LoanAmount, decimal InterestRate, int RepaymentPeriodYears);

public interface ILoanService
{
    decimal CalculateMonthlyPayment(LoanRequest request);
}

public class LoanService : ILoanService
{
    public decimal CalculateMonthlyPayment(LoanRequest request)
    {
        // For demonstration purposes, a simple calculation is used
        decimal monthlyInterestRate = request.InterestRate / 12 / 100;
        int totalPayments = request.RepaymentPeriodYears * 12;
        decimal monthlyPayment = (request.LoanAmount * monthlyInterestRate) /
                                  (1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -totalPayments));
        return (int)Math.Floor(monthlyPayment);
    }
}
