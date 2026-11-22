CREATE TABLE [tkt].[Offer]
(
    [OfferId]     INT           IDENTITY (1, 1) NOT NULL,
    [EventId]     INT           NOT NULL,
    [Name]        NVARCHAR(200) NOT NULL,
    [SaleStartAt] DATETIME2 (0) NOT NULL,
    [SaleEndAt]   DATETIME2 (0) NULL,
    [IsActive]    BIT           NOT NULL CONSTRAINT [DF_Offer_IsActive]  DEFAULT 1,
    [CreatedAt]   DATETIME2 (0) NOT NULL CONSTRAINT [DF_Offer_CreatedAt] DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Offer]       PRIMARY KEY CLUSTERED ([OfferId] ASC),
    CONSTRAINT [FK_Offer_Event] FOREIGN KEY ([EventId]) REFERENCES [tkt].[Event] ([EventId])
);
