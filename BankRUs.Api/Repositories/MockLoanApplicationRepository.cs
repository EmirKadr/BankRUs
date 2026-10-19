using System.Collections.Concurrent;
using BankRUs.Api.Models;

namespace BankRUs.Api.Repositories;

// Lagrar låneansökningar i minnet. Datan försvinner när applikationen startas om.
public class MockLoanApplicationRepository : ILoanApplicationRepository
{
    private readonly ConcurrentDictionary<int, LoanApplication> _loanApplications = new();
    private int _lastId;

    public MockLoanApplicationRepository()
    {
        // Påhittade ansökningar för samma kunder som i MockCustomerRepository
        Seed("19900101-2010", 100000, LoanApplicationStatus.Approved); // Anna Svensson (71)
        Seed("19850515-1234", 50000, LoanApplicationStatus.Approved);  // Erik Johansson (50)
        Seed("19780320-5678", 750000, LoanApplicationStatus.Declined); // Maria Lindberg (0)
    }

    public Task<LoanApplication> CreateAsync(LoanApplication loanApplication)
    {
        loanApplication.Id = Interlocked.Increment(ref _lastId);
        _loanApplications[loanApplication.Id] = loanApplication;

        return Task.FromResult(loanApplication);
    }

    private void Seed(string socialSecurityNumber, int amount, LoanApplicationStatus status) =>
        CreateAsync(new LoanApplication
        {
            SocialSecurityNumber = socialSecurityNumber,
            Amount = amount,
            Status = status
        });
}
