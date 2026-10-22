namespace BankRUs.LoanApplicationService.Services;

// Tillfällig implementation som simulerar kreditupplysningstjänsten, så att låneflödet kan testas fristående
public class FakeCreditScoreService : ICreditScoreService
{
    public async Task<int> GetCreditScoreAsync(string socialSecurityNumber)
    {
        // Simulerar ett anrop till en extern tjänst
        await Task.Delay(500);

        // Returnerar ett påhittat kreditvärde
        return 75;
    }
}
