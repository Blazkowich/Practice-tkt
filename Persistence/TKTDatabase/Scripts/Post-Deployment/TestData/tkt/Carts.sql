-- Cart 1 : User 1 (Admin), Event 1 — active, holds EventSeat 6 (Booked)
-- Cart 2 : User 2 (Moderator), Event 1 — completed, led to Order 1 / Ticket 1
MERGE INTO [tkt].[Cart] AS [Target]
USING (VALUES
    ('00000001-0000-0000-0000-000000000001', '00000011-0000-0000-0000-000000000001', 1, '2099-12-31T23:59:59'),
    ('00000002-0000-0000-0000-000000000002', '00000012-0000-0000-0000-000000000002', 1, NULL)
) AS [Source] ([CartId], [UserId], [EventId], [ExpiresAt])
ON [Target].[CartId] = [Source].[CartId]
WHEN MATCHED THEN
    UPDATE SET
        [UserId]    = [Source].[UserId],
        [EventId]   = [Source].[EventId],
        [ExpiresAt] = [Source].[ExpiresAt]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([CartId], [UserId], [EventId], [ExpiresAt])
    VALUES ([Source].[CartId], [Source].[UserId], [Source].[EventId], [Source].[ExpiresAt]);
