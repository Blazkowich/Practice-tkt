MERGE INTO [tkt].[EventStatus] AS [Target]
USING (VALUES
    (1, N'Draft'),
    (2, N'Published'),
    (3, N'OnSale'),
    (4, N'Completed'),
    (5, N'Cancelled')
) AS [Source] ([EventStatusId], [Name])
ON [Target].[EventStatusId] = [Source].[EventStatusId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([EventStatusId], [Name])
    VALUES ([Source].[EventStatusId], [Source].[Name]);
