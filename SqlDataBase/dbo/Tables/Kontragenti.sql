CREATE TABLE [dbo].[Kontragenti]
(
	[Id] INT NOT NULL  IDENTITY, 
    [Name] NVARCHAR(50) NULL, 
    [EIK] BIGINT NULL, 
    [DDSNumber] NVARCHAR(50) NULL, 
    PRIMARY KEY ([Id])
)
