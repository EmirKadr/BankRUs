using BankRUs.Api.Models;
using BankRUs.Api.Repositories;
using BankRUs.Api.Validators;

namespace BankRUs.Api.Services;

public class LoanApplicationService(
    ICreditScoreService creditScoreService,
    ILoanApplicationRepository loanApplicationRepository) : ILoanApplicationService
{
    private const int MinimumCreditScore = 50;

    public async Task<LoanApplication> CreateLoanApplicationAsync(string socialSecurityNumber, int amount)
    {
        LoanApplicationValidator.Validate(socialSecurityNumber, amount);

        // Okänd kund saknar kredithistorik och behandlas som lägsta kreditvärde
        var creditScore = await creditScoreService.GetCreditScoreAsync(socialSecurityNumber) ?? 0;

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
