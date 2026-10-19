namespace BankRUs.Api.DTOs;

public record CreateLoanApplicationRequest(string SocialSecurityNumber, int Amount);
