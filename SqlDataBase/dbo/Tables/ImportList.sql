CREATE TABLE [dbo].[ImportList]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1), 
    [KontragentiId] BIGINT NULL, 
    [Date] DATE NULL, 
    [Number] BIGINT NULL, 
    [DO] DECIMAL(18, 2) NULL, 
    [DDS] DECIMAL(18, 2) NULL, 
    [FullValue] DECIMAL(18, 2) NULL, 
    [AccountingStatusId] INT NULL, 
    [NameAndEik] NVARCHAR(50) NULL 
)
