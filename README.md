# BankRUs

Bank-R-Us interna tjänster – två ASP.NET Core Web API:er (Minimal APIs) i samma solution:

- **BankRUs.CreditScoreService** – beräknar en kunds kreditvärdighet (0–100) utifrån årsinkomst och total skuld.
- **BankRUs.LoanApplicationService** – tar emot låneansökningar och godkänner eller avslår dem. Kreditvärdet hämtas över HTTP från CreditScoreService.

```
Klient ──POST /api/loanapplications──► LoanApplicationService
                                          │ GET /api/customers/{ssn}/credit-score
                                          ▼
                                      CreditScoreService
```

## Endpoints

**CreditScoreService** (`https://localhost:8000`, `http://localhost:9000`)

| Metod | Endpoint | Beskrivning |
|---|---|---|
| GET | `/api/customers/{socialSecurityNumber}/credit-score` | Kundens kreditvärde. `404` om kunden inte finns. |
| POST | `/api/customers` | Lägg till kund (`socialSecurityNumber`, `name`, `annualIncome`, `totalDebt`). `201` med kreditvärde, `409` om kunden finns, `400` vid ogiltig data. |
| PUT | `/api/customers/{socialSecurityNumber}` | Uppdatera `annualIncome` och `totalDebt`. `200` med nytt kreditvärde, `404` om kunden saknas, `400` vid ogiltig data. |

**LoanApplicationService** (`https://localhost:8001`, `http://localhost:9001`)

| Metod | Endpoint | Beskrivning |
|---|---|---|
| POST | `/api/loanapplications` | Låneansökan (`socialSecurityNumber`, `amount`). `201` med `id` och `status` (`Approved` om kreditvärdet är minst 50, annars `Declined`), `400` vid ogiltig data. |

## Kreditberäkning

`CreditScore = 100 - (TotalDebt / AnnualIncome * 100)`, avrundat och begränsat till 0–100. Kund utan årsinkomst får 0. Okänd kund i en låneansökan behandlas som kreditvärde 0.

## Koppling mellan tjänsterna

LoanApplicationService har två implementationer av `ICreditScoreService`:

- `HttpCreditScoreService` – anropar CreditScoreService. Används när `CreditScoreService:BaseUrl` är satt.
- `FakeCreditScoreService` – returnerar alltid 75. Används när adressen saknas, så tjänsten kan köras fristående.

## Teknik

- ASP.NET Core Web API (.NET 10), OpenAPI
- Dapper + SQL Server LocalDB/Express
- Interfaces och dependency injection (services och repositories)
- Azure App Service – i Production används in-memory-repositories (Singleton)

## Kom igång

1. Skapa databasen genom att köra `BankRUs.CreditScoreService/Data/CreateDatabase.sql` och `BankRUs.LoanApplicationService/Data/CreateDatabase.sql`.
2. Sätt anslutningssträngen med User Secrets (projekten delar samma User Secrets):
   ```
   dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\MSSQLLocalDB;Database=BankRUsDb;Trusted_Connection=True;" --project BankRUs.CreditScoreService
   ```
3. Starta båda tjänsterna (i Visual Studio: *Configure Startup Projects → Multiple startup projects*):
   ```
   dotnet run --project BankRUs.CreditScoreService
   dotnet run --project BankRUs.LoanApplicationService
   ```
   Testanrop finns i respektive projekts `.http`-fil.

## Azure

Varje tjänst är en egen App Service. Sätt app-inställningen `CreditScoreService__BaseUrl` på LoanApplicationService till CreditScoreService-adressen.
