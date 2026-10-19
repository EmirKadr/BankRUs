using BankRUs.Api.Models;

namespace BankRUs.Api.Services;

public interface ILoanApplicationService
{
    Task<LoanApplication> CreateLoanApplicationAsync(string socialSecurityNumber, int amount);
}
