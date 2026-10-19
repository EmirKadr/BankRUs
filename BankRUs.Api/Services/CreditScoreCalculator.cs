using BankRUs.Api.Models;

namespace BankRUs.Api.Services;

// Ren beräkning utan beroenden – kan testas utan databas.
public static class CreditScoreCalculator
{
    private const int MinCreditScore = 0;
    private const int MaxCreditScore = 100;

    public static int Calculate(Customer customer)
    {
        // Kund utan årsinkomst får 0 poäng (och vi undviker division med 0)
        if (customer.AnnualIncome <= 0)
            return MinCreditScore;

        var creditScore = MaxCreditScore - (customer.TotalDebt / customer.AnnualIncome * MaxCreditScore);

        return (int)Math.Clamp(Math.Round(creditScore, MidpointRounding.AwayFromZero), MinCreditScore, MaxCreditScore);
    }
}
