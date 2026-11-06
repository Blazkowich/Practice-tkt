-- EventStatusId: 1=Draft 2=Published 3=OnSale 4=Completed 5=Cancelled
SET IDENTITY_INSERT [tkt].[Event] ON;

MERGE INTO [tkt].[Event] AS [Target]
USING (VALUES
    (1, 1, N'Rock Concert - Night 1', N'An unforgettable opening night.',       '2027-03-15T19:00:00', '2027-03-15T17:00:00', 3),
    (2, 1, N'Rock Concert - Night 2', N'The encore you have been waiting for.', '2027-03-16T19:00:00', '2027-03-16T17:00:00', 2)
) AS [Source] ([EventId], [VenueId], [Name], [Description], [EventDate], [DoorsOpenAt], [EventStatusId])
ON [Target].[EventId] = [Source].[EventId]
WHEN MATCHED THEN
    UPDATE SET
        [VenueId]       = [Source].[VenueId],
        [Name]          = [Source].[Name],
        [Description]   = [Source].[Description],
        [EventDate]     = [Source].[EventDate],
        [DoorsOpenAt]   = [Source].[DoorsOpenAt],
        [EventStatusId] = [Source].[EventStatusId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([EventId], [VenueId], [Name], [Description], [EventDate], [DoorsOpenAt], [EventStatusId])
    VALUES ([Source].[EventId], [Source].[VenueId], [Source].[Name], [Source].[Description], [Source].[EventDate], [Source].[DoorsOpenAt], [Source].[EventStatusId]);

SET IDENTITY_INSERT [tkt].[Event] OFF;
