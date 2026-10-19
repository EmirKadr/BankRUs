using System.Text.RegularExpressions;

namespace BankRUs.Api.Validators;

// Delas av kund- och låneansökningsvalideringen så att regeln bara finns på ett ställe
public static partial class SocialSecurityNumberValidator
{
    // Format: YYYYMMDD-XXXX
    [GeneratedRegex(@"^\d{8}-\d{4}$")]
    private static partial Regex Pattern();

    public static void Validate(string? socialSecurityNumber)
    {
        if (string.IsNullOrWhiteSpace(socialSecurityNumber) || !Pattern().IsMatch(socialSecurityNumber))
            throw new ArgumentException("Personnummer måste ha formatet YYYYMMDD-XXXX.");
    }
}
