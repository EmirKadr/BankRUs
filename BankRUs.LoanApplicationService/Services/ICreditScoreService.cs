namespace BankRUs.LoanApplicationService.Services;

public interface ICreditScoreService
{
    Task<int> GetCreditScoreAsync(string socialSecurityNumber);
}
