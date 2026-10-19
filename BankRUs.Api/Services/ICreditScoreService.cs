namespace BankRUs.Api.Services;

public interface ICreditScoreService
{
    // Returnerar null om kunden inte finns
    Task<int?> GetCreditScoreAsync(string socialSecurityNumber);
}
