using BankRUs.CreditScoreService.Models;
using BankRUs.CreditScoreService.Repositories;
using BankRUs.CreditScoreService.Validators;

namespace BankRUs.CreditScoreService.Services;

public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
{
    public async Task<Customer?> CreateCustomerAsync(string socialSecurityNumber, string name, decimal annualIncome, decimal totalDebt)
    {
        CustomerValidator.ValidateNewCustomer(socialSecurityNumber, name, annualIncome, totalDebt);

        return await customerRepository.CreateAsync(new Customer
        {
            SocialSecurityNumber = socialSecurityNumber,
            Name = name.Trim(),
            AnnualIncome = annualIncome,
            TotalDebt = totalDebt
        });
    }

    public async Task<Customer?> UpdateCustomerFinancesAsync(string socialSecurityNumber, decimal annualIncome, decimal totalDebt)
    {
        CustomerValidator.ValidateFinances(annualIncome, totalDebt);

        return await customerRepository.UpdateFinancesAsync(socialSecurityNumber, annualIncome, totalDebt);
    }
}
