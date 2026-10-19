using BankRUs.Api.DTOs;
using BankRUs.Api.Repositories;
using BankRUs.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICreditScoreService, CreditScoreService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();

// Lokalt (Development): SQL Server. I Azure: in-memory, Singleton så att datan lever mellan anrop.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
}
else
{
    builder.Services.AddSingleton<ICustomerRepository, MockCustomerRepository>();
    builder.Services.AddSingleton<ILoanApplicationRepository, MockLoanApplicationRepository>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ---------- Kunder och kreditvärde ----------

app.MapGet("/api/customers/{socialSecurityNumber}/credit-score", async (string socialSecurityNumber, ICreditScoreService creditScoreService) =>
{
    var creditScore = await creditScoreService.GetCreditScoreAsync(socialSecurityNumber);

    return creditScore is null
        ? Results.NotFound(new { error = $"Kunden med personnummer {socialSecurityNumber} finns inte." })
        : Results.Ok(new CreditScoreResponse(socialSecurityNumber, creditScore.Value));
});

app.MapPost("/api/customers", async (CreateCustomerRequest request, ICustomerService customerService) =>
{
    try
    {
        var customer = await customerService.CreateCustomerAsync(
            request.SocialSecurityNumber,
            request.Name,
            request.AnnualIncome,
            request.TotalDebt);

        if (customer is null)
            return Results.Conflict(new { error = $"Kunden med personnummer {request.SocialSecurityNumber} finns redan." });

        var response = new CreditScoreResponse(customer.SocialSecurityNumber, CreditScoreCalculator.Calculate(customer));

        return Results.Created($"/api/customers/{customer.SocialSecurityNumber}/credit-score", response);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/customers/{socialSecurityNumber}", async (string socialSecurityNumber, UpdateCustomerRequest request, ICustomerService customerService) =>
{
    try
    {
        var customer = await customerService.UpdateCustomerFinancesAsync(
            socialSecurityNumber,
            request.AnnualIncome,
            request.TotalDebt);

        return customer is null
            ? Results.NotFound(new { error = $"Kunden med personnummer {socialSecurityNumber} finns inte." })
            : Results.Ok(new CreditScoreResponse(customer.SocialSecurityNumber, CreditScoreCalculator.Calculate(customer)));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// ---------- Låneansökningar ----------

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
