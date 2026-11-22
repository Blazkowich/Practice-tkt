CREATE INDEX [IX_VenueSeat_SectionId] ON [tkt].[VenueSeat] ([SectionId] ASC);
GO
CREATE INDEX [IX_VenueSeat_RowId] ON [tkt].[VenueSeat] ([RowId] ASC) WHERE [RowId] IS NOT NULL;
