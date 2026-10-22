using BankRUs.LoanApplicationService.Models;
using BankRUs.LoanApplicationService.Repositories;
using BankRUs.LoanApplicationService.Validators;

namespace BankRUs.LoanApplicationService.Services;

public class LoanApplicationService(
    ICreditScoreService creditScoreService,
    ILoanApplicationRepository loanApplicationRepository) : ILoanApplicationService
{
    private const int MinimumCreditScore = 50;

    public async Task<LoanApplication> CreateLoanApplicationAsync(string socialSecurityNumber, int amount)
    {
        LoanApplicationValidator.Validate(socialSecurityNumber, amount);

        var creditScore = await creditScoreService.GetCreditScoreAsync(socialSecurityNumber);

        var loanApplication = new LoanApplication
        {
            SocialSecurityNumber = socialSecurityNumber,
            Amount = amount,
            Status = creditScore >= MinimumCreditScore
                ? LoanApplicationStatus.Approved
                : LoanApplicationStatus.Declined
        };

        return await loanApplicationRepository.CreateAsync(loanApplication);
    }
}
