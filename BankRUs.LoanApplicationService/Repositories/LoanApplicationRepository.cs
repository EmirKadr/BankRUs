using BankRUs.LoanApplicationService.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BankRUs.LoanApplicationService.Repositories;

public class LoanApplicationRepository(IConfiguration configuration) : ILoanApplicationRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Anslutningssträngen 'Default' saknas.");

    public async Task<LoanApplication> CreateAsync(LoanApplication loanApplication)
    {
        const string sql = """
            INSERT INTO dbo.LoanApplications (SocialSecurityNumber, Amount, Status)
            OUTPUT INSERTED.Id
            VALUES (@SocialSecurityNumber, @Amount, @Status);
            """;

        using var connection = new SqlConnection(_connectionString);

        loanApplication.Id = await connection.QuerySingleAsync<int>(sql, new
        {
            loanApplication.SocialSecurityNumber,
            loanApplication.Amount,
            Status = loanApplication.Status.ToString()
        });

        return loanApplication;
    }
}
