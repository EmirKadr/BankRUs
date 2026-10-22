namespace BankRUs.CreditScoreService.Models;

public class Customer
{
    public int Id { get; set; }
    public required string SocialSecurityNumber { get; set; }
    public required string Name { get; set; }
    public decimal AnnualIncome { get; set; }
    public decimal TotalDebt { get; set; }
}
