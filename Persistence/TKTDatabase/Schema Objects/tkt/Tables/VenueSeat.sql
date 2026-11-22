-- Physical seats defined in the venue manifest.
-- RowId / SeatNumber are NULL for General Admission sections.
CREATE TABLE [tkt].[VenueSeat]
(
    [VenueSeatId] INT          IDENTITY (1, 1) NOT NULL,
    [SectionId]   INT          NOT NULL,
    [RowId]       INT          NULL,
    [SeatNumber]  NVARCHAR(20) NULL,
    [SortOrder]   INT          NOT NULL CONSTRAINT [DF_VenueSeat_SortOrder] DEFAULT 0,
    CONSTRAINT [PK_VenueSeat]         PRIMARY KEY CLUSTERED ([VenueSeatId] ASC),
    CONSTRAINT [FK_VenueSeat_Section] FOREIGN KEY ([SectionId]) REFERENCES [tkt].[Section] ([SectionId]),
    CONSTRAINT [FK_VenueSeat_Row]     FOREIGN KEY ([RowId])     REFERENCES [tkt].[Row]     ([RowId])
);
