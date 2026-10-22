using BankRUs.LoanApplicationService.Models;

namespace BankRUs.LoanApplicationService.Services;

public interface ILoanApplicationService
{
    Task<LoanApplication> CreateLoanApplicationAsync(string socialSecurityNumber, int amount);
}
