CREATE TABLE [tkt].[SeatState]
(
    [SeatStateId] TINYINT      NOT NULL,
    [Name]        NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_SeatState] PRIMARY KEY CLUSTERED ([SeatStateId] ASC)
);
