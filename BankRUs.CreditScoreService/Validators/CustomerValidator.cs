namespace BankRUs.CreditScoreService.Validators;

public static class CustomerValidator
{
    public static void ValidateNewCustomer(string? socialSecurityNumber, string? name, decimal annualIncome, decimal totalDebt)
    {
        SocialSecurityNumberValidator.Validate(socialSecurityNumber);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Namn måste anges.");

        ValidateFinances(annualIncome, totalDebt);
    }

    public static void ValidateFinances(decimal annualIncome, decimal totalDebt)
    {
        if (annualIncome < 0)
            throw new ArgumentException("Årsinkomsten får inte vara negativ.");

        if (totalDebt < 0)
            throw new ArgumentException("Den totala skulden får inte vara negativ.");
    }
}
