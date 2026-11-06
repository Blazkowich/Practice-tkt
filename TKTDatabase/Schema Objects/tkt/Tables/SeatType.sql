CREATE TABLE [tkt].[SeatType]
(
    [SeatTypeId] TINYINT      NOT NULL,
    [Name]       NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_SeatType] PRIMARY KEY CLUSTERED ([SeatTypeId] ASC)
);
