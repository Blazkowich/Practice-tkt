MERGE INTO [tkt].[SeatState] AS [Target]
USING (VALUES
    (1, N'Available'),
    (2, N'Booked'),
    (3, N'Sold')
) AS [Source] ([SeatStateId], [Name])
ON [Target].[SeatStateId] = [Source].[SeatStateId]
WHEN MATCHED THEN
    UPDATE SET [Name] = [Source].[Name]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SeatStateId], [Name])
    VALUES ([Source].[SeatStateId], [Source].[Name]);
