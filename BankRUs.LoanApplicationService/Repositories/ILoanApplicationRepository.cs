using BankRUs.LoanApplicationService.Models;

namespace BankRUs.LoanApplicationService.Repositories;

public interface ILoanApplicationRepository
{
    Task<LoanApplication> CreateAsync(LoanApplication loanApplication);
}
