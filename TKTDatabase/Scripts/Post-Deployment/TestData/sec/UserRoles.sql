MERGE INTO [sec].[UserRole] AS [Target]
USING (VALUES
    ('00000011-0000-0000-0000-000000000001', 3),
    ('00000012-0000-0000-0000-000000000002', 2),
    ('00000013-0000-0000-0000-000000000003', 1)
) AS [Source] ([UserId], [RoleId])
ON [Target].[UserId] = [Source].[UserId] AND [Target].[RoleId] = [Source].[RoleId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([UserId], [RoleId])
    VALUES ([Source].[UserId], [Source].[RoleId]);
