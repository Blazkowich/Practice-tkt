MERGE INTO [tkt].[PriceCategory] AS [Target]
USING (VALUES
    (1, N'Adult'),
    (2, N'Child'),
    (3, N'VIP'),
    (4, N'Student'),
    (5, N'Senior')
) AS [Source] ([PriceCategoryId], [Name])
ON [Target].[PriceCategoryId] = [Source].[PriceCategoryId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([PriceCategoryId], [Name])
    VALUES ([Source].[PriceCategoryId], [Source].[Name]);
