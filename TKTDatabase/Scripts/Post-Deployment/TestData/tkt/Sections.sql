-- SeatTypeId: 1=Reserved  2=GeneralAdmission
SET IDENTITY_INSERT [tkt].[Section] ON;

MERGE INTO [tkt].[Section] AS [Target]
USING (VALUES
    (1, 1, N'E35', 1, 1),
    (2, 1, N'N22', 2, 2),
    (3, 1, N'S44', 1, 3)
) AS [Source] ([SectionId], [VenueId], [Name], [SeatTypeId], [SortOrder])
ON [Target].[SectionId] = [Source].[SectionId]
WHEN MATCHED THEN
    UPDATE SET
        [VenueId]    = [Source].[VenueId],
        [Name]       = [Source].[Name],
        [SeatTypeId] = [Source].[SeatTypeId],
        [SortOrder]  = [Source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SectionId], [VenueId], [Name], [SeatTypeId], [SortOrder])
    VALUES ([Source].[SectionId], [Source].[VenueId], [Source].[Name], [Source].[SeatTypeId], [Source].[SortOrder]);

SET IDENTITY_INSERT [tkt].[Section] OFF;
