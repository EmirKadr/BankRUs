namespace BankRUs.Api.Validators;

public static class LoanApplicationValidator
{
    public static void Validate(string? socialSecurityNumber, int amount)
    {
        SocialSecurityNumberValidator.Validate(socialSecurityNumber);

        if (amount <= 0)
            throw new ArgumentException("Lånebeloppet måste vara större än 0.");
    }
}
