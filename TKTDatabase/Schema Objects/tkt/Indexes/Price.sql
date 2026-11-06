CREATE INDEX [IX_Price_OfferId] ON [tkt].[Price] ([OfferId] ASC);
GO
CREATE INDEX [IX_Price_SectionId] ON [tkt].[Price] ([SectionId] ASC) WHERE [SectionId] IS NOT NULL;
