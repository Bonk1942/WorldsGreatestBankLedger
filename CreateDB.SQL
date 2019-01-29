USE [master]
GO

--CREATE DB
DROP DATABASE IF EXISTS WorldsGreatestBankingLedger
GO

CREATE DATABASE WorldsGreatestBankingLedger
GO

USE WorldsGreatestBankingLedger
GO

--CREATE TABLES
DROP TABLE IF EXISTS dbo.Account
GO
CREATE TABLE dbo.Account(
	ID int IDENTITY(1,1) PRIMARY KEY,
	UserName nvarchar(100) NOT NULL UNIQUE,
	FirstName nvarchar(50) NOT NULL,
	LastName nvarchar(50) NOT NULL,
	PW nvarchar(255) NOT NULL --hashed 
);
GO


DROP TABLE IF EXISTS dbo.Transactions
GO
CREATE TABLE dbo.Transactions(
	ID int IDENTITY(1,1),
	UserName nvarchar(100) NOT NULL, 
	Debit money DEFAULT 0.00 CHECK(Debit >= 0.00), --no negative numbers
	Credit money DEFAULT 0.00 CHECK(Credit >= 0.00), --no negative numbers
	TranDate date DEFAULT GETDATE(),
	PRIMARY KEY (ID),
	CONSTRAINT FK_USERNAME FOREIGN KEY (UserName) REFERENCES Account(UserName)
);
GO

--CREATE VIEWS
DROP VIEW IF EXISTS dbo.vw_Login
GO
CREATE VIEW vw_Login AS
SELECT UserName, PW FROM Account
GO


DROP VIEW IF EXISTS dbo.vw_AccountInfo
GO
CREATE VIEW vw_AccountInfo AS
SELECT UserName, FirstName, LastName FROM Account
GO


DROP VIEW IF EXISTS dbo.vw_TranHist
GO
Create View vw_TranHist AS
SELECT UserName, TranDate, Debit, Credit 
FROM Transactions
GO

--SPROCS
DROP PROCEDURE IF EXISTS sp_CreateAccount
GO
CREATE PROCEDURE sp_CreateAccount
	@UserName nvarchar(100),
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@PW nvarchar(255)
AS
BEGIN
	INSERT INTO Account
           (UserName,FirstName,LastName,PW)
     VALUES
           (@UserName,@FirstName,@LastName,@PW)
END
GO


DROP PROCEDURE IF EXISTS sp_Deposit
GO 
CREATE PROCEDURE sp_Deposit 
	@UserName nvarchar(100), 
	@Credit money
AS
BEGIN
	INSERT INTO Transactions
	(UserName, Credit)
	VALUES (@UserName, @Credit)
END
GO


DROP PROCEDURE IF EXISTS sp_Withdraw
GO
CREATE PROCEDURE sp_Withdraw
	@UserName nvarchar(100), 
	@Debit money
AS
BEGIN
	INSERT INTO Transactions
	(UserName, Debit)
	VALUES (@UserName, @Debit)
END
GO