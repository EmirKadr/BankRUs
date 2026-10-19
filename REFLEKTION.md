# Reflektionsfrågor – Credit Score Service

**1. Hur har du använt interfaces och dependency injection för att skapa löst kopplad kod?**
`CreditScoreService` beror på `ICustomerRepository`, och endpointen beror på `ICreditScoreService` – aldrig på konkreta klasser. Implementationerna registreras i `Program.cs` och skickas in via konstruktorn av DI-containern. Ingen klass skapar sina egna beroenden med `new`.

**2. Vilket ansvar har ICustomerRepository respektive ICreditScoreService?**
- `ICustomerRepository`: dataåtkomst – hämta en kund utifrån personnummer (eller `null` om den inte finns).
- `ICreditScoreService`: affärslogik – räkna ut kreditvärdet utifrån kundens årsinkomst och skuld.

**3. Varför bör beräkningen av kreditvärdet ligga i en service och inte i repositoryt?**
Beräkningen är en affärsregel, inte dataåtkomst. Ligger den i servicen gäller samma regel oavsett var datan kommer ifrån (SQL Server eller minnet), den behöver inte dupliceras i varje repository, och den kan testas utan databas.

**4. Hur kan du byta ut implementationen av ICustomerRepository utan att ändra koden i CreditScoreService?**
Genom att ändra registreringen i `Program.cs`. Lokalt (Development) registreras `CustomerRepository` (SQL Server/Dapper), i Azure (Production) `MockCustomerRepository` (minnet). Båda implementerar `ICustomerRepository`, så `CreditScoreService` påverkas inte.

**5. Hur har du tillämpat Separation of Concerns och Single Responsibility Principle (SRP)?**
Applikationen är uppdelad i endpoints (HTTP och statuskoder), DTO:er (API-kontraktet), validator (indataregler), `CreditScoreCalculator` (beräkningen), `CreditScoreService` (hämta kreditvärde), `CustomerService` (lägga till och uppdatera kunder), repository (dataåtkomst) och modell (kunden). Varje klass har en anledning att ändras – t.ex. ändras `CustomerRepository` bara om lagringen ändras och `CreditScoreService` bara om beräkningsreglerna ändras.

**6. Vilka fördelar finns med att använda DTO i stället för att returnera databasentiteter direkt från API?**
API:et exponerar bara det som behövs (`socialSecurityNumber`, `creditScore`) – inte kundens namn, inkomst, skuld eller databas-ID. Databasen kan ändras utan att API-kontraktet bryts, och känslig data läcker inte ut av misstag.

**7. Hur skulle du kunna testa kreditberäkningen utan att ansluta till en riktig databas?**
Beräkningen ligger i den statiska klassen `CreditScoreCalculator`, som bara tar en `Customer` och inte har några beroenden – den kan testas direkt med påhittade kunder. För att testa hela `CreditScoreService` kan man ge den en fejkad `ICustomerRepository` i ett enhetstest – t.ex. `MockCustomerRepository` eller en mock från ett bibliotek som Moq/NSubstitute – med kunder vars förväntade poäng är kända (Anna 71, Erik 50, Maria 0, en kund utan inkomst 0, en okänd kund `null`).
