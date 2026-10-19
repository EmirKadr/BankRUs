namespace BankRUs.Api.Models;

public class LoanApplication
{
    public int Id { get; set; }
    public required string SocialSecurityNumber { get; set; }
    public required int Amount { get; set; }
    public required LoanApplicationStatus Status { get; set; }
}
