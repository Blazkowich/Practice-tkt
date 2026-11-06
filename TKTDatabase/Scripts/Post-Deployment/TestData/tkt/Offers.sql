SET IDENTITY_INSERT [tkt].[Offer] ON;

MERGE INTO [tkt].[Offer] AS [Target]
USING (VALUES
    (1, 1, N'General Sale', '2026-12-01T00:00:00', '2027-03-15T18:00:00', 1),
    (2, 2, N'General Sale', '2026-12-01T00:00:00', '2027-03-16T18:00:00', 1)
) AS [Source] ([OfferId], [EventId], [Name], [SaleStartAt], [SaleEndAt], [IsActive])
ON [Target].[OfferId] = [Source].[OfferId]
WHEN MATCHED THEN
    UPDATE SET
        [EventId]     = [Source].[EventId],
        [Name]        = [Source].[Name],
        [SaleStartAt] = [Source].[SaleStartAt],
        [SaleEndAt]   = [Source].[SaleEndAt],
        [IsActive]    = [Source].[IsActive]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([OfferId], [EventId], [Name], [SaleStartAt], [SaleEndAt], [IsActive])
    VALUES ([Source].[OfferId], [Source].[EventId], [Source].[Name], [Source].[SaleStartAt], [Source].[SaleEndAt], [Source].[IsActive]);

SET IDENTITY_INSERT [tkt].[Offer] OFF;
