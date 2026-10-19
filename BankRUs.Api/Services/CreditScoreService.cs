using BankRUs.Api.Repositories;

namespace BankRUs.Api.Services;

public class CreditScoreService(ICustomerRepository customerRepository) : ICreditScoreService
{
    public async Task<int?> GetCreditScoreAsync(string socialSecurityNumber)
    {
        var customer = await customerRepository.GetBySocialSecurityNumberAsync(socialSecurityNumber);

        return customer is null ? null : CreditScoreCalculator.Calculate(customer);
    }
}
