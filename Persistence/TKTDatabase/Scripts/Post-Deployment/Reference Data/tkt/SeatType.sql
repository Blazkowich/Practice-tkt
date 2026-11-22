MERGE INTO [tkt].[SeatType] AS [Target]
USING (VALUES
    (1, N'Reserved'),
    (2, N'GeneralAdmission')
) AS [Source] ([SeatTypeId], [Name])
ON [Target].[SeatTypeId] = [Source].[SeatTypeId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SeatTypeId], [Name])
    VALUES ([Source].[SeatTypeId], [Source].[Name]);
