using BankRUs.LoanApplicationService.DTOs;
using BankRUs.LoanApplicationService.Repositories;
using BankRUs.LoanApplicationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();

// Riktiga kreditupplysningstjänsten om dess adress är konfigurerad, annars den simulerade
var creditScoreServiceUrl = builder.Configuration["CreditScoreService:BaseUrl"];
if (!string.IsNullOrWhiteSpace(creditScoreServiceUrl))
{
    builder.Services.AddHttpClient<ICreditScoreService, HttpCreditScoreService>(client =>
        client.BaseAddress = new Uri(creditScoreServiceUrl.TrimEnd('/') + "/"));
}
else
{
    builder.Services.AddScoped<ICreditScoreService, FakeCreditScoreService>();
}

// Lokalt (Development): SQL Server. I Azure: in-memory, Singleton så att datan lever mellan anrop.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
}
else
{
    builder.Services.AddSingleton<ILoanApplicationRepository, MockLoanApplicationRepository>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/api/loanapplications", async (CreateLoanApplicationRequest request, ILoanApplicationService loanApplicationService) =>
{
    try
    {
        var loanApplication = await loanApplicationService.CreateLoanApplicationAsync(
            request.SocialSecurityNumber,
            request.Amount);

        var response = new LoanApplicationResponse(loanApplication.Id, loanApplication.Status.ToString());

        return Results.Created($"/api/loanapplications/{loanApplication.Id}", response);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();
