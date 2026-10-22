namespace BankRUs.LoanApplicationService.DTOs;

public record CreateLoanApplicationRequest(string SocialSecurityNumber, int Amount);
