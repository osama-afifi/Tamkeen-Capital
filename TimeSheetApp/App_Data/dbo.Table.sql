CREATE TABLE [dbo].[SystemUsers]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL , 
    [Email] NCHAR(200) NOT NULL, 
    [Password] NCHAR(200) NULL, 
    [PasswordSalt] NCHAR(100) NULL, 
    [FirstName] NCHAR(100) NULL, 
    [LastName] NCHAR(100) NULL, 
    [Admin] BIT NULL, 
    [Mobile] NCHAR(15) NULL, 
    [LastLogin] DATETIME NULL, 
    PRIMARY KEY ([Email])
)
