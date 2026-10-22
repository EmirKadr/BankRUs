using BankRUs.CreditScoreService.Models;

namespace BankRUs.CreditScoreService.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetBySocialSecurityNumberAsync(string socialSecurityNumber);

    // Returnerar null om en kund med samma personnummer redan finns
    Task<Customer?> CreateAsync(Customer customer);

    // Returnerar null om kunden inte finns
    Task<Customer?> UpdateFinancesAsync(string socialSecurityNumber, decimal annualIncome, decimal totalDebt);
}
