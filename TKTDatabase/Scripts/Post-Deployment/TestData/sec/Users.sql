MERGE INTO [sec].[User] AS [Target]
USING (VALUES
    ('00000011-0000-0000-0000-000000000001', N'auth|admin001', N'admin@tkt.example.com', N'System',  N'Admin',   1),
    ('00000012-0000-0000-0000-000000000002', N'auth|mod001',   N'mod@tkt.example.com',   N'Event',   N'Manager', 1),
    ('00000013-0000-0000-0000-000000000003', N'auth|user001',  N'user@tkt.example.com',  N'Regular', N'User',    1)
) AS [Source] ([UserId], [ExternalUserId], [Email], [FirstName], [LastName], [IsActive])
ON [Target].[UserId] = [Source].[UserId]
WHEN MATCHED THEN
    UPDATE SET
        [ExternalUserId] = [Source].[ExternalUserId],
        [Email]          = [Source].[Email],
        [FirstName]      = [Source].[FirstName],
        [LastName]       = [Source].[LastName],
        [IsActive]       = [Source].[IsActive]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([UserId], [ExternalUserId], [Email], [FirstName], [LastName], [IsActive])
    VALUES ([Source].[UserId], [Source].[ExternalUserId], [Source].[Email], [Source].[FirstName], [Source].[LastName], [Source].[IsActive]);
