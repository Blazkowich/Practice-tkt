CREATE TABLE [tkt].[PriceCategory]
(
    [PriceCategoryId] TINYINT      NOT NULL,
    [Name]            NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_PriceCategory] PRIMARY KEY CLUSTERED ([PriceCategoryId] ASC)
);
