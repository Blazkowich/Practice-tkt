CREATE TABLE [tkt].[Row]
(
    [RowId]     INT          IDENTITY (1, 1) NOT NULL,
    [SectionId] INT          NOT NULL,
    [Name]      NVARCHAR(50) NOT NULL,
    [SortOrder] INT          NOT NULL CONSTRAINT [DF_Row_SortOrder] DEFAULT 0,
    CONSTRAINT [PK_Row]         PRIMARY KEY CLUSTERED ([RowId] ASC),
    CONSTRAINT [FK_Row_Section] FOREIGN KEY ([SectionId]) REFERENCES [tkt].[Section] ([SectionId])
);
