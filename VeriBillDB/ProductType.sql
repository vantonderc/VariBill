--CREATE TABLE [dbo].[ProductType]
--(
--	[Id]			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(), 
--    [Name]			NVARCHAR(100) NOT NULL, 
--    [Description]	NVARCHAR(500) NULL,
--	--CONSTRAINT	[PK_ProductType]	PRIMARY KEY CLUSTERED ([Id] ASC)		


	
--)

CREATE TABLE [dbo].[ProductType]
(
    [Id]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Name]          NVARCHAR(100) NOT NULL,
    [Description]   NVARCHAR(500) NULL,
    [IsActive]      BIT NOT NULL DEFAULT 1,
    [IsDeleted]     BIT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_ProductType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);