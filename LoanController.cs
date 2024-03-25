using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoanController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanController(ILoanService loanService)
    {
        _loanService = loanService;
    }
    [HttpPost("calculateMonthlyPayment")]
    public ActionResult<decimal> CalculateLoanPayment([FromBody] LoanCalculationRequest request)
    {
        // Perform loan payment calculation logic here based on request parameters
        // Example calculation (simplified):
        decimal monthlyInterestRate = request.InterestRate / 12 / 100;
        int totalPayments = request.RepaymentPeriod * 12;
        decimal monthlyPayment = CalculateMonthlyPayment(request.LoanAmount, monthlyInterestRate, totalPayments);
        
        return Ok(new { monthlyPayment });
    }

    private decimal CalculateMonthlyPayment(decimal loanAmount, decimal monthlyInterestRate, int totalPayments)
    {
        decimal monthlyPayment = loanAmount * (monthlyInterestRate / (1 - (decimal)Math.Pow(1 + (double)monthlyInterestRate, -totalPayments)));
        return monthlyPayment;
    }
}

public class LoanCalculationRequest
{
    public decimal LoanAmount { get; set; }
    public decimal InterestRate { get; set; }
    public int RepaymentPeriod { get; set; }
    public string PaymentPlan { get; set; }
}
