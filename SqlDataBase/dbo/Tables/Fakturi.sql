CREATE TABLE [dbo].[Fakturi]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DocPayableReceivableId] INT NULL,
    [KontragentiId] BIGINT NULL, 
    [AccDate] DATE NULL,
    [Date] DATE NULL, 
    [Number] BIGINT NULL, 
    [DO] DECIMAL(18, 2) NULL, 
    [DDS] DECIMAL(18, 2) NULL, 
    [FullValue] DECIMAL(18, 2) NULL, 
    [DealKindId] INT NULL, 
    [DocTypeId] INT NULL, 
    [Account] INT NULL,
    [InCashAccount] INT NULL,
    [Note] NVARCHAR(50) NULL,
    [Image] IMAGE NULL,
    [AccountingStatusId] INT NULL, 
    [PurchaseOrSale] NVARCHAR(20) NULL
)
