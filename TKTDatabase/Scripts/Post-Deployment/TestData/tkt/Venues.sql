SET IDENTITY_INSERT [tkt].[Venue] ON;

MERGE INTO [tkt].[Venue] AS [Target]
USING (VALUES
    (1, N'Rice-Eccles Stadium', N'451 S 1400 E', N'Salt Lake City', N'USA', 1)
) AS [Source] ([VenueId], [Name], [Address], [City], [Country], [IsActive])
ON [Target].[VenueId] = [Source].[VenueId]
WHEN MATCHED THEN
    UPDATE SET
        [Name]     = [Source].[Name],
        [Address]  = [Source].[Address],
        [City]     = [Source].[City],
        [Country]  = [Source].[Country],
        [IsActive] = [Source].[IsActive]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([VenueId], [Name], [Address], [City], [Country], [IsActive])
    VALUES ([Source].[VenueId], [Source].[Name], [Source].[Address], [Source].[City], [Source].[Country], [Source].[IsActive]);

SET IDENTITY_INSERT [tkt].[Venue] OFF;
