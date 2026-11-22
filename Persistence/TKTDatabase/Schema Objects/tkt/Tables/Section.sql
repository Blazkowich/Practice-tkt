CREATE TABLE [tkt].[Section]
(
    [SectionId]  INT           IDENTITY (1, 1) NOT NULL,
    [VenueId]    INT           NOT NULL,
    [Name]       NVARCHAR(100) NOT NULL,
    [SeatTypeId] TINYINT       NOT NULL,
    [SortOrder]  INT           NOT NULL CONSTRAINT [DF_Section_SortOrder] DEFAULT 0,
    CONSTRAINT [PK_Section]          PRIMARY KEY CLUSTERED ([SectionId] ASC),
    CONSTRAINT [FK_Section_Venue]    FOREIGN KEY ([VenueId])    REFERENCES [tkt].[Venue]    ([VenueId]),
    CONSTRAINT [FK_Section_SeatType] FOREIGN KEY ([SeatTypeId]) REFERENCES [tkt].[SeatType] ([SeatTypeId])
);
