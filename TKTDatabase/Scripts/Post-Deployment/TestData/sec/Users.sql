MERGE INTO [sec].[User] AS [Target]
USING (VALUES
    ('00000011-0000-0000-0000-000000000001', N'admin@tkt.example.com', N'System',  N'Admin',   1),
    ('00000012-0000-0000-0000-000000000002', N'mod@tkt.example.com',   N'Event',   N'Manager', 1),
    ('00000013-0000-0000-0000-000000000003', N'user@tkt.example.com',  N'Regular', N'User',    1)
) AS [Source] ([UserId], [Email], [FirstName], [LastName], [IsActive])
ON [Target].[UserId] = [Source].[UserId]
WHEN MATCHED THEN
    UPDATE SET
        [Email]     = [Source].[Email],
        [FirstName] = [Source].[FirstName],
        [LastName]  = [Source].[LastName],
        [IsActive]  = [Source].[IsActive]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([UserId], [Email], [FirstName], [LastName], [IsActive])
    VALUES ([Source].[UserId], [Source].[Email], [Source].[FirstName], [Source].[LastName], [Source].[IsActive]);
