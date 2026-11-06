MERGE INTO [sec].[Role] AS [Target]
USING (VALUES
    (1, N'User'),
    (2, N'Moderator'),
    (3, N'Admin')
) AS [Source] ([RoleId], [Name])
ON [Target].[RoleId] = [Source].[RoleId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([RoleId], [Name])
    VALUES ([Source].[RoleId], [Source].[Name]);
