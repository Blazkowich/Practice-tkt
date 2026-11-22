SET IDENTITY_INSERT [tkt].[CartItem] ON;

MERGE INTO [tkt].[CartItem] AS [Target]
USING (VALUES
    (1, '00000001-0000-0000-0000-000000000001', 6),
    (2, '00000002-0000-0000-0000-000000000002', 7)
) AS [Source] ([CartItemId], [CartId], [EventSeatId])
ON [Target].[CartItemId] = [Source].[CartItemId]
WHEN MATCHED THEN
    UPDATE SET
        [CartId]      = [Source].[CartId],
        [EventSeatId] = [Source].[EventSeatId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([CartItemId], [CartId], [EventSeatId])
    VALUES ([Source].[CartItemId], [Source].[CartId], [Source].[EventSeatId]);

SET IDENTITY_INSERT [tkt].[CartItem] OFF;
