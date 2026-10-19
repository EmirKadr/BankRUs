using BankRUs.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BankRUs.Api.Repositories;

public class CustomerRepository(IConfiguration configuration) : ICustomerRepository
{
    // SQL Server-felkoder för brott mot UNIQUE-villkor
    private const int UniqueConstraintViolation = 2627;
    private const int UniqueIndexViolation = 2601;

    private readonly string _connectionString = configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Anslutningssträngen 'Default' saknas.");

    public async Task<Customer?> GetBySocialSecurityNumberAsync(string socialSecurityNumber)
    {
        const string sql = """
            SELECT Id, SocialSecurityNumber, Name, AnnualIncome, TotalDebt
            FROM dbo.Customers
            WHERE SocialSecurityNumber = @SocialSecurityNumber;
            """;

        using var connection = new SqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { SocialSecurityNumber = socialSecurityNumber });
    }

    public async Task<Customer?> CreateAsync(Customer customer)
    {
        const string sql = """
            INSERT INTO dbo.Customers (SocialSecurityNumber, Name, AnnualIncome, TotalDebt)
            OUTPUT INSERTED.Id
            VALUES (@SocialSecurityNumber, @Name, @AnnualIncome, @TotalDebt);
            """;

        using var connection = new SqlConnection(_connectionString);

        try
        {
            customer.Id = await connection.QuerySingleAsync<int>(sql, customer);
            return customer;
        }
        catch (SqlException ex) when (ex.Number is UniqueConstraintViolation or UniqueIndexViolation)
        {
            return null;
        }
    }

    public async Task<Customer?> UpdateFinancesAsync(string socialSecurityNumber, decimal annualIncome, decimal totalDebt)
    {
        const string sql = """
            UPDATE dbo.Customers
            SET AnnualIncome = @AnnualIncome, TotalDebt = @TotalDebt
            OUTPUT INSERTED.Id, INSERTED.SocialSecurityNumber, INSERTED.Name, INSERTED.AnnualIncome, INSERTED.TotalDebt
            WHERE SocialSecurityNumber = @SocialSecurityNumber;
            """;

        using var connection = new SqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new
        {
            SocialSecurityNumber = socialSecurityNumber,
            AnnualIncome = annualIncome,
            TotalDebt = totalDebt
        });
    }
}
