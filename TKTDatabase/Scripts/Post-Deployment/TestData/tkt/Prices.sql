-- PriceCategoryId: 1=Adult 2=Child 3=VIP
-- SectionId: 1=E35(Reserved)  2=N22(GA)  3=S44(Reserved)
SET IDENTITY_INSERT [tkt].[Price] ON;

MERGE INTO [tkt].[Price] AS [Target]
USING (VALUES
    (1, 1, 1, NULL, 1,  75.00, 'USD'),
    (2, 1, 1, NULL, 3, 150.00, 'USD'),
    (3, 1, 2, NULL, 1,  45.00, 'USD'),
    (4, 1, 3, NULL, 1,  65.00, 'USD'),
    (5, 2, 1, NULL, 1,  80.00, 'USD'),
    (6, 2, 1, NULL, 3, 160.00, 'USD'),
    (7, 2, 2, NULL, 1,  50.00, 'USD'),
    (8, 2, 3, NULL, 1,  70.00, 'USD')
) AS [Source] ([PriceId], [OfferId], [SectionId], [RowId], [PriceCategoryId], [Amount], [Currency])
ON [Target].[PriceId] = [Source].[PriceId]
WHEN MATCHED THEN
    UPDATE SET
        [OfferId]         = [Source].[OfferId],
        [SectionId]       = [Source].[SectionId],
        [RowId]           = [Source].[RowId],
        [PriceCategoryId] = [Source].[PriceCategoryId],
        [Amount]          = [Source].[Amount],
        [Currency]        = [Source].[Currency]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([PriceId], [OfferId], [SectionId], [RowId], [PriceCategoryId], [Amount], [Currency])
    VALUES ([Source].[PriceId], [Source].[OfferId], [Source].[SectionId], [Source].[RowId], [Source].[PriceCategoryId], [Source].[Amount], [Source].[Currency]);

SET IDENTITY_INSERT [tkt].[Price] OFF;
