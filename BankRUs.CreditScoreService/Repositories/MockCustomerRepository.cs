using System.Collections.Concurrent;
using BankRUs.CreditScoreService.Models;

namespace BankRUs.CreditScoreService.Repositories;

// Lagrar kunduppgifter i minnet. Datan försvinner när applikationen startas om.
public class MockCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<string, Customer> _customers = new(StringComparer.Ordinal);
    private int _lastId;

    public MockCustomerRepository()
    {
        Seed("19900101-2010", "Anna Svensson", 520000, 150000);
        Seed("19850515-1234", "Erik Johansson", 360000, 180000);
        Seed("19780320-5678", "Maria Lindberg", 680000, 1200000);
    }

    public Task<Customer?> GetBySocialSecurityNumberAsync(string socialSecurityNumber) =>
        Task.FromResult(_customers.GetValueOrDefault(socialSecurityNumber));

    public Task<Customer?> CreateAsync(Customer customer)
    {
        if (_customers.ContainsKey(customer.SocialSecurityNumber))
            return Task.FromResult<Customer?>(null);

        customer.Id = Interlocked.Increment(ref _lastId);

        return Task.FromResult(_customers.TryAdd(customer.SocialSecurityNumber, customer) ? customer : null);
    }

    public Task<Customer?> UpdateFinancesAsync(string socialSecurityNumber, decimal annualIncome, decimal totalDebt)
    {
        if (!_customers.TryGetValue(socialSecurityNumber, out var existing))
            return Task.FromResult<Customer?>(null);

        // Ersätt med en ny instans i stället för att ändra den delade, så att samtidiga anrop inte ser halvuppdaterad data
        var updated = new Customer
        {
            Id = existing.Id,
            SocialSecurityNumber = existing.SocialSecurityNumber,
            Name = existing.Name,
            AnnualIncome = annualIncome,
            TotalDebt = totalDebt
        };
        _customers[socialSecurityNumber] = updated;

        return Task.FromResult<Customer?>(updated);
    }

    private void Seed(string socialSecurityNumber, string name, decimal annualIncome, decimal totalDebt) =>
        CreateAsync(new Customer
        {
            SocialSecurityNumber = socialSecurityNumber,
            Name = name,
            AnnualIncome = annualIncome,
            TotalDebt = totalDebt
        });
}
