using BankRUs.Api.Models;

namespace BankRUs.Api.Repositories;

public interface ILoanApplicationRepository
{
    Task<LoanApplication> CreateAsync(LoanApplication loanApplication);
}
