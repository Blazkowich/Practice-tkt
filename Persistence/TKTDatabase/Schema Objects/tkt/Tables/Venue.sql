CREATE TABLE [tkt].[Venue]
(
    [VenueId]   INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR(200) NOT NULL,
    [Address]   NVARCHAR(500) NULL,
    [City]      NVARCHAR(100) NULL,
    [Country]   NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 (0) NOT NULL CONSTRAINT [DF_Venue_CreatedAt] DEFAULT GETUTCDATE(),
    [IsActive]  BIT           NOT NULL CONSTRAINT [DF_Venue_IsActive]   DEFAULT 1,
    CONSTRAINT [PK_Venue] PRIMARY KEY CLUSTERED ([VenueId] ASC)
);
