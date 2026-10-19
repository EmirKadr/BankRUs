# BankRUs

Bank-R-Us interna tjänster samlade i ett ASP.NET Core Web API (Minimal APIs):

- **Credit Score Service** – beräknar en kunds kreditvärdighet (0–100) utifrån årsinkomst och total skuld.
- **Loan Application Service** – tar emot låneansökningar och godkänner eller avslår dem utifrån kundens kreditvärde.

## Endpoints

| Metod | Endpoint | Beskrivning |
|---|---|---|
| GET | `/api/customers/{socialSecurityNumber}/credit-score` | Kundens kreditvärde. `404` om kunden inte finns. |
| POST | `/api/customers` | Lägg till kund (`socialSecurityNumber`, `name`, `annualIncome`, `totalDebt`). `201` med kreditvärde, `409` om kunden finns, `400` vid ogiltig data. |
| PUT | `/api/customers/{socialSecurityNumber}` | Uppdatera `annualIncome` och `totalDebt`. `200` med nytt kreditvärde, `404` om kunden saknas, `400` vid ogiltig data. |
| POST | `/api/loanapplications` | Låneansökan (`socialSecurityNumber`, `amount`). `201` med `id` och `status` (`Approved` om kreditvärdet är minst 50, annars `Declined`), `400` vid ogiltig data. |

## Kreditberäkning

`CreditScore = 100 - (TotalDebt / AnnualIncome * 100)`, avrundat och begränsat till 0–100. Kund utan årsinkomst får 0. Okänd kund i en låneansökan behandlas som kreditvärde 0.

## Teknik

- ASP.NET Core Web API (.NET 10), OpenAPI
- Dapper + SQL Server LocalDB/Express
- Interfaces och dependency injection (services och repositories)
- Azure App Service – i Production används in-memory-repositories (Singleton)

## Kom igång

1. Skapa databasen genom att köra `BankRUs.Api/Data/CreateDatabase.sql`.
2. Sätt anslutningssträngen med User Secrets:
   ```
   dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\MSSQLLocalDB;Database=BankRUsDb;Trusted_Connection=True;" --project BankRUs.Api
   ```
3. Kör applikationen:
   ```
   dotnet run --project BankRUs.Api
   ```
   HTTPS: `https://localhost:8000`, HTTP: `http://localhost:9000`. Testanrop finns i `BankRUs.Api/BankRUs.Api.http`.
