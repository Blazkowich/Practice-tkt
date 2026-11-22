-- Price level within an offer. SectionId/RowId scope the price:
--   SectionId NULL  = price applies to all sections in the offer
--   RowId     NULL  = price applies to all rows in the section
CREATE TABLE [tkt].[Price]
(
    [PriceId]         INT            IDENTITY (1, 1) NOT NULL,
    [OfferId]         INT            NOT NULL,
    [SectionId]       INT            NULL,
    [RowId]           INT            NULL,
    [PriceCategoryId] TINYINT        NOT NULL,
    [Amount]          DECIMAL (10,2) NOT NULL,
    [Currency]        CHAR (3)       NOT NULL CONSTRAINT [DF_Price_Currency] DEFAULT 'USD',
    CONSTRAINT [PK_Price]               PRIMARY KEY CLUSTERED ([PriceId] ASC),
    CONSTRAINT [FK_Price_Offer]         FOREIGN KEY ([OfferId])         REFERENCES [tkt].[Offer]         ([OfferId]),
    CONSTRAINT [FK_Price_Section]       FOREIGN KEY ([SectionId])       REFERENCES [tkt].[Section]       ([SectionId]),
    CONSTRAINT [FK_Price_Row]           FOREIGN KEY ([RowId])           REFERENCES [tkt].[Row]           ([RowId]),
    CONSTRAINT [FK_Price_PriceCategory] FOREIGN KEY ([PriceCategoryId]) REFERENCES [tkt].[PriceCategory] ([PriceCategoryId])
);
