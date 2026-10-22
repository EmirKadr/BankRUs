using System.Net;

namespace BankRUs.LoanApplicationService.Services;

// Hämtar kreditvärdet från BankRUs.CreditScoreService över HTTP
public class HttpCreditScoreService(HttpClient httpClient) : ICreditScoreService
{
    private record CreditScoreResponse(string SocialSecurityNumber, int CreditScore);

    public async Task<int> GetCreditScoreAsync(string socialSecurityNumber)
    {
        using var response = await httpClient.GetAsync($"api/customers/{Uri.EscapeDataString(socialSecurityNumber)}/credit-score");

        // Okänd kund saknar kredithistorik och behandlas som lägsta kreditvärde
        if (response.StatusCode == HttpStatusCode.NotFound)
            return 0;

        response.EnsureSuccessStatusCode();

        var creditScore = await response.Content.ReadFromJsonAsync<CreditScoreResponse>();

        return creditScore?.CreditScore ?? 0;
    }
}
