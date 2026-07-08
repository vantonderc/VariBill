CREATE TABLE [dbo].[Product]
(
	[Id]			UNIQUEIDENTIFIER NOT NULL  DEFAULT NEWID(),--TODO:IDentity(1,1)
	[ProductTypeId] UNIQUEIDENTIFIER NOT NULL,
	[Name]          NVARCHAR(200)    NOT NULL,
	[SKU]           NVARCHAR(50)     NOT NULL,  
	[Price]         DECIMAL(18, 2)   NOT NULL,
	[Quantity]      INT              NOT NULL DEFAULT 0,  
    [Description]   NVARCHAR(1000)   NULL,
    [IsActive]      BIT              NOT NULL DEFAULT 1,   
    [IsDeleted]     BIT              NOT NULL DEFAULT 0,  
	[DateCreated]	DATETIME	     NOT NULL DEFAULT GETUTCDATE(),
	[DateModified]	DATETIME	     NULL,

	-- Primary & Declarative Constraints
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Product_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [UQ_Product_SKU] UNIQUE NONCLUSTERED ([SKU] ASC),  
    CONSTRAINT [CK_Product_Price_Positive] CHECK ([Price] > 0),

    -- Referential Integrity
    CONSTRAINT [FK_Product_ProductType_ProductTypeId] FOREIGN KEY ([ProductTypeId])
        REFERENCES [dbo].[ProductType] ([Id])
        ON DELETE NO ACTION
        ON UPDATE NO ACTION


	
);
GO
-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Product_ProductTypeId] ON [dbo].[Product] ([ProductTypeId]);