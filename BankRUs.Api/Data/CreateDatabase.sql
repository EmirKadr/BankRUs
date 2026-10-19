IF DB_ID('BankRUsDb') IS NULL
    CREATE DATABASE BankRUsDb;
GO

USE BankRUsDb;
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NULL
CREATE TABLE dbo.Customers
(
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    SocialSecurityNumber NVARCHAR(13)  NOT NULL UNIQUE,
    Name                 NVARCHAR(100) NOT NULL,
    AnnualIncome         DECIMAL(18,2) NOT NULL,
    TotalDebt            DECIMAL(18,2) NOT NULL
);
GO

IF OBJECT_ID('dbo.LoanApplications', 'U') IS NULL
CREATE TABLE dbo.LoanApplications
(
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    SocialSecurityNumber NVARCHAR(13) NOT NULL,
    Amount               INT          NOT NULL CHECK (Amount > 0),
    Status               NVARCHAR(20) NOT NULL CHECK (Status IN ('Approved', 'Declined'))
);
GO

-- Testdata (fiktiva personnummer)
IF NOT EXISTS (SELECT 1 FROM dbo.Customers)
INSERT INTO dbo.Customers (SocialSecurityNumber, Name, AnnualIncome, TotalDebt) VALUES
    ('19900101-2010', N'Anna Svensson',  520000, 150000),
    ('19850515-1234', N'Erik Johansson', 360000, 180000),
    ('19780320-5678', N'Maria Lindberg', 680000, 1200000);
GO
