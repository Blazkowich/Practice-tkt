-- N22 is General Admission — no rows.
SET IDENTITY_INSERT [tkt].[Row] ON;

MERGE INTO [tkt].[Row] AS [Target]
USING (VALUES
    (1, 1, N'A', 1),
    (2, 1, N'B', 2),
    (3, 3, N'A', 1)
) AS [Source] ([RowId], [SectionId], [Name], [SortOrder])
ON [Target].[RowId] = [Source].[RowId]
WHEN MATCHED THEN
    UPDATE SET
        [SectionId] = [Source].[SectionId],
        [Name]      = [Source].[Name],
        [SortOrder] = [Source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([RowId], [SectionId], [Name], [SortOrder])
    VALUES ([Source].[RowId], [Source].[SectionId], [Source].[Name], [Source].[SortOrder]);

SET IDENTITY_INSERT [tkt].[Row] OFF;
