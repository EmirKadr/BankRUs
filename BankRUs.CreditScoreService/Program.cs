using BankRUs.CreditScoreService.DTOs;
using BankRUs.CreditScoreService.Repositories;
using BankRUs.CreditScoreService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICreditScoreService, CreditScoreService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Lokalt (Development): SQL Server. I Azure: in-memory, Singleton så att datan lever mellan anrop.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
}
else
{
    builder.Services.AddSingleton<ICustomerRepository, MockCustomerRepository>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

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

app.Run();
