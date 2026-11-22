-- E35 Row A  : VenueSeatId  1-5   (SectionId=1, RowId=1)
-- E35 Row B  : VenueSeatId  6-10  (SectionId=1, RowId=2)
-- N22 GA     : VenueSeatId 11-15  (SectionId=2, RowId=NULL, SeatNumber=NULL)
-- S44 Row A  : VenueSeatId 16-18  (SectionId=3, RowId=3)
SET IDENTITY_INSERT [tkt].[VenueSeat] ON;

MERGE INTO [tkt].[VenueSeat] AS [Target]
USING (VALUES
    ( 1, 1,    1, N'1',  1),
    ( 2, 1,    1, N'2',  2),
    ( 3, 1,    1, N'3',  3),
    ( 4, 1,    1, N'4',  4),
    ( 5, 1,    1, N'5',  5),
    ( 6, 1,    2, N'1',  1),
    ( 7, 1,    2, N'2',  2),
    ( 8, 1,    2, N'3',  3),
    ( 9, 1,    2, N'4',  4),
    (10, 1,    2, N'5',  5),
    (11, 2, NULL, NULL,  1),
    (12, 2, NULL, NULL,  2),
    (13, 2, NULL, NULL,  3),
    (14, 2, NULL, NULL,  4),
    (15, 2, NULL, NULL,  5),
    (16, 3,    3, N'1',  1),
    (17, 3,    3, N'2',  2),
    (18, 3,    3, N'3',  3)
) AS [Source] ([VenueSeatId], [SectionId], [RowId], [SeatNumber], [SortOrder])
ON [Target].[VenueSeatId] = [Source].[VenueSeatId]
WHEN MATCHED THEN
    UPDATE SET
        [SectionId]  = [Source].[SectionId],
        [RowId]      = [Source].[RowId],
        [SeatNumber] = [Source].[SeatNumber],
        [SortOrder]  = [Source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([VenueSeatId], [SectionId], [RowId], [SeatNumber], [SortOrder])
    VALUES ([Source].[VenueSeatId], [Source].[SectionId], [Source].[RowId], [Source].[SeatNumber], [Source].[SortOrder]);

SET IDENTITY_INSERT [tkt].[VenueSeat] OFF;
